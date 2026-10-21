using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

// Lives in Avalonia.Themes.Default (NOT ...Default.Theme): the {default:PickerPopupAlignment.Apply}
// spelling resolves against clr-namespace:Avalonia.Themes.Default, where the default theme registers
// its attached properties.
namespace Avalonia.Themes.Default
{
    /// <summary>
    /// OPENS THE DATE/TIME PICKER POPUP SO ITS CENTRE (SELECTED) ROW LINES UP EXACTLY WITH THE
    /// CLOSED BOX (task 143): the pointer is already over the box's content when the user clicks,
    /// so the value they are looking at should stay under the pointer. Both pickers share ONE
    /// implementation — whichever control carries the flag gets the treatment.
    /// </summary>
    /// <remarks>
    /// HOW IT PLACES. The stock picker opens its popup with Placement=Bottom / Anchor=Bottom /
    /// Gravity=Bottom / ConstraintAdjustment=SlideY plus tunable Horizontal/VerticalOffset. In that
    /// mode the UNCONSTRAINED popup top-left is (boxCentreX − popupW/2 + HOffset, boxBottom + VOffset).
    /// This behaviour watches the popup's IsOpen and, once open, computes the offsets that put the
    /// DESIRED popup corner — centred on the box so the centre row sits dead-on the box's centre —
    /// into Horizontal/VerticalOffset. EXCEPTION (also task 143): if centring would push the popup
    /// off-screen, STAYING FULLY VISIBLE WINS — the desired corner is clamped into the parent
    /// top-level's client area first, and the stock SlideY adjustment backs any residual overshoot
    /// up on-screen.
    ///
    /// WHY OFFSETS RATHER THAN A CUSTOM RECTANGLE. PlacementMode.Custom hands the positioner a raw
    /// rectangle, but the managed positioner in this framework version misinterprets rectangles the
    /// callback supplies (measured live: whatever rectangle the callback returned, the popup landed
    /// with its bottom-right at the supplied top-left). Feeding the stock mode its own vocabulary
    /// (anchors + offsets) sidesteps that entirely and keeps the native constraint handling working.
    ///
    /// WHY WATCH ISOPEN AND WRITE AFTERWARDS. The stock picker's OnApplyTemplate RESETS the popup's
    /// placement properties after template application, wiping anything installed once. Watching
    /// IsOpen and writing the offsets on the NEXT dispatcher cycle (after the stock reset and the
    /// first configure have both run) beats that deterministically.
    /// </remarks>
    // Deliberately NOT static: 12.x refuses a static class as the generic owner argument of
    // RegisterAttached. All members are static anyway.
    public sealed partial class PickerPopupAlignment
    {
        private sealed class Attachment
        {
            public Control Root { get; init; } = null!;
            public EventHandler<RoutedEventArgs>? Loaded { get; set; }
            public EventHandler<TemplateAppliedEventArgs>? Applied { get; set; }
            // The popup we own (found once parts exist) plus its watchers.
            public Popup? OwnedPopup { get; set; }
            public IDisposable? OpenWatch { get; set; }
            public EventHandler? LayoutWatchAction { get; set; }
            // Last alignment intent, remembered so the calibration pass can measure the residual.
            public Point LastDesired { get; set; }
            public Visual? LastTopLevel { get; set; }
        }

#pragma warning disable IDE0028, IDE0090 // The repo's analyzers reject both spellings; keep the explicit form.
        private static readonly ConditionalWeakTable<Control, Attachment> Live = new();
#pragma warning restore IDE0028, IDE0090

        // --------------------------------------------------------------------------- helpers

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

        private static bool Dbg => Environment.GetEnvironmentVariable("THEME_LAB_DEBUG_POPUP") != null;

        private static void LogDbg(string msg)
        {
            if (Dbg)
                Console.Error.WriteLine(msg);
        }

        // --------------------------------------------------------------- the attached flag

        // Registered under the bare name "Apply": the XAML spelling {default:PickerPopupAlignment.Apply}
        // resolves as CLASS PickerPopupAlignment + PROPERTY Apply, and Avalonia forbids a period
        // inside the registered name.
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
        // compiled XAML applies {default:PickerPopupAlignment.Apply} via SetValue, which FIRES THE
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
                }
                catch
                {
                    // A flag-watch failure must never take the app down.
                }
            }
        }

        static PickerPopupAlignment()
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
            if (Live.TryGetValue(root, out var existing))
            {
                TryWire(existing);
                return;
            }

            var att = new Attachment { Root = root };
            att.Loaded = (_, _) => TryWire(att);
            att.Applied = (_, _) => TryWire(att);
            root.AddHandler(Control.LoadedEvent, att.Loaded);
            root.AddHandler(TemplatedControl.TemplateAppliedEvent, att.Applied);
            root.Unloaded += (_, _) => Unwire(att);
            Live.Add(root, att);
            TryWire(att);
        }

        private static void Unwire(Attachment att)
        {
            Teardown(att);
            var p = att.Root;
            p.RemoveHandler(Control.LoadedEvent, att.Loaded);
            p.RemoveHandler(TemplatedControl.TemplateAppliedEvent, att.Applied);
            Live.Remove(p);
        }

        private static void Teardown(Attachment att)
        {
            att.OpenWatch?.Dispose();
            att.OpenWatch = null;
            if (att.LayoutWatchAction != null && att.OwnedPopup != null)
                TopLevel.GetTopLevel(att.OwnedPopup)?.LayoutUpdated -= att.LayoutWatchAction;
            att.LayoutWatchAction = null;
            att.OwnedPopup = null;
        }

        // Finds the popup UNDER the flagged root (the flag rides LayoutRoot, a sibling of the
        // popup, because the Popup itself never raises Loaded) and wires the installer onto it.
        private static void TryWire(Attachment att)
        {
            var popup = FindPopupUnder(att.Root);
            if (popup == null)
                return;
            Wire(popup, att);
        }

        private static Popup? FindPopupUnder(Control root)
        {
            foreach (var v in DescendantsOf(root))
            {
                if (v is Popup p)
                    return p;
            }

            return null;
        }

        private static void Wire(Popup popup, Attachment att)
        {
            if (att.OwnedPopup == popup && att.OpenWatch != null)
                return;

            Teardown(att);
            att.OwnedPopup = popup;
            // Watch IsOpen (GLOBAL observable, filtered to THIS popup): align on EVERY open,
            // because the stock picker's OnApplyTemplate resets the popup's placement properties.
            var watch = new OpenWatcher(popup, () => ScheduleAlign(popup, att));
            watch.Subscription = Popup.IsOpenProperty.Changed.Subscribe(watch);
            att.OpenWatch = watch;
            // Re-align on every layout pass while open: navigating/reflowing the PAGE moves the
            // closed box after our initial pass, and the popup must follow it.
            var tl = TopLevel.GetTopLevel(popup);
            if (tl != null)
            {
                EventHandler eh = (_, _) =>
                {
                    if (popup.IsOpen)
                        ScheduleAlign(popup, att);
                };
                att.LayoutWatchAction = eh;
                tl.LayoutUpdated += eh;
            }
            if (popup.IsOpen)
                ScheduleAlign(popup, att);
        }

        // Filters the global IsOpen observable down to one popup's openings.
        private sealed class OpenWatcher : IObserver<AvaloniaPropertyChangedEventArgs<bool>>, IDisposable
        {
            private readonly Popup _popup;
            private readonly Action _align;
            public IDisposable? Subscription { get; set; }
            public OpenWatcher(Popup popup, Action align)
            {
                _popup = popup;
                _align = align;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(AvaloniaPropertyChangedEventArgs<bool> value)
            {
                if (ReferenceEquals(value.Sender, _popup))
                {
                    LogDbg($"[PPA] watcher: isOpen={value.NewValue.GetValueOrDefault()}");
                    if (value.NewValue.GetValueOrDefault())
                        _align();
                }
            }

            public void Dispose()
            {
                Subscription?.Dispose();
                Subscription = null;
            }
        }

        // Runs the alignment a few dispatcher cycles AFTER the open: by then the stock
        // OnApplyTemplate reset and the positioner's first ConfigurePosition have both happened,
        // so our offset writes stick. Retries while the popup content has no measured size yet
        // (freshly opened templates measure asynchronously).
        private static void ScheduleAlign(Popup popup, Attachment att, int attempt = 0)
        {
            LogDbg($"[PPA] scheduling align (attempt {attempt})");
            try
            {
                // Normal (not Background): the stock opener may run ExecuteInitialLayoutPass
                // SYNCHRONOUSLY right after IsOpen=true; a Background job can lose the race to the
                // NEXT layout pass and its offsets then never reach the renderer.
                Dispatcher.UIThread.Post(() =>
                {
                    var ok = AlignOnce(popup, att);
                    if (!ok && popup.IsOpen && attempt < 10)
                    {
                        // A popup hosted OUTSIDE the window's own tree can never be aligned by
                        // window-client offsets, so do not keep retrying it: the stock placement is
                        // the best we can offer for such a popup.
                        if (!popup.IsUsingOverlayLayer)
                        {
                            LogDbg("[PPA] giving up on this popup: it is not hosted in the window's overlay layer");
                            return;
                        }
                        ScheduleAlign(popup, att, attempt + 1);
                    }
                    else if (ok && popup.IsOpen)
                        // Measurement-driven correction: the managed positioner folds in shifts we
                        // cannot predict (SlideY/host chrome), so nudge against where the popup
                        // ACTUALLY landed; the affine map converges in one or two rounds.
                        Dispatcher.UIThread.Post(() => Calibrate(popup, att));
                });
            }
            catch
            {
                // Never take the app down over placement cosmetics.
            }
        }

        // Feedback pass: measures where the popup ACTUALLY sits (live Bounds walk — the cached
        // TransformToVisual matrix lags a layout pass in headless pumps) and nudges the offsets by
        // the residual. Prediction alone undershoots because the managed positioner folds in shifts
        // we cannot see (SlideY/host chrome); the offset→landing map is affine, so one measured
        // nudge converges. Iterates (bounded) until the residual is negligible.
        private static void Calibrate(Popup popup, Attachment att, int round = 0)
        {
            try
            {
                var child = popup.Child as Visual;
                var tl = att.LastTopLevel ?? TopLevel.GetTopLevel(popup);
                if (child == null || tl == null || !popup.IsOpen)
                    return;

                // THE CONTENT MUST LIVE IN THE SAME TOP LEVEL AS THE BOX. The offsets this
                // behaviour writes are in WINDOW-CLIENT units, so they only mean anything when the
                // popup is hosted INSIDE the window's own visual tree (the overlay host). In a
                // SEPARATE NATIVE POPUP WINDOW the content's top level is a different object, the
                // live walk below cannot reach it and returns (0,0) — a BOGUS "actual" position.
                // The loop then adds the whole desired position to the offset, over and over
                // (measured live on macOS: 818 -> 4148 -> 8180), walking the popup thousands of
                // pixels off-screen: the picker appeared to do nothing at all. Refuse to calibrate
                // across a host boundary rather than chase a position that does not exist.
                var childTopLevel = TopLevel.GetTopLevel(child);
                if (childTopLevel is not null && !ReferenceEquals(childTopLevel, tl))
                {
                    LogDbg($"[PPA] calibrate skipped: the popup content lives in another top level ({childTopLevel.GetType().Name}), not the window's");
                    return;
                }

                var actual = WalkRenderOffset(child, tl);
                var rx = att.LastDesired.X - actual.X;
                var ry = att.LastDesired.Y - actual.Y;
                LogDbg($"[PPA] calibrate r{round}: desired=({att.LastDesired.X:F1},{att.LastDesired.Y:F1}) actual=({actual.X:F1},{actual.Y:F1}) residual=({rx:F1},{ry:F1})");
                if (Math.Abs(rx) <= 0.5 && Math.Abs(ry) <= 0.5)
                    return; // Converged.
                if (round >= 5)
                    return; // Bounded: stop chasing rather than oscillate.
                popup.HorizontalOffset += rx;
                popup.VerticalOffset += ry;
                Dispatcher.UIThread.Post(() => Calibrate(popup, att, round + 1));
            }
            catch
            {
                // Never take the app down over placement cosmetics.
            }
        }

        // Returns true when the alignment was applied (or nothing needed applying); false when it
        // bailed on missing data (caller may retry).
        private static bool AlignOnce(Popup popup, Attachment att)
        {
            LogDbg($"[PPA] align-once entered: open={popup.IsOpen} loaded={popup.IsLoaded}");
            try
            {
                if (!popup.IsOpen || !popup.IsLoaded)
                {
                    LogDbg($"[PPA] bail: isOpen={popup.IsOpen} isLoaded={popup.IsLoaded}");
                    return false;
                }

                var target = popup.PlacementTarget;
                // The popup's own DesiredSize reads 0x0 here; the CONTENT's bounds are the real size.
                var ps = popup.Child?.Bounds.Size ?? default;
                if (target == null || ps.Width <= 0 || ps.Height <= 0)
                {
                    LogDbg($"[PPA] bail: target={(target?.GetType().Name)} size={ps}");
                    return false;
                }

                // THE OFFSETS BELOW ARE IN WINDOW-CLIENT UNITS, so they only apply when the popup
                // is hosted INSIDE the window's own visual tree (the overlay host). A popup in a
                // SEPARATE NATIVE WINDOW interprets the same numbers in screen space, which walks
                // it thousands of pixels off-screen — measured live on macOS: the picker opened
                // ~8000px away and the user saw nothing. Leave such a popup on the stock
                // placement rather than misplacing it; the theme's own templates ask for the
                // overlay host, so this only guards a hand-written template.
                if (!popup.IsUsingOverlayLayer)
                {
                    LogDbg("[PPA] align skipped: the popup is not hosted in the window's overlay layer, so window-client offsets do not apply to it");
                    return false;
                }

                // Centre-row offset INSIDE the popup content, measured in the content's own
                // coordinate space (immune to the popup's absolute-position staleness).
                var band = FindBand(popup.Child);
                var bandCentreInContent = BandCentreInContent(band, popup.Child);

                // Box centre in TOP-LEVEL coordinates. NOTE: target.TransformToVisual(topLevel)
                // returns a CACHED matrix that lags a layout pass in headless pumps (measured: a
                // second open read the PREVIOUS picker's origin), so the origin is walked up the
                // visual tree through live RenderBounds instead.
                var tb = target.Bounds;
                var tl = TopLevel.GetTopLevel(popup);
                var tOrigin = WalkRenderOffset(target, tl);
                var boxCentre = new Point(tOrigin.X + tb.Width / 2, tOrigin.Y + tb.Height / 2);

                // Desired popup top-left: centred on the box, clamped into the work area.
                var clientSize = ParentClientSize(popup);
                var desired = ComputePlacement(boxCentre, ps, clientSize, bandCentreInContent);

                // Stock-mode base position (unconstrained): top-left =
                // (boxCentreX − popupW/2, boxBottom) + offsets. Solve for the offsets.
                var ox = desired.X - (tOrigin.X + tb.Width / 2 - ps.Width / 2);
                var oy = desired.Y - (tOrigin.Y + tb.Height);

                att.LastDesired = new Point(desired.X, desired.Y);
                att.LastTopLevel = tl;
                var wrote = Math.Abs(popup.HorizontalOffset - ox) > 0.01 || Math.Abs(popup.VerticalOffset - oy) > 0.01;
                if (wrote)
                {
                    popup.HorizontalOffset = ox;
                    popup.VerticalOffset = oy;
                }
                LogDbg($"[PPA] align: id={System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(popup)} desired=({desired.X:F1},{desired.Y:F1}) base=({tOrigin.X + tb.Width / 2 - ps.Width / 2:F1},{tOrigin.Y + tb.Height:F1}) " +
                       $"offsets=({ox:F1},{oy:F1}) wrote={wrote} after=({popup.HorizontalOffset:F1},{popup.VerticalOffset:F1}) " +
                       $"margin={popup.Margin}");
                return true;
            }
            catch
            {
                // Never take the app down over placement cosmetics.
                return false;
            }
        }

        // Origin of <paramref name="start"/> in <paramref name="root"/>'s coordinates, summed from
        // live Bounds (recomputed every arrange pass). Used instead of TransformToVisual, whose
        // accumulated matrix is CACHED and lags a layout pass in headless pumps (measured: a
        // second open read the previous picker's origin through it).
        private static Point WalkRenderOffset(Visual start, Visual? root)
        {
            var pt = new Point(start.Bounds.X, start.Bounds.Y);
            var v = start.GetVisualParent();
            while (v != null && !ReferenceEquals(v, root))
            {
                pt = new Point(pt.X + v.Bounds.X, pt.Y + v.Bounds.Y);
                v = v.GetVisualParent();
            }

            return pt;
        }

        // The centre band (#HighlightRect) inside the popup content.
        private static Border? FindBand(Control? content)
        {
            if (content == null)
                return null;
            foreach (var v in DescendantsOf(content))
            {
                if (v is Border b && b.Name == "HighlightRect")
                    return b;
            }

            return null;
        }

        // The band's centre expressed in the popup CONTENT's coordinate space, via a LIVE bounds
        // walk (the cached TransformToVisual matrix lags a layout pass — measured off by 80px on
        // a freshly opened popup). Falls back to the geometric middle of the card when the band
        // cannot be found yet.
        private static Point BandCentreInContent(Border? band, Control? content)
        {
            if (band != null && content != null)
            {
                var origin = WalkRenderOffset(band, content);
                var r = band.Bounds;
                return new Point(origin.X + r.Width / 2, origin.Y + r.Height / 2);
            }

            var cs = content?.Bounds.Size ?? default;
            return new Point(cs.Width / 2, cs.Height / 2);
        }

        private static Size ParentClientSize(Popup popup)
        {
            var tl = TopLevel.GetTopLevel(popup);
            if (tl != null && tl.ClientSize.Width > 0)
                return tl.ClientSize;
            return new Size(1200, 800);
        }

        // --------------------------------------------------------------------- pure geometry

        /// <summary>
        /// Pure placement maths (harness-tested): the popup's desired TOP-LEFT so its centre row
        /// (at <paramref name="bandCentreInContent"/>, in content coordinates) coincides with the
        /// box's centre (<paramref name="boxCentre"/>, in window coordinates), CLAMPED so the
        /// popup stays fully inside the work area (<paramref name="clientSize"/>) — staying
        /// fully visible wins over perfect alignment.
        /// </summary>
        public static Rect ComputePlacement(Point boxCentre, Size popupSize, Size clientSize, Point bandCentreInContent)
        {
            var x = boxCentre.X - bandCentreInContent.X;
            var y = boxCentre.Y - bandCentreInContent.Y;

            // Clamp: the popup must stay fully on-screen (staying visible wins over alignment).
            if (popupSize.Width >= clientSize.Width)
                x = 0;
            else
                x = Math.Clamp(x, 0, clientSize.Width - popupSize.Width);
            if (popupSize.Height >= clientSize.Height)
                y = 0;
            else
                y = Math.Clamp(y, 0, clientSize.Height - popupSize.Height);

            return new Rect(x, y, popupSize.Width, popupSize.Height);
        }
    }
}
