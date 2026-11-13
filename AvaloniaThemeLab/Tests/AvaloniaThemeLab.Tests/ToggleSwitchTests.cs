using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The switch is a face that holds a knob.</summary>
public sealed class ToggleSwitchTests
{
    private sealed record Shape(Rect Track, Rect Dot, double Border);

    private static Shape Measure(Action<DefaultTheme>? theme, bool on)
    {
        var toggle = new ToggleSwitch { IsChecked = on };
        var window = new Window { Content = new StackPanel { Children = { toggle } }, Width = 400, Height = 300 };
        TestTheme.With(theme);
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
        var off = Measure(null, false);
        Assert.Equal(new Size(40, 20), off.Track.Size);
        Assert.Equal(new Size(10, 10), off.Dot.Size);
        Assert.Equal(off.Track.X + 5, off.Dot.X, 0.5);

        var on = Measure(null, true);
        Assert.Equal(on.Track.Right - 5, on.Dot.Right, 0.5);
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void At_spacing_0_the_dot_meets_the_outline(bool on)
    {
        var s = Measure(t => { t.SpacingScale = 0; }, on);
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
    [InlineData(0.5, true)]
    public void A_low_control_height_shrinks_the_switch_and_keeps_the_dot_inside(double controlHeight, bool on)
    {
        var s = Measure(t => { t.ControlHeightScale = controlHeight; }, on);
        var standard = Measure(null, on);
        Assert.True(s.Track.Height < standard.Track.Height, $"Control height {controlHeight}: the track is {s.Track.Height:0.#}px tall, at 1 it is {standard.Track.Height:0.#}px");
        Assert.True(s.Dot.Width >= 6 - 0.01, $"The dot is {s.Dot.Width:0.#}px, under its floor");
        var inner = s.Track.Deflate(s.Border);
        Assert.True(inner.Contains(s.Dot), $"The {s.Dot.Width:0.#}px dot at {s.Dot} is not inside the track's inner edge {inner}");
        Assert.Equal(2 * s.Track.Height, s.Track.Width, 0.5);
    }

    /// <summary>Control height sizes the switch's height, and its width with it: the switch scales as one face.</summary>
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void A_higher_control_height_makes_the_switch_taller(bool on)
    {
        var standard = Measure(null, on);
        var tall = Measure(t => { t.ControlHeightScale = 2; }, on);
        Assert.Equal(standard.Track.Height + 10, tall.Track.Height, 0.5);
        Assert.Equal(2 * tall.Track.Height, tall.Track.Width, 0.5);
        Assert.Equal(20, tall.Dot.Width, 0.5);
        Assert.True(tall.Track.Deflate(tall.Border).Contains(tall.Dot), $"The dot at {tall.Dot} is outside the track {tall.Track}");
    }

    /// <summary>Spacing is the gap round the dot: less spacing shrinks the switch, more grows it, and the dot keeps its size.</summary>
    [AvaloniaTheory]
    [InlineData(0, false)]
    [InlineData(0, true)]
    [InlineData(3, false)]
    [InlineData(3, true)]
    public void Spacing_resizes_the_switch_and_never_the_dot(double spacing, bool on)
    {
        var s = Measure(t => { t.SpacingScale = spacing; }, on);
        Assert.Equal(new Size(10, 10), s.Dot.Size);
        Assert.Equal(10 + 2 * 4 * spacing + 2, s.Track.Height, 0.5);
        Assert.Equal(2 * s.Track.Height, s.Track.Width, 0.5);
        Assert.True(s.Track.Deflate(s.Border).Contains(s.Dot), $"The dot at {s.Dot} is outside the track {s.Track}");
    }

    /// <summary>The on and off label sits clear of the track at every size: it never slides under the switch.</summary>
    [AvaloniaTheory]
    [InlineData(1, 1)]
    [InlineData(0, 1)]
    [InlineData(2, 1)]
    [InlineData(1, 0)]
    [InlineData(1, 3)]
    public void The_label_sits_clear_of_the_track(double controlHeight, double spacing)
    {
        foreach (var on in new[] { false, true })
        {
            var toggle = new ToggleSwitch { IsChecked = on };
            var window = new Window { Content = new StackPanel { Children = { toggle } }, Width = 400, Height = 300 };
            TestTheme.With(t => { t.ControlHeightScale = controlHeight; t.SpacingScale = spacing; });
            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
                var outline = toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "OuterBorder");
                var label = toggle.GetVisualDescendants().OfType<Avalonia.Controls.Presenters.ContentPresenter>().Single(p => p.Name == (on ? "PART_OnContentPresenter" : "PART_OffContentPresenter"));
                var trackRight = outline.TranslatePoint(new Point(outline.Bounds.Width, 0), toggle)!.Value.X;
                var labelLeft = label.TranslatePoint(default, toggle)!.Value.X;
                Assert.True(labelLeft >= trackRight - 0.5, $"{(on ? "On" : "Off")}: the label starts at {labelLeft:0.#}, inside the track ending at {trackRight:0.#}");
            }
            finally
            {
                window.Close();
            }
        }
    }

    /// <summary>A real click on the track turns the switch on, and a second turns it off.</summary>
    [AvaloniaFact]
    public void Clicking_the_track_toggles_the_switch()
    {
        var toggle = new ToggleSwitch();
        var window = new Window { Content = new StackPanel { Children = { toggle } }, Width = 400, Height = 300 };
        TestTheme.With(null);
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Avalonia.Headless.AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();

            var outline = toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "OuterBorder");
            var centre = outline.TranslatePoint(new Point(outline.Bounds.Width / 2, outline.Bounds.Height / 2), window)!.Value;
            foreach (var expected in new[] { true, false })
            {
                window.MouseMove(centre);
                window.MouseDown(centre, Avalonia.Input.MouseButton.Left);
                window.MouseUp(centre, Avalonia.Input.MouseButton.Left);
                Dispatcher.UIThread.RunJobs();
                Assert.Equal(expected, toggle.IsChecked);
            }
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Icon size has nothing to do with the switch: it was the dot's floor, which outgrew the dot at icon size 3.</summary>
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void Icon_size_does_not_change_the_switch(bool on)
    {
        var standard = Measure(null, on);
        var bigIcons = Measure(t => { t.IconSizeScale = 3; }, on);
        Assert.Equal(standard.Track.Size, bigIcons.Track.Size);
        Assert.Equal(standard.Dot.Size, bigIcons.Dot.Size);
    }

    /// <summary>The header sits one spacing step above the track, and nothing pads the switch below it.</summary>
    [AvaloniaTheory]
    [InlineData(1)]
    [InlineData(2)]
    public void The_header_is_one_spacing_step_above_the_track(double spacing)
    {
        var toggle = new ToggleSwitch { Content = "Header" };
        var window = new Window { Content = new StackPanel { Children = { toggle } }, Width = 400, Height = 300 };
        TestTheme.With(t => { t.SpacingScale = spacing; });
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            var header = toggle.GetVisualDescendants().OfType<Avalonia.Controls.Presenters.ContentPresenter>().Single(p => p.Name == "PART_ContentPresenter");
            var outline = toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "OuterBorder");
            var headerBottom = header.TranslatePoint(new Point(0, header.Bounds.Height), toggle)!.Value.Y;
            var trackTop = outline.TranslatePoint(default, toggle)!.Value.Y;
            var trackBottom = trackTop + outline.Bounds.Height;
            Assert.Equal(8 * spacing, trackTop - headerBottom, 0.5);
            Assert.Equal(toggle.Bounds.Height - toggle.Padding.Bottom, trackBottom, 0.5);
        }
        finally
        {
            window.Close();
        }
    }
}
