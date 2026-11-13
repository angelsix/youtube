using AngelSix.ThemeEngine;
using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using AngelSix.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The colour entries' samples share one colour per pane, seeded from the selected accent, so the controls follow each other.</summary>
public sealed class ColorSampleTests
{
    private static readonly string[] colourEntries = ["ColorPicker", "ColorPreviewer", "ColorSlider", "ColorSpectrum", "ColorView"];

    public static TheoryData<string> Entries() => [.. colourEntries];

    [AvaloniaTheory]
    [MemberData(nameof(Entries))]
    public void Every_sample_starts_at_the_selected_accent(string entry)
    {
        var (theme, lab) = Lab();
        var accent = lab.SelectedAccent;
        lab.SelectedAccent = MainWindowViewModel.Hue(AccentKind.Warning);
        var window = Show(lab, entry);
        try
        {
            var light = Samples(window, dark: false);
            Assert.NotEmpty(light);
            Assert.All(light, sample => Assert.Equal(theme.AccentWarning, ColorOf(sample)));

            // The dark pane seeds from its own palette's Warning, the same for every sample in it
            var dark = Samples(window, dark: true);
            Assert.NotEmpty(dark);
            Assert.Single(dark.Select(ColorOf).Distinct());
        }
        finally
        {
            lab.SelectedAccent = accent;
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Dragging_the_saturation_strip_moves_every_slider_and_keeps_the_hue()
    {
        var (_, lab) = Lab();
        var window = Show(lab, "ColorSlider");
        try
        {
            var samples = Samples(window, dark: false);
            var hueBefore = HsvOf(samples[0]).H;
            var saturation = samples.OfType<ColorSlider>().First(slider => slider.ColorComponent == ColorComponent.Component2 && slider.IsEnabled);
            Drag(window, saturation, from: 0.9, to: 0);

            Assert.Equal(0, HsvOf(saturation).S, 2);
            Assert.All(samples, sample => Assert.Equal(HsvOf(saturation), HsvOf(sample)));
            Assert.Equal(hueBefore, HsvOf(saturation).H, 2);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Dragging_the_spectrum_moves_every_spectrum()
    {
        AssertDragLinks("ColorSpectrum", samples => samples.OfType<ColorSpectrum>().First(spectrum => spectrum.IsEnabled));
    }

    [AvaloniaFact]
    public void Dragging_an_editor_moves_every_editor()
    {
        AssertDragLinks("ColorView", samples => samples.OfType<ColorView>().First(view => view.IsEnabled)
            .GetVisualDescendants().OfType<ColorSpectrum>().First());
    }

    [AvaloniaFact]
    public void Picking_a_shade_moves_every_previewer()
    {
        var (_, lab) = Lab();
        var window = Show(lab, "ColorPreviewer");
        try
        {
            var samples = Samples(window, dark: false);
            var seed = HsvOf(samples[0]);
            var shade = samples.OfType<ColorPreviewer>().First(previewer => previewer.IsEnabled)
                .GetVisualDescendants().OfType<Border>().First(border => border.Name == "PART_AccentIncrement1Border");
            Click(window, shade);

            Assert.NotEqual(seed, HsvOf(samples[0]));
            Assert.All(samples, sample => Assert.Equal(HsvOf(samples[0]), HsvOf(sample)));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Picking_another_accent_replaces_a_dragged_colour()
    {
        var (theme, lab) = Lab();
        var accent = lab.SelectedAccent;
        var window = Show(lab, "ColorSlider");
        try
        {
            var samples = Samples(window, dark: false);
            Drag(window, samples.OfType<ColorSlider>().First(slider => slider.ColorComponent == ColorComponent.Component1 && slider.IsEnabled), from: 0.2, to: 0.7);
            lab.SelectedAccent = MainWindowViewModel.Hue(AccentKind.Success);
            Dispatcher.UIThread.RunJobs();

            Assert.All(samples, sample => Assert.Equal(theme.AccentSuccess, ColorOf(sample)));
        }
        finally
        {
            lab.SelectedAccent = accent;
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Dragging_a_sample_leaves_the_theme_untouched()
    {
        var (theme, lab) = Lab();
        var seeds = Seeds(theme);
        var window = Show(lab, "ColorSlider");
        try
        {
            var saturation = Samples(window, dark: false).OfType<ColorSlider>()
                .First(slider => slider.ColorComponent == ColorComponent.Component2 && slider.IsEnabled);
            var dragged = saturation.Color;
            Drag(window, saturation, from: 0.9, to: 0.1);

            Assert.NotEqual(dragged, saturation.Color);
            Assert.Equal(seeds, Seeds(theme));
        }
        finally
        {
            window.Close();
        }
    }

    private static void AssertDragLinks(string entry, Func<IReadOnlyList<Control>, Control> pick)
    {
        var (_, lab) = Lab();
        var window = Show(lab, entry);
        try
        {
            var samples = Samples(window, dark: false);
            var seed = HsvOf(samples[0]);
            Drag(window, pick(samples), from: 0.5, to: 0.2);

            Assert.NotEqual(seed, HsvOf(samples[0]));
            Assert.All(samples, sample => Assert.Equal(HsvOf(samples[0]), HsvOf(sample)));
        }
        finally
        {
            window.Close();
        }
    }

    private static IReadOnlyList<Control> Samples(Window window, bool dark)
    {
        return window.GetVisualDescendants().OfType<Control>()
            .Where(control => control is ColorSlider or ColorSpectrum or ColorPreviewer or ColorView)
            .Where(control => control.TemplatedParent is null)
            .Where(control => InPane(control, dark))
            .Where(control => HsvOf(control).A >= 1)
            .ToList();
    }

    private static bool InPane(Visual visual, bool dark)
    {
        return visual.GetVisualAncestors().OfType<Border>().Any(border => border.Name == (dark ? "PART_DarkPane" : "PART_LightPane"));
    }

    private static HsvColor HsvOf(Control sample) => sample switch
    {
        ColorSlider slider => slider.HsvColor,
        ColorSpectrum spectrum => spectrum.HsvColor,
        ColorPreviewer previewer => previewer.HsvColor,
        ColorView view => view.HsvColor,
        _ => throw new ArgumentOutOfRangeException(nameof(sample)),
    };

    private static Color ColorOf(Control sample) => HsvOf(sample).ToRgb();

    private static Color[] Seeds(DefaultTheme theme) =>
        [theme.AccentNeutral, theme.AccentPrimary, theme.AccentSuccess, theme.AccentWarning, theme.AccentError, theme.AccentInfo];

    private static void Drag(Window window, Control control, double from, double to)
    {
        Point at(double fraction) => control.TranslatePoint(new Point(control.Bounds.Width * fraction, control.Bounds.Height * (control is ColorSlider ? 0.5 : fraction)), window)!.Value;
        window.MouseDown(at(from), MouseButton.Left);
        for (var step = 1; step <= 8; step++)
            window.MouseMove(at(from + (to - from) * step / 8));
        window.MouseUp(at(to), MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    private static void Click(Window window, Control control)
    {
        var centre = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseDown(centre, MouseButton.Left);
        window.MouseUp(centre, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    // The playground's view model is shared with every other test's window, so the entry shown before is put back when this window closes.
    private static Window Show(MainWindowViewModel lab, string entry)
    {
        var window = new MainWindow { DataContext = lab, Width = 1200, Height = 800 };
        var shown = lab.SelectedEntry;
        window.Closed += (_, _) => lab.SelectedEntry = shown;
        lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == entry);
        window.Show();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        AwaitSpectra(window);
        return window;
    }

    // A ColorSpectrum builds its colour map off the UI thread and ignores the pointer until it has one, which is when it paints its field
    private static void AwaitSpectra(Window window)
    {
        static bool painted(ColorSpectrum spectrum) => spectrum.GetVisualDescendants().OfType<Shape>()
            .Any(shape => shape.Name is "PART_SpectrumRectangle" or "PART_SpectrumEllipse" && shape.IsVisible && shape.Fill is not null);

        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (!window.GetVisualDescendants().OfType<ColorSpectrum>().All(painted) && DateTime.UtcNow < deadline)
        {
            Thread.Sleep(10);
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static (DefaultTheme Theme, MainWindowViewModel Gallery) Lab()
    {
        var app = (App)Application.Current!;
        var theme = TestTheme.Current;
        return (theme, app.Services.GetRequiredService<MainWindowViewModel>());
    }
}
