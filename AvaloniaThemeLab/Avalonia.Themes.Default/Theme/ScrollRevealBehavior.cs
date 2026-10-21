using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Themes.Default;

/// <summary>
/// Marks a <see cref="ScrollViewer"/> as actively scrolling (adds the <c>scrolling</c> style
/// class) so its auto-hide bars can fade back in WHILE the content moves — wheel, trackpad,
/// thumb drag or keyboard alike — and fade out again after the stock hide delay once it settles.
/// Attached as <c>{default:ScrollReveal.Apply}</c> from the ScrollViewer template.
/// </summary>
/// <remarks>
/// <para><b>Why a behaviour:</b> the framework signals scroll activity only through
/// <see cref="ScrollViewer.ScrollChangedEvent"/> (there is no public "is scrolling" state, and
/// <see cref="RangeBase.IsExpanded"/> on the bars is non-public), so nothing pure-XAML can drive
/// the reveal. The event's <see cref="ScrollChangedEventArgs.OffsetDelta"/> covers every input
/// path at once: a wheel or trackpad delta, a thumb drag and a keyboard step all land as a
/// non-zero offset delta.</para>
/// <para>The class is armed on every positional change and disarmed by a <see cref="DispatcherTimer"/>
/// running the STOCK HIDE DELAY (one second of quiet, the same grace the stock auto-hide bars
/// give before collapsing). A fresh change restarts the clock, so continuous scrolling keeps the
/// bars revealed. Headless captures freeze clocks, so the proof is structural: the subscribers
/// installed and the class riding the viewer after a crawled wheel action.</para>
/// <para>The attached property carries a BARE name ("Apply"): Avalonia 12 rejects a dotted
/// registration name ('name' may not contain periods), and the owner type supplies uniqueness.</para>
/// </remarks>
public sealed class ScrollReveal
{
    /// <summary>Attaches the scroll-reveal marking to a control hosted by a <see cref="ScrollViewer"/>.</summary>
    public static readonly AttachedProperty<bool> ApplyProperty =
        AvaloniaProperty.RegisterAttached<ScrollReveal, Control, bool>("Apply");

    // The style class the ScrollViewer theme reads: "^.[ScrollingClass]" reveals the auto-hide bars.
    private const string ScrollingClass = "scrolling";

    // The stock hide delay: one second of quiet before the bars fade out again. Kept local: the
    // engine exposes no motion-duration tokens beyond the UI-transition trio, and this drives a
    // behaviour, not a style.
    private static readonly TimeSpan HideDelay = TimeSpan.FromSeconds(1);

    private sealed class Attachment
    {
        public required Control Root { get; init; }
        public EventHandler<RoutedEventArgs>? Loaded;
        public EventHandler<RoutedEventArgs>? Unloaded;
        public EventHandler<ScrollChangedEventArgs>? ScrollChanged;
        public DispatcherTimer? Idle;
        public ScrollViewer? Viewer;
    }

    // One live attachment per host control: the weak key keeps a detached viewer from pinning its
    // subscriptions (mirrors WheelSnap.Live).
    private static readonly Dictionary<WeakReference<Control>, Attachment> Live = [];

    private ScrollReveal()
    {
    }

    public static void SetApply(Control target, bool value) => target.SetValue(ApplyProperty, value);

    public static bool GetApply(Control target) => target.GetValue(ApplyProperty);

    private static void OnApplyPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not Control control || e.Property != ApplyProperty)
            return;
        try
        {
            if ((bool)e.NewValue!)
                Attach(control);
            else
                Detach(control);
        }
        catch
        {
            // Wiring trouble must never propagate into the shared application.
        }
    }

    private static void Attach(Control root)
    {
        PruneDead();
        foreach (var kvp in Live)
        {
            if (kvp.Key.TryGetTarget(out var t) && ReferenceEquals(t, root))
                return;
        }

        var attachment = new Attachment { Root = root };
        attachment.Loaded = (_, _) => SafeWire(root, attachment);
        attachment.Unloaded = (_, _) => SafeUnwire(attachment);
        root.Loaded += attachment.Loaded;
        root.Unloaded += attachment.Unloaded;
        Live[new WeakReference<Control>(root)] = attachment;
    }

    private static void Detach(Control root)
    {
        WeakReference<Control>? doomedKey = null;
        foreach (var kvp in Live)
        {
            if (kvp.Key.TryGetTarget(out var t) && ReferenceEquals(t, root))
                doomedKey = kvp.Key;
        }
        if (doomedKey is null || !Live.TryGetValue(doomedKey, out var attachment))
            return;

        if (attachment.Loaded is not null)
            root.Loaded -= attachment.Loaded;
        if (attachment.Unloaded is not null)
            root.Unloaded -= attachment.Unloaded;
        Unwire(attachment);
        Live.Remove(doomedKey);
    }

    private static void PruneDead()
    {
        foreach (var key in Live.Keys.Where(k => !k.TryGetTarget(out _)).ToList())
            Live.Remove(key);
    }

    private static void SafeWire(Control root, Attachment attachment)
    {
        try
        {
            Wire(root, attachment);
        }
        catch
        {
            // Discovery trouble leaves the viewer stock-behaving rather than broken.
        }
    }

    private static void SafeUnwire(Attachment attachment)
    {
        try
        {
            Unwire(attachment);
        }
        catch
        {
        }
    }

    private static void Wire(Control root, Attachment attachment)
    {
        if (attachment.Viewer is not null)
            return;

        // The host sits INSIDE the viewer's template (the content presenter), so climb to the
        // enclosing ScrollViewer rather than assuming a fixed relationship.
        for (var v = root.GetVisualParent(); v is not null; v = v.GetVisualParent())
        {
            if (v is ScrollViewer viewer)
            {
                attachment.Viewer = viewer;
                break;
            }
        }
        if (attachment.Viewer is null)
            return;

        // Every POSITIONAL change — wheel, trackpad momentum, thumb drag, keyboard — arms the
        // reveal and restarts the hide-delay clock. Extent/viewport-only updates (resize, layout)
        // carry a zero offset delta and leave the bars resting.
        attachment.ScrollChanged = (_, args) =>
        {
            if (args.OffsetDelta == default)
                return;
            MarkActive(attachment);
        };
        attachment.Idle = new DispatcherTimer { Interval = HideDelay };
        attachment.Idle.Tick += (_, _) => Disarm(attachment);
        attachment.Viewer.ScrollChanged += attachment.ScrollChanged;
    }

    private static void Unwire(Attachment attachment)
    {
        if (attachment.Viewer is not null && attachment.ScrollChanged is not null)
            attachment.Viewer.ScrollChanged -= attachment.ScrollChanged;
        attachment.Idle?.Stop();
        attachment.Viewer = null;
        attachment.ScrollChanged = null;
        attachment.Idle = null;
    }

    private static void MarkActive(Attachment attachment)
    {
        Guard(() =>
        {
            var viewer = attachment.Viewer;
            if (viewer is null || !viewer.IsVisible)
                return;
            viewer.Classes.Add(ScrollingClass);
            attachment.Idle!.Stop();
            attachment.Idle.Start();
        });
    }

    private static void Disarm(Attachment attachment)
    {
        Guard(() =>
        {
            attachment.Idle?.Stop();
            attachment.Viewer?.Classes.Remove(ScrollingClass);
        });
    }

    private static void Guard(Action action)
    {
        try
        {
            action();
        }
        catch
        {
            // Any surprise degrades to the stock behaviour rather than taking the viewer down.
        }
    }
}
