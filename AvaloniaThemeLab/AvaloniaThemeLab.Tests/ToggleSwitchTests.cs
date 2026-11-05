using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless.XUnit;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// The switch is a face that holds a knob. The knob's dot is a glyph (IconSize), the gap round it is spacing, and
/// control height is only the track's floor: spacing 0 closes the gap so the dot meets the outline, and a low control
/// height never squeezes the track below the knob it holds.
/// </summary>
public sealed class ToggleSwitchTests
{
    private sealed record Shape(Rect Track, Rect Dot, double Border);

    private static Shape Measure(DefaultTheme theme, bool on)
    {
        var toggle = new ToggleSwitch { IsChecked = on };
        var window = new Window { Content = new StackPanel { Children = { toggle } }, Width = 400, Height = 300 };
        ThemeRegion.SetTheme(window, theme);
        try
        {
            window.Show();
            for (var i = 0; i < 3; i++)
            {
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
            }

            var outline = toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "OuterBorder");
            var dot = toggle.GetVisualDescendants().OfType<Ellipse>().Single(e => e.Name == (on ? "SwitchKnobOn" : "SwitchKnobOff"));
            var track = new Rect(outline.TranslatePoint(default, toggle)!.Value, outline.Bounds.Size);
            var dotRect = new Rect(dot.TranslatePoint(default, toggle)!.Value, dot.Bounds.Size);
            return new Shape(track, dotRect, outline.BorderThickness.Left);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void The_default_switch_keeps_its_shape()
    {
        var off = Measure(new DefaultTheme(), false);
        Assert.Equal(new Size(40, 20), off.Track.Size);
        Assert.Equal(new Size(10, 10), off.Dot.Size);
        Assert.Equal(off.Track.X + 5, off.Dot.X, 0.5);

        var on = Measure(new DefaultTheme(), true);
        Assert.Equal(on.Track.Right - 5, on.Dot.Right, 0.5);
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void At_spacing_0_the_dot_meets_the_outline(bool on)
    {
        var s = Measure(new DefaultTheme { SpacingScale = 0 }, on);
        var inner = s.Track.Deflate(s.Border);
        Assert.True(Math.Abs(s.Dot.Top - inner.Top) < 0.5 && Math.Abs(s.Dot.Bottom - inner.Bottom) < 0.5,
            $"The dot spans {s.Dot.Top:0.#}-{s.Dot.Bottom:0.#} inside an outline whose inner edge is {inner.Top:0.#}-{inner.Bottom:0.#}");
        var side = on ? Math.Abs(s.Dot.Right - inner.Right) : Math.Abs(s.Dot.Left - inner.Left);
        Assert.True(side < 0.5, $"The dot sits {side:0.#}px from the outline's inner edge at its end");
    }

    [AvaloniaTheory]
    [InlineData(0, false)]
    [InlineData(0, true)]
    [InlineData(0.2, false)]
    [InlineData(0.4, true)]
    public void A_low_control_height_keeps_the_knob_whole_inside_the_track(double controlHeight, bool on)
    {
        var s = Measure(new DefaultTheme { ControlHeightScale = controlHeight }, on);
        Assert.Equal(new Size(10, 10), s.Dot.Size);
        var inner = s.Track.Deflate(s.Border);
        Assert.True(inner.Contains(s.Dot), $"The {s.Dot.Width:0.#}px dot at {s.Dot} is not inside the track's inner edge {inner}");
        Assert.True(s.Track.Width >= 2 * s.Track.Height - 0.5, $"The track is {s.Track.Width:0.#}x{s.Track.Height:0.#}: the knob has no room to travel");
    }
}
