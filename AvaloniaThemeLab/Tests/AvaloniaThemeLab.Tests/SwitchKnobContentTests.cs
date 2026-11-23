using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The switch's knob carries a glyph a consumer sets from outside, without retemplating the control.</summary>
public sealed class SwitchKnobContentTests
{
    private static (ToggleSwitch Toggle, Window Window) Show(ToggleSwitch toggle, Styles? styles = null)
    {
        var window = new Window { Content = new StackPanel { Children = { toggle } }, Width = 400, Height = 300 };
        if (styles is not null)
            window.Styles.Add(styles);
        TestTheme.With(null);
        window.Show();
        for (var i = 0; i < 3; i++)
        {
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
        }

        return (toggle, window);
    }

    private static ContentPresenter KnobContent(ToggleSwitch toggle) =>
        toggle.GetVisualDescendants().OfType<ContentPresenter>().Single(p => p.Name == "PART_KnobContent");

    /// <summary>The hook exists on a real switch, and reading it back returns what was set.</summary>
    [AvaloniaFact]
    public void A_consumer_can_set_the_knobs_content()
    {
        var toggle = new ToggleSwitch();
        Assert.Null(SwitchKnob.GetContent(toggle));

        SwitchKnob.SetContent(toggle, "sun");
        Assert.Equal("sun", SwitchKnob.GetContent(toggle));
    }

    /// <summary>Setting the hook puts the content inside the knob, centred on the dot it shares the cell with.</summary>
    [AvaloniaFact]
    public void The_knobs_content_renders_centred_on_the_dot()
    {
        var toggle = new ToggleSwitch();
        var (shown, window) = Show(toggle);
        try
        {
            Assert.Null(KnobContent(shown).Content);

            SwitchKnob.SetContent(shown, "sun");
            for (var i = 0; i < 3; i++)
            {
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
            }

            var presenter = KnobContent(shown);
            Assert.Equal("sun", presenter.Content);
            Assert.True(presenter.Bounds.Width > 0, "The knob content presenter laid out at zero width, so the glyph is invisible");

            var dot = shown.GetVisualDescendants().OfType<Ellipse>().Single(e => e.Name == "SwitchKnobOff");
            var centre = new Point(presenter.Bounds.Width / 2, presenter.Bounds.Height / 2);
            var dotCentre = dot.TranslatePoint(new Point(dot.Bounds.Width / 2, dot.Bounds.Height / 2), presenter)!.Value;
            Assert.True(Math.Abs(centre.X - dotCentre.X) < 1 && Math.Abs(centre.Y - dotCentre.Y) < 1,
                $"The glyph sits at {centre}, its dot's centre is {dotCentre}");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>An unset hook leaves the knob pixel-equivalent: same dot geometry either way.</summary>
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void An_unset_hook_leaves_the_knob_unchanged(bool on)
    {
        var plain = new ToggleSwitch { IsChecked = on };
        var (shown, window) = Show(plain);
        try
        {
            var dot = shown.GetVisualDescendants().OfType<Ellipse>().Single(e => e.Name == (on ? "SwitchKnobOn" : "SwitchKnobOff"));
            Assert.Equal(new Size(10, 10), dot.Bounds.Size);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>The content swaps between checked and unchecked from plain styles a page can declare.</summary>
    [AvaloniaFact]
    public void Page_styles_swap_the_glyph_with_the_checked_state()
    {
        var styles = new Styles
        {
            new Style(x => x.OfType<ToggleSwitch>().Class(":unchecked"))
            {
                Setters = { new Setter(SwitchKnob.ContentProperty, "sun") },
            },
            new Style(x => x.OfType<ToggleSwitch>().Class(":checked"))
            {
                Setters = { new Setter(SwitchKnob.ContentProperty, "moon") },
            },
        };

        var toggle = new ToggleSwitch();
        var (shown, window) = Show(toggle, styles);
        try
        {
            Assert.Equal("sun", KnobContent(shown).Content);

            shown.IsChecked = true;
            for (var i = 0; i < 3; i++)
            {
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
            }

            Assert.Equal("moon", KnobContent(shown).Content);
        }
        finally
        {
            window.Close();
        }
    }
}
