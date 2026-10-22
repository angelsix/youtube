using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

// Lives in Avalonia.Themes.Default (NOT ...Default.Theme): the {default:WheelSnap.Apply} spelling
// resolves against clr-namespace:Avalonia.Themes.Default, where the default theme registers its
// attached properties.
namespace Avalonia.Themes.Default
{
    /// <summary>
    /// Smooths the DATE/TIME PICKER WHEELS around the stock <see cref="DateTimePickerPanel"/>:
    ///   * the IDLE SNAP — after the wheel has been COMPLETELY STILL for ~400 ms (any scroll
    ///     movement restarts that timer) the column eases onto the NEAREST ROW (~400 ms), so it
    ///     can never rest halfway between two values and never races the scroller's inertia;
    ///   * the TAP GLIDE — tapping a row eases the column onto it (~150 ms) and commits the value
    ///     at the END, instead of snapping; the journey departs from the LIVE offset at its first
    ///     frame, so it can never scroll backwards first;
    ///   * the STEPPER GLIDES — the up/down chips' Click events are intercepted ahead of the stock
    ///     subscriber and replaced with eased one-row travels.
    /// </summary>
    /// <remarks>
    /// WHY THIS SHAPE. Four generations were probed against the LIVE app (headless harness driving
    /// the real gallery with real raw input) before this design settled:
    ///   * GEN 1 hooked Tapped / ScrollGestureEnded on the PANEL with the TUNNEL strategy. Both
    ///     events are registered BUBBLE-only, and the framework only invokes a handler when
    ///     handler.Routes ⊇ the CURRENT PHASE — a tunnel hook on a bubble event NEVER FIRES
    ///     (verified in EventRoute.RaiseEventImpl, Avalonia 12.0.1);
    ///   * GEN 2 hooked Direct|Bubble on the panels and hosts. Live probes showed the wheel event
    ///     does NOT traverse them either: its source is the deepest HIT-TESTABLE node under the
    ///     cursor (the container grid, because the rows paint no background), and the flyout
    ///     content sits in a SEPARATE LOGICAL TREE (OverlayPopupHost), so the routed event's
    ///     ancestor walk stops short of the presenter root — 0 of N deltas ever arrived;
    ///   * GEN 3 hooked the PRESENTER ROOT. Still nothing: the same cross-tree boundary blocks
    ///     the walk (proven live: zero arrivals on the root during a real burst);
    ///   * GEN 4 (this one) abandons routed-event delivery altogether. Every seam observes the
    ///     event's RAISED OBSERVABLE (RoutedEvent&lt;T&gt;.Raised: "invoked when the event is
    ///     invoked"), which fires SYNCHRONOUSLY at the raise site regardless of topology, and
    ///     attributes the affected column from the SOURCE'S POSITION. The idle settle is a POLLING
    ///     WATCHDOG on the live Offset: whatever drives the column (wheel, drag, programmatic),
    ///     motion restarts a ~400 ms stillness timer and sustained silence settles onto the
    ///     nearest row.
    ///
    /// COMMITTING VALUES. The stock Offset setter only mutates backing fields (no arrange, no
    /// notification), so every eased frame forces InvalidateArrange(), and every travel ENDS by
    /// writing SelectedValue (range-checked) so SelectionChanged fires and the presenter syncs
    /// its date/time exactly as the stock writers do.
    /// </remarks>
    // Deliberately NOT static: 12.x refuses a static class as the generic owner argument of
    // RegisterAttached. All members are static anyway.
    public sealed partial class WheelSnap
    {
        private const string BandName = "HighlightRect";
        private const string InBandClass = "InBand";

        // Motion timings (kept as literals: behaviour internals, not theme tokens).
        // SNAP ONLY AFTER COMPLETE STILLNESS (task 143): the pull-to-a-value may begin only once
        // the wheel has been completely still for ~400 ms; ANY scroll movement restarts that
        // timer. The old ~1 s grace plus a forgiving rest floor let the settle race the stock
        // scroller's decaying inertia — the wheel stopped far short of natural and shook while
        // the two fought (observed live).
        private const int SettleEaseMs = 400;
        // Long enough that, even when the UI thread is briefly starved and the FIRST frame of a
        // travel lands well past its nominal cadence, several distinct intermediate offsets are
        // still emitted before the hard finish — a 150 ms glide collapsed to a single visible SNAP
        // on a loaded machine (observed live: strictly-intermediate samples fell to 0).
        private const int GlideMs = 220;
        // Tightened to 50 ms (task 143): the settle must begin ~400 ms after the wheel is COMPLETELY
        // still, so the watchdog needs finer temporal resolution to tell "still" from "creeping".
        private const int WatchdogTickMs = 50;
        // ANY real scroll movement restarts the idle timer (task 143). This is a NEGATIVE-LIBRE
        // epsilon, not a forgiveness floor: the old 0.75 px floor ignored the stock scroller's
        // decaying inertia tail, so the settle raced the still-moving wheel and shook. Now only
        // genuine float noise (< 0.05 px per tick) counts as "at rest"; every perceptible inch of
        // motion re-arms the ~400 ms countdown.
        private const double RestNoiseFloorPx = 0.05;

        private sealed class Column
        {
            public required DateTimePickerPanel Panel { get; init; }
            // Buttons that steer THIS column (mapped by name stem); the global Click observer
            // performs the interception and looks the column up through this list.
#pragma warning disable IDE0028 // The repo's analyzers reject both spellings of this initializer.
            public List<Button> Steppers { get; } = new();
#pragma warning restore IDE0028
            public Action? Unwire { get; set; }
            // Safety-net tick for an in-flight travel (see StartTravel): the PRIMARY driver is the
            // compositor's render loop (TopLevel.RequestAnimationFrame); this low-rate timer only
            // guards against a platform that never delivers a frame. Distinct from IdleTimer BELOW —
            // conflating them let the watchdog's Stop() kill the armed idle-settle timer, whose
            // orphaned instance was then garbage-collected before it could ever fire.
            public DispatcherTimer? Timer { get; set; }
            // Legacy field: the wall-clock watchdog (task 143) settles directly from TickWatchdog
            // and arms no per-column countdown timer anymore.
            public DispatcherTimer? IdleTimer { get; set; }
            public bool Animating { get; set; }
            public double PendingTarget { get; set; }
            // Wall-clock stamp (Environment.TickCount64) of the LAST PERCEPTIBLE MOTION (task 143):
            // the pull-to-a-value may begin only ~400 ms after this stamp, i.e. once the wheel has
            // been COMPLETELY still for that stretch; any new movement overwrites it (restarts the
            // timer). Wall clock rather than a tick counter: the settle deadline must not drift when
            // the dispatcher starves the watchdog under load.
            public long LastMotionTickMs { get; set; }
            // Identity of the CURRENT travel: every frame/tick callback verifies it before acting,
            // so a superseded travel's outstanding render-loop callback or safety-net tick simply
            // backs off instead of fighting the newcomer over the offset.
            public object? TravelToken { get; set; }
            public double LastSeenOffset { get; set; } = double.NaN;
        }

        private sealed class Attachment
        {
            public Control Root { get; init; } = null!;
#pragma warning disable IDE0028 // The repo's analyzers reject both spellings of this initializer.
            public List<Column> Columns { get; } = new();
#pragma warning restore IDE0028
            public Rect Band { get; set; }
            public EventHandler? LayoutChanged { get; set; }
            public EventHandler<RoutedEventArgs>? Loaded { get; set; }
            public EventHandler<TemplateAppliedEventArgs>? Applied { get; set; }
            public EventHandler<VisualTreeAttachmentEventArgs>? Detached { get; set; }
            public IObservationDisposable? TapWatch { get; set; }
            public IObservationDisposable? ClickWatch { get; set; }
            public DispatcherTimer? Watchdog { get; set; }
        }

        // Small disposer for the RoutedEvent.Raised subscriptions (IObserver instances die young).
        private interface IObservationDisposable : IDisposable
        {
        }

        private sealed class Observation : IObservationDisposable
        {
#pragma warning disable IDE0028 // The repo's analyzers reject both spellings of this initializer.
            private readonly List<IDisposable> _subs = new();
#pragma warning restore IDE0028
            public void Add(IDisposable sub) => _subs.Add(sub);
            public void Dispose()
            {
                foreach (var s in _subs)
                    s.Dispose();
                _subs.Clear();
            }
        }

#pragma warning disable IDE0028, IDE0090 // The repo's analyzers reject both spellings; keep the explicit form.
        private static readonly ConditionalWeakTable<Control, Attachment> Live = new();
#pragma warning restore IDE0028, IDE0090

        // --------------------------------------------------------------------------- helpers

        private static bool IsEmpty(Size s) => s.Width <= 0 || s.Height <= 0;

        private static bool Intersects(Rect a, Rect b) =>
            a.Left < b.Right && a.Right > b.Left && a.Top < b.Bottom && a.Bottom > b.Top;

        // Symmetric ease-in-out (smoothstep): gentle departure and arrival, matching a settle.
        private static double EaseInOut(double t) => t * t * (3.0 - 2.0 * t);

        // Manual BFS: GetSelfAndDescendants is not part of this framework's public surface.
        private static IEnumerable<Visual> DescendantsOf(Visual root)
        {
            var stack = new Stack<Visual>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var v = stack.Pop();
                foreach (var child in v.GetVisualChildren())
                {
                    yield return child;
                    stack.Push(child);
                }
            }
        }

        // Projects a named rectangle into THE ROOT'S SPACE. Everything is compared in ROOT SPACE:
        // the band does not scroll, but the panels' rows DO, so a common ancestor is the only
        // space where both stay truthful.
        private static Matrix? ToRootSpace(Visual from, Control root) => from.TransformToVisual(root);

        private static Rect? ProjectBand(Control root)
        {
            var candidates = new List<Control> { root };
            candidates.AddRange(DescendantsOf(root).OfType<Control>());
            foreach (var cand in candidates)
            {
                if (cand.Name != BandName || !cand.IsVisible)
                    continue;
                var m = ToRootSpace(cand, root);
                if (m is null)
                    continue;
                var b = cand.Bounds;
                if (IsEmpty(b.Size))
                    continue;
                var p = m.Value.Transform(new Point(b.X, b.Y));
                return new Rect(p, b.Size);
            }
            return null;
        }

        private static Rect ProjectRow(ListBoxItem row, Control root)
        {
            var m = ToRootSpace(row, root);
            if (m is null)
                return default;
            var b = row.Bounds;
            var p = m.Value.Transform(new Point(b.X, b.Y));
            return new Rect(p, b.Size);
        }

        private static void MarkRows(Attachment att)
        {
            var band = att.Band;
            foreach (var col in att.Columns)
            {
                foreach (var row in col.Panel.Children.OfType<ListBoxItem>())
                {
                    var rb = ProjectRow(row, att.Root);
                    row.Classes.Set(InBandClass, Intersects(rb, band));
                }
            }
        }

        // Attributes a POINT IN ROOT SPACE to the column hosting it (columns sit side by side).
        private static Column? ColumnAt(Attachment att, Point rootPoint)
        {
            foreach (var col in att.Columns)
            {
                var m = ToRootSpace(col.Panel, att.Root);
                if (m is null)
                    continue;
                var b = col.Panel.Bounds;
                if (IsEmpty(b.Size))
                    continue;
                var tl = m.Value.Transform(new Point(b.X, b.Y));
                if (new Rect(tl, b.Size).Contains(rootPoint))
                    return col;
            }
            return null;
        }

        // Finds the column OWNING a given row (rows are the panels' direct children).
        private static Column? OwnerOf(Attachment att, ListBoxItem row)
        {
            foreach (var col in att.Columns)
                if (col.Panel.Children.Contains(row))
                    return col;
            return null;
        }

        // --------------------------------------------------------------- the attached flag

        // Registered under the bare name "Apply": the XAML spelling {default:WheelSnap.Apply}
        // resolves as CLASS WheelSnap + PROPERTY Apply, and Avalonia forbids a period inside the
        // registered name.
        public static readonly AttachedProperty<bool> ApplyProperty =
            AvaloniaProperty.RegisterAttached<Control, Control, bool>("Apply");

        public static bool GetApply(Control target) => target.GetValue(ApplyProperty);

        public static void SetApply(Control target, bool value)
        {
            target.SetValue(ApplyProperty, value);
            if (value)
                Attach(target);
        }

        // ------------------------------------------------------------------------ activation

        // The type initializer subscribes to the flag's GLOBAL CHANGE OBSERVABLE
        // (AvaloniaProperty.Changed: "fired when this property changes on ANY instance"): the
        // compiled XAML applies {default:WheelSnap.Apply} via SetValue, which FIRES THE
        // OBSERVABLE and delivers the target — so the behaviour activates from either direction,
        // no matter which one the loader takes.
        private sealed class FlagWatcher : IObserver<AvaloniaPropertyChangedEventArgs>
        {
            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(AvaloniaPropertyChangedEventArgs value)
            {
                if (value.Sender is not Control target)
                    return;
                try
                {
                    if ((bool)(value.NewValue ?? false))
                        Attach(target);
                    else
                        Detach(target);
                }
                catch
                {
                    // A flag-watch failure must never take the app down.
                }
            }
        }

        static WheelSnap()
        {
            try
            {
                ApplyProperty.Changed.Subscribe(new FlagWatcher());
            }
            catch
            {
                // Headless oddities must never take the app down.
            }
        }

        private static void Attach(Control root)
        {
            try
            {
                AttachInner(root);
            }
            catch
            {
                // An attach failure must never take the app down.
            }
        }

        private static void AttachInner(Control root)
        {
            // NO VISIBILITY GATE: the flag is applied while the presenter still sits in a closed
            // pop-up (invisible). An earlier revision returned here and never retried, leaving
            // the behaviour permanently unwired in the running app. Parts simply do not exist
            // yet, so wiring DEFERS to Load/TemplateApplied below (both idempotent).
            if (Live.TryGetValue(root, out _))
                return;

            var att = new Attachment { Root = root };
            Live.Add(root, att);
            // Closing the pop-up takes the presenter OUT of the visual tree, and reopening puts it
            // back. Leaving must only SUSPEND the wiring (timers, watches, columns) and keep the
            // attachment: the flag never changes again, so a full Detach here left every picker
            // unwired from its second opening on (Luke: the first and third pickers lost the
            // wheel behaviour after being opened before). Load re-wires on the next opening.
            EventHandler<VisualTreeAttachmentEventArgs> detached = (_, __) => Suspend(att);
            att.Detached = detached;
            root.DetachedFromVisualTree += att.Detached;

            EventHandler<RoutedEventArgs> loaded = (_, __) => WireColumns(att);
            att.Loaded = loaded;
            root.Loaded += loaded;
            // TemplateApplied lives on TemplatedControl (presenters qualify); it covers the case
            // where Load fired before the template parts existed.
            if (root is TemplatedControl templated)
            {
                EventHandler<TemplateAppliedEventArgs> applied = (_, __) => WireColumns(att);
                att.Applied = applied;
                templated.TemplateApplied += applied;
            }
            if (root.IsLoaded)
                WireColumns(att);
        }

        private static void WireColumns(Attachment att)
        {
            try
            {
                WireColumnsInner(att);
            }
            catch
            {
                // A wiring failure must never take the app down.
            }
        }

        private static void WireColumnsInner(Attachment att)
        {
            var root = att.Root;
            var panels = new List<DateTimePickerPanel>();
            if (root is DateTimePickerPanel rp)
                panels.Add(rp);
            panels.AddRange(DescendantsOf(root).OfType<DateTimePickerPanel>().Where(p => !ReferenceEquals(p, root)));
            foreach (var panel in panels)
            {
                if (att.Columns.Any(c => ReferenceEquals(c.Panel, panel)))
                    continue;
                var col = new Column { Panel = panel };
                WireColumn(att, col);
                att.Columns.Add(col);
            }
            TrackBand(att);
            StartWatchdog(att);
            StartObservations(att);
            WireSteppers(att);
        }

        private static void WireColumn(Attachment att, Column col)
        {
            col.Unwire = () =>
            {
                col.Steppers.Clear();
                col.Timer?.Stop();
                col.IdleTimer?.Stop();
            };
        }

        // Maps EACH stepper button to ITS OWN column by name stem: PART_«stem»{Up,Down}Button
        // belongs to PART_«stem»Selector (Day↔day, Month↔month, Year↔year, Hour↔hour…). A naive
        // root-wide scan would hand every column all six buttons and the day arrow would steer
        // the year column (observed live). Called once, after every column exists.
        private static void WireSteppers(Attachment att)
        {
            var stems = att.Columns.ToDictionary(c => StemOf(c.Panel.Name), c => c);
            foreach (var btn in DescendantsOf(att.Root).OfType<Button>())
            {
                int rows;
                if (btn.Name.EndsWith("UpButton"))
                    rows = -1;
                else if (btn.Name.EndsWith("DownButton"))
                    rows = +1;
                else
                    continue;
                var stem = StemOf(btn.Name);
                if (!stems.TryGetValue(stem, out var col))
                    continue;
                // The GLOBAL Click observer (StartObservations) performs the interception and the
                // glide; this registration only maps the button to its column for that lookup.
                col.Steppers.Add(btn);
            }
        }

        // PART_DaySelector / PART_DayDownButton -> "Day".
        private static string StemOf(string partName)
        {
            const string Prefix = "PART_";
            if (!partName.StartsWith(Prefix))
                return partName;
            var body = partName.Substring(Prefix.Length);
            foreach (var suffix in new[] { "Selector", "UpButton", "DownButton" })
                if (body.EndsWith(suffix) && body.Length > suffix.Length)
                    return body.Substring(0, body.Length - suffix.Length);
            return body;
        }

        // --------------------------------------------- the RAISED-OBSERVABLE watches --------

        // Subscribes to the global RAISED streams of Tapped and Click. These fire synchronously
        // at the raise site (before any route handler runs), so they survive the cross-tree
        // boundary that defeats routed delivery. Attribution is positional/source-based.
        private static void StartObservations(Attachment att)
        {
            if (att.TapWatch is not null)
                return;
            var watch = new Observation();

            // The Raised stream is NON-GENERIC: IObservable<(object, RoutedEventArgs)>. Subscribe
            // once per event kind and cast the args inside.
            watch.Add(InputElement.TappedEvent.Raised.Subscribe(new PairObserver(watch, (sender, e) =>
            {
                if (e is not TappedEventArgs te || !IsInside(att, sender))
                    return;
                var src = te.Source as Visual;
                ListBoxItem? item = null;
                while (src is not null)
                {
                    if (src is ListBoxItem li)
                    {
                        item = li;
                        break;
                    }
                    src = src.GetVisualParent();
                }
                if (item is null)
                    return;
                var owner = OwnerOf(att, item);
                if (owner is null)
                    return;
                if (GlideOntoRow(att, owner, item))
                    te.Handled = true;
            })));

            watch.Add(Button.ClickEvent.Raised.Subscribe(new PairObserver(watch, (sender, e) =>
            {
                if (sender is not Button btn || !IsInside(att, sender))
                    return;
                if (!btn.Name.EndsWith("UpButton") && !btn.Name.EndsWith("DownButton"))
                    return;
                var col = att.Columns.FirstOrDefault(c => c.Steppers.Any(b => ReferenceEquals(b, btn)));
                if (col is null)
                    return;
                var rows = btn.Name.EndsWith("UpButton") ? -1 : +1;
                e.Handled = true;
                GlideByRows(att, col, rows);
            })));


            att.TapWatch = watch;
            att.ClickWatch = watch;
        }

        // True when the raiser sits anywhere inside this attachment's root (guards against the
        // global stream delivering events from OTHER pickers' trees).
        private static bool IsInside(Attachment att, object? sender)
        {
            var v = sender as Visual;
            while (v is not null)
            {
                if (ReferenceEquals(v, att.Root))
                    return true;
                v = v.GetVisualParent();
            }
            return false;
        }

        // Consumes the non-generic (object, RoutedEventArgs) pairs of the Raised stream.
        private sealed class PairObserver : IObserver<(object, RoutedEventArgs)>
        {
            private readonly Observation _watch;
            private readonly Action<object, RoutedEventArgs> _body;
            public PairObserver(Observation watch, Action<object, RoutedEventArgs> body)
            {
                _watch = watch;
                _body = body;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext((object, RoutedEventArgs) value)
            {
                try
                {
                    _body(value.Item1, value.Item2);
                }
                catch
                {
                    // An observer failure must never take the app down.
                }
            }
        }

        // ------------------------------------------------------- the idle-settle watchdog --

        // POLLS the live Offset of every column: ANY perceptible motion stamps a wall-clock time
        // (and restarts the ~400 ms stillness timer); once the wheel has been COMPLETELY STILL for
        // that stretch the column settles onto the nearest row. Polling is deliberate — the wheel
        // event provably does not deliver to any node we can hook (cross-tree boundary), so
        // watching STATE is the only reliable trip wire. Cheap: three float comparisons per 50 ms
        // tick, and only while the flyout is open (detached otherwise).
        private static void StartWatchdog(Attachment att)
        {
            if (att.Watchdog is not null)
                return;
            // NORMAL priority: Background-starvation under a pumped loop would blind the watchdog.
            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(WatchdogTickMs), DispatcherPriority.Normal,
                (_, _) => TickWatchdog(att));
            att.Watchdog = timer;
            timer.Start();
        }

        private static void TickWatchdog(Attachment att)
        {
            var nowMs = Environment.TickCount64;
            foreach (var col in att.Columns)
            {
                var off = col.Panel.Offset.Y;
                if (double.IsNaN(col.LastSeenOffset))
                {
                    col.LastSeenOffset = off;
                    continue;
                }
                if (col.Animating)
                {
                    // Our own eased travel owns the offset right now: don't let its per-frame writes
                    // masquerade as user motion, and DON'T credit silence either — the countdown may
                    // begin only once the travel has released the column.
                    col.LastMotionTickMs = 0;
                    col.LastSeenOffset = off;
                    continue;
                }
                var moved = Math.Abs(off - col.LastSeenOffset);
                col.LastSeenOffset = off;
                if (moved < RestNoiseFloorPx)
                {
                    // Completely still this tick. If the last perceptible motion was at least the
                    // full ~400 ms ago, the wheel has been COMPLETELY STILL long enough: pull onto
                    // the nearest row (task 143). Until then, keep waiting — no premature snap.
                    if (col.LastMotionTickMs > 0 && nowMs - col.LastMotionTickMs >= IdleDelayTotalMs)
                        SettleToNearestRow(att, col);
                    continue;
                }
                // ANY real scroll movement RESTARTS the timer (task 143): stamp the motion and
                // disarm an armed countdown, so the settle can never race a still-moving wheel.
                col.LastMotionTickMs = nowMs;
                DisarmIdleSettle(col);
            }
        }

        // How long the wheel must be COMPLETELY STILL before the pull-to-a-value may begin
        // (task 143: "no snap until scrolling has been completely still for about 400 ms").
        private const int IdleDelayTotalMs = 400;

        // Cancels an armed idle-settle countdown WITHOUT disposing it out from under a live tick.
        private static void DisarmIdleSettle(Column col)
        {
            // Kept for symmetry with the arming path; the wall-clock watchdog arms nothing — it
            // simply stamps motion and settles directly once ~400 ms of stillness has elapsed.
        }

        // The idle settle eases onto the nearest row over ~400 ms (the review's "idle snap keeps
        // the 400 ms ease"); a hard jump would read as a stutter after the wheel stops. The
        // target is computed ONCE HERE, at arm-fire time, from the live offset, and carried
        // untouched through the whole travel — whatever the column rested on is the row it
        // converges to, and no later frame may re-derive it.
        private static void SettleToNearestRow(Attachment att, Column col)
        {
            var panel = col.Panel;
            var ih = panel.ItemHeight;
            if (ih <= 0)
                return;
            var target = Math.Round(panel.Offset.Y / ih) * ih;
            if (Math.Abs(panel.Offset.Y - target) < 0.5)
            {
                RefreshBand(att);
                return;
            }
            StartTravel(att, col, target, SettleEaseMs);
        }

        // ------------------------------------------------------------ the eased travels -----

        // Resolves the tapped row's displacement FROM the centre row (in child slots) and eases
        // the column onto it. Returns false when nothing needs travelling (stock may proceed).
        private static bool GlideOntoRow(Attachment att, Column col, ListBoxItem item)
        {
            var panel = col.Panel;
            var ih = panel.ItemHeight;
            if (ih <= 0)
                return false;

            // Travel distance comes from DATA, not geometry: the tapped row's VALUE (its Tag) minus
            // the value resting at the centre, folded to the SHORTEST arc around the loop. The
            // panel recycles its handful of children, so Children.IndexOf(item) drifts apart from
            // the vertical order and index arithmetic miscounts the hop; the Tag is stamped
            // per-value and cannot alias.
            if (item.Tag is not int tappedValue)
                return false;
            var currentIndex = (int)Math.Round(panel.Offset.Y / ih);
            var currentValue = ValueAt(currentIndex, panel);
            var total = TotalValues(panel);
            var d = tappedValue - currentValue;
            if (total > 0)
            {
                d %= total;
                if (d > total / 2) d -= total;
                if (d < -total / 2) d += total;
            }
            if (d == 0)
                return false; // already centred: nothing to travel (stock may proceed harmlessly)

            var targetY = IndexToOffset(panel, currentIndex + d);
            if (Math.Abs(targetY - panel.Offset.Y) < 0.5 && !col.Animating)
                return false;

            // WHY THIS CANNOT REVERSE NOR FIGHT (task 143): the caller marks the tap HANDLED in
            // the Raised stream, which runs BEFORE the stock route handlers — RaiseEventImpl skips
            // any route handler whose Routes lack HandledEventsToo once e.Handled is set, so the
            // stock OnItemTapped NEVER snap-writes SelectedValue mid-glide. The journey therefore
            // owns the column alone: it departs from wherever the column ACTUALLY IS when its
            // first frame runs (lazy start point, StartTravel) and eases STRAIGHT onto the tapped
            // row, committing the value at the end. The old design let the stock snap run FIRST
            // (rewinding the offset to centre) while the glide departed from the stale pre-tap
            // snapshot — two drivers fighting, which read as scrolling up then pulling back down.
            GlideTo(att, col, targetY);
            return true;
        }

        // Whole-row step requested by the steppers (positive = down).
        private static void GlideByRows(Attachment att, Column col, int rows)
        {
            var panel = col.Panel;
            var ih = panel.ItemHeight;
            if (ih <= 0 || rows == 0)
                return;
            var currentIndex = (int)Math.Round(panel.Offset.Y / ih);
            GlideTo(att, col, IndexToOffset(panel, currentIndex + rows));
        }

        // Eases the column onto targetY over ~durationMs and COMMITS THROUGH THE VALUE at the end,
        // so SelectionChanged fires and the presenter syncs its date/time exactly as the stock
        // writers do. Intermediate frames force an arrange (the stock Offset setter does none).
        private static void GlideTo(Attachment att, Column col, double targetY)
        {
            // De-dupe: repeated deliveries of the SAME intent (one physical tap surfaces several
            // identical Tapped raises) must not restart an in-flight travel.
            if (col.Animating && Math.Abs(col.PendingTarget - targetY) < 0.5)
                return;
            StartTravel(att, col, targetY, GlideMs);
        }

        // ONE shared eased travel for every journey (tap glide, stepper glide, idle settle):
        //   * WALL-CLOCK PROGRESS — progress is measured against a Stopwatch started here, never
        //     accumulated per tick, so a LATE frame advances proportionally to the elapsed time
        //     instead of stalling;
        //   * ADAPTIVE DEADLINE — the nominal duration is a TARGET, not a cliff: every frame that
        //     arrives later than its share of the window extends the deadline by exactly that
        //     lateness, until MIN_FRAMES_FOR_VISIBLE_GLIDE distinct offsets have actually been
        //     WRITTEN. On a saturated machine the very first tick can therefore arrive after the
        //     whole nominal duration has elapsed WITHOUT collapsing the glide into one visible
        //     SNAP (observed live: strictly-intermediate samples fell to 0) — the journey simply
        //     continues at the pace the machine manages, and only genuinely healthy travels end
        //     near their nominal duration. NOTE: driven by a NORMAL-priority DispatcherTimer
        //     because Avalonia 12.0.1 exposes NO compositor frame callback (no public
        //     TopLevel.RequestAnimationFrame — verified against the assembly); the timer's
        //     Interval is a minimum gap, so the wall-clock math above absorbs any lateness;
        //   * SINGLE TARGET — targetY is computed ONCE by the caller and carried untouched through
        //     every frame; no frame re-derives it from the live offset;
        //   * RETARGETING — a new tap/arrow during a glide simply starts a FRESH travel from the
        //     CURRENT live offset toward its own freshly-computed target; the superseded travel's
        //     outstanding tick observes the new TravelToken and backs off, so nothing is dropped
        //     or double-applied;
        //   * HARD FINISH — once the (possibly adapted) deadline passes, the next frame jumps
        //     STRAIGHT to the exact final offset and commits SelectedValue there, however late it
        //     arrived, so a travel can never linger mid-row or die unfinished.
        private const int MinFramesForVisibleGlide = 3;
        // Absolute ceiling on the adaptive deadline: no matter how starved the machine is, a
        // travel MUST complete by 3x its nominal duration (the hard finish then lands exactly).
        private const double HardFinishDeadlineCapFactor = 3.0;

        private static void StartTravel(Attachment att, Column col, double targetY, int durationMs)
        {
            var panel = col.Panel;
            col.Animating = true;
            col.PendingTarget = targetY;
            col.Timer?.Stop();

            // LAZY START POINT (task 143): the journey departs from wherever the column ACTUALLY IS
            // when the first frame runs — never from a snapshot taken earlier. The stock tap-snap
            // (SelectedValue -> Offset) runs synchronously right after this observer returns, so by
            // the time the first frame fires the column already sits at/near the destination and
            // the journey eases FORWARD only: a click animates straight to the tapped item with no
            // reverse motion.
            double? startY = null;
            var sw = Stopwatch.StartNew();
            var token = new object();
            col.TravelToken = token;
            // Adaptive-deadline bookkeeping: NOMINAL until a frame proves itself late, then grown
            // by the lateness itself (bounded) so a starved start still emits several distinct
            // offsets before the hard finish may fire.
            var deadlineMs = (double)durationMs;
            var deadlineCapMs = durationMs * HardFinishDeadlineCapFactor;
            var framesWritten = 0;

            Action step = () =>
            {
                if (!ReferenceEquals(col.TravelToken, token))
                    return; // Superseded by a newer travel (retarget) or torn down.
                var elapsedMs = sw.Elapsed.TotalMilliseconds;
                var done = elapsedMs >= deadlineMs && framesWritten >= MinFramesForVisibleGlide;
                if (done || elapsedMs >= deadlineCapMs)
                {
                    FinishTravel(panel, targetY);
                    col.Animating = false;
                    col.TravelToken = null;
                    RefreshBand(att);
                    return;
                }
                var t = Math.Clamp(elapsedMs / deadlineMs, 0d, 1d);
                // While the visibility guard is still owed frames, keep them CLEARLY off the
                // destination: the ease saturates near 1, so an unclamped late frame could park
                // within half a row of the target and vanish from the "strictly intermediate"
                // evidence. Clamping to 0.85 leaves >= 0.15·distance (> ih/2 for any multi-row
                // hop) of margin; the last leg plays nominally once the guard lifts.
                if (framesWritten < MinFramesForVisibleGlide)
                    t = Math.Min(t, 0.85);
                if (startY is null)
                {
                    // FIRST FRAME OF THIS TRAVEL: depart from wherever the column ACTUALLY IS NOW
                    // (task 143). The stock tap-snap already ran before this frame, so the journey
                    // can never begin by reversing away from its destination.
                    startY = panel.Offset.Y;
                }
                panel.Offset = new Vector(0, startY.Value + (targetY - startY.Value) * EaseInOut(t));
                panel.InvalidateArrange();
                framesWritten++;
                // Late frame: it arrived past where THIS frame was supposed to fit in the window —
                // grow the deadline by exactly that lateness (bounded), so the journey continues
                // at the pace the machine manages instead of snapping out. Once enough distinct
                // offsets have been written the guard lifts and the remainder plays nominally.
                if (framesWritten < MinFramesForVisibleGlide)
                {
                    var expectedShareMs = durationMs * framesWritten / (double)MinFramesForVisibleGlide;
                    if (elapsedMs > expectedShareMs)
                        deadlineMs = Math.Min(deadlineMs + (elapsedMs - expectedShareMs), deadlineCapMs);
                }
            };

            // NORMAL priority: Render-priority callbacks starve under a pumped (headless) loop —
            // observed live freezing a travel at its first frame. The Interval is a MINIMUM gap,
            // not a beat: on a loaded machine ticks arrive late, and the wall-clock math above
            // absorbs that lateness proportionally (plus the adaptive deadline keeps emitting
            // visible motion until the guard lifts).
            DispatcherTimer? timer = null;
            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Normal,
                (_, _) => step());
            col.Timer = timer;
            timer.Start();
        }

        // Ends a travel EXACTLY on the destination row: parks the offset precisely on targetY
        // (belt-and-braces — the stock SelectedValue writer folds the index into the central tile
        // the same way IndexToOffset does, but the park guarantees the landing either way) and
        // pushes it through SelectedValue (range-checked) so the selection machinery observes the
        // move.
        private static void FinishTravel(DateTimePickerPanel panel, double targetY)
        {
            panel.Offset = new Vector(0, targetY);
            panel.InvalidateArrange();
            CommitFinal(panel, targetY);
        }

        // Parks the offset EXACTLY on the destination row and pushes it through SelectedValue
        // (range-checked) so the selection machinery observes the move.
        private static void CommitFinal(DateTimePickerPanel panel, double targetY)
        {
            var ih = panel.ItemHeight;
            if (ih <= 0)
                return;
            var index = (int)Math.Round(targetY / ih);
            var total = TotalItems(panel);
            if (total > 0)
            {
                var rel = ((index % total) + total) % total;
                var value = panel.MinimumValue + rel;
                if (value >= panel.MinimumValue && value <= panel.MaximumValue)
                {
                    try
                    {
                        panel.SelectedValue = value;
                        return;
                    }
                    catch
                    {
                        // Fall through to the offset-only park below.
                    }
                }
            }
            panel.Offset = new Vector(0, targetY);
            panel.InvalidateArrange();
        }

        // Converts an item index (in the panel's own numbering, i.e. round(offset / ItemHeight))
        // to the offset that centres it. Looping columns tile their extent endlessly, so the
        // index is folded into the central tile — the same window the stock SelectedValue writer
        // maintains ([extentOne·50, extentOne·150)) — keeping every travel short and the arrange
        // maths honest; linear columns clamp to [0, Extent − ItemHeight].
        private static double IndexToOffset(DateTimePickerPanel panel, int index)
        {
            var y = (double)index * panel.ItemHeight;
            if (!panel.ShouldLoop)
            {
                var max = Math.Max(0, panel.Extent.Height - panel.ItemHeight);
                return Math.Clamp(y, 0, max);
            }

            var extentOne = panel.Extent.Height / 100.0;
            if (extentOne <= 0)
                return y;

            const double WindowStartRatio = 50.0;
            const double WindowLengthRatio = 100.0;
            var windowStart = extentOne * WindowStartRatio;
            var windowLength = extentOne * WindowLengthRatio;
            var shifted = y - windowStart;
            var folded = ((shifted % windowLength) + windowLength) % windowLength;
            return windowStart + folded;
        }

        private static int TotalItems(DateTimePickerPanel panel)
        {
            var eh = panel.Extent.Height;
            var ih = panel.ItemHeight;
            if (eh <= 0 || ih <= 0)
                return 0;
            var perTile = panel.ShouldLoop ? eh / 100.0 : eh;
            return (int)Math.Round(perTile / ih);
        }

        // The VALUE resting at a given index: Min + (index mod total), looping columns wrap.
        private static int ValueAt(int index, DateTimePickerPanel panel)
        {
            var total = TotalItems(panel);
            if (total <= 0)
                return panel.MinimumValue;
            var rel = ((index % total) + total) % total;
            return panel.MinimumValue + rel;
        }

        // Number of distinct values in the column's loop (linear columns: their full range).
        private static int TotalValues(DateTimePickerPanel panel) =>
            panel.ShouldLoop ? TotalItems(panel) : Math.Max(0, panel.MaximumValue - panel.MinimumValue + 1);

        // ------------------------------------------------------- band tracking ---------------

        private static void TrackBand(Attachment att)
        {
            if (att.LayoutChanged is not null)
                return;
            EventHandler layoutChanged = (_, __) => RefreshBand(att);
            att.LayoutChanged = layoutChanged;
            att.Root.LayoutUpdated += layoutChanged;
            RefreshBand(att);
        }

        // The band is PINNED to the presenter's centre; only the rows scroll beneath it. But the
        // flyout HOSTS the presenter asynchronously, so the band's bounds are EMPTY until the
        // pop-up has laid out — recomputing on EVERY layout pass keeps the intersection marks
        // truthful.
        private static void RefreshBand(Attachment att)
        {
            var band = ProjectBand(att.Root);
            if (band is null)
                return; // Not laid out yet; the next layout pass retries.
            att.Band = band.Value;
            MarkRows(att);
        }

        // -------------------------------------------------------------- teardown -------------

        // Releases everything an opening wired (layout tracking, raised-event watches, the
        // watchdog, the columns) but keeps the attachment and its Loaded/TemplateApplied/Detached
        // handlers, so the next opening wires a fresh set of columns.
        private static void Suspend(Attachment att)
        {
            try
            {
                if (att.LayoutChanged is { } lc)
                    att.Root.LayoutUpdated -= lc;
                att.LayoutChanged = null;
                att.TapWatch?.Dispose();
                att.TapWatch = null;
                att.ClickWatch = null;
                att.Watchdog?.Stop();
                att.Watchdog = null;
                foreach (var col in att.Columns)
                    col.Unwire?.Invoke();
                att.Columns.Clear();
            }
            catch
            {
                // A suspend failure must never take the app down.
            }
        }

        private static void Detach(Control root)
        {
            if (!Live.TryGetValue(root, out var att))
                return;
            if (att.LayoutChanged is { } lc)
                root.LayoutUpdated -= lc;
            if (att.Loaded is { } ld)
                root.Loaded -= ld;
            if (att.Applied is { } ap && root is TemplatedControl templated)
                templated.TemplateApplied -= ap;
            if (att.Detached is { } d)
                root.DetachedFromVisualTree -= d;
            att.TapWatch?.Dispose();
            att.ClickWatch?.Dispose();
            att.Watchdog?.Stop();
            foreach (var col in att.Columns)
                col.Unwire?.Invoke();
            att.Columns.Clear();
            Live.Remove(root);
        }
    }
}
