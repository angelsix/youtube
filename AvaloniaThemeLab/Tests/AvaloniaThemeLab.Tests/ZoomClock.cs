using System.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// A _clock the test steps by hand. Avalonia's own clocks are wall-_clock stopwatches, so two
/// layouts shot a moment apart sample looping animations at different phases and catch
/// one-shot entrances mid-flight; pulsing one _clock to the same elapsed time before each
/// shot makes the comparison deterministic.
/// </summary>
internal sealed class ZoomClock
{
    // Every looping animation in the theme runs 2s or 3.2s; 16s is a whole number of both,
    // and is past every one-shot entrance, so each step lands on the same resting phase.
    private const double StepMs = 16000;

    private readonly object _clock;
    private readonly MethodInfo _pulse;
    private double _now;
    private readonly AvaloniaProperty _clockProperty;

    public ZoomClock()
    {
        var animatable = typeof(Animatable);
        _clockProperty = (AvaloniaProperty)animatable
            .GetField("ClockProperty", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        var mediaContextType = animatable.Assembly.GetType("Avalonia.Media.MediaContext", throwOnError: true)!;
        var mediaContext = mediaContextType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
        var clockType = mediaContextType.GetNestedType("MediaContextClock", BindingFlags.NonPublic)!;
        _clock = Activator.CreateInstance(clockType, mediaContext)!;
        _pulse = clockType.GetMethod("Pulse", BindingFlags.Public | BindingFlags.Instance)!;
    }

    /// <summary>Attaches before the window is shown, so every descendant inherits the _clock. The first _pulse is time zero.</summary>
    public void Attach(Window window)
    {
        window.SetValue(_clockProperty, _clock);
        Pulse(0);
    }

    /// <summary>Steps to the next whole phase, so looping animations rest where every shot agrees.</summary>
    public void Advance()
    {
        _now += StepMs;
        Pulse(_now);
    }

    /// <summary>
    /// Inheritance alone is not enough: a transform animator reads its control's _clock when the
    /// style attaches, and a render transform is not in the visual tree at all. Every visual and
    /// every render transform gets the _clock as a local value.
    /// </summary>
    public void AttachEverywhere(Window window)
    {
        foreach (var visual in window.GetVisualDescendants().Prepend(window))
        {
            visual.SetValue(_clockProperty, _clock);
            switch (visual.RenderTransform)
            {
                case TransformGroup group:
                    foreach (var child in group.Children)
                        child.SetValue(_clockProperty, _clock);
                    break;
                case Animatable transform:
                    transform.SetValue(_clockProperty, _clock);
                    break;
            }
        }
    }

    private void Pulse(double ms) => _pulse.Invoke(_clock, [TimeSpan.FromMilliseconds(ms)]);
}