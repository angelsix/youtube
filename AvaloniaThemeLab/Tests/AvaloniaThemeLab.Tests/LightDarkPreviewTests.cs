using AngelSix.ThemeEngine.Controls;
using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The light/dark preview shows one pane light and one dark whatever the app is doing, and a preview wider than its pane can be scrolled to rather than being cut off.</summary>
public sealed class LightDarkPreviewTests
{
    [AvaloniaFact]
    public void The_light_pane_stays_light_when_the_app_is_dark()
    {
        var (window, lab, shown) = Open("Button", t => { t.IsDark = true; }, 1200);
        try
        {
            var preview = window.GetVisualDescendants().OfType<LightDarkPreview>().First();
            var light = Lightness(Pane(preview, "PART_LightPane"));
            var dark = Lightness(Pane(preview, "PART_DarkPane"));

            Assert.True(light > 0.5, $"The light pane's surface is {light:0.00} lightness with the app dark");
            Assert.True(dark < 0.5, $"The dark pane's surface is {dark:0.00} lightness with the app dark");
        }
        finally
        {
            Close(window, lab, shown);
        }
    }

    [AvaloniaFact]
    public void A_preview_wider_than_its_pane_scrolls_to_its_far_edge()
    {
        // ColorSpectrum at twice the base size is wider than half of a 900px window
        var (window, lab, shown) = Open("ColorSpectrum", t => { t.BaseSize = 2; }, 900);
        try
        {
            var preview = window.GetVisualDescendants().OfType<LightDarkPreview>().First();
            foreach (var paneName in new[] { "PART_LightPane", "PART_DarkPane" })
            {
                var viewer = Pane(preview, paneName).GetVisualDescendants().OfType<ScrollViewer>().First();
                var content = (Control)viewer.Content!;

                // The furthest right any part of the preview reaches, in the scrolled content's own coordinates
                var right = content.GetVisualDescendants().OfType<Visual>()
                    .Where(v => v.IsEffectivelyVisible && v.Bounds.Width > 0)
                    .Select(v => v.TranslatePoint(new Point(v.Bounds.Width, 0), content)?.X ?? 0)
                    .Max() + content.Bounds.X;
                Assert.True(right > viewer.Viewport.Width + 1, $"{paneName}: the test needs a preview wider than the pane ({right:0.#} vs {viewer.Viewport.Width:0.#})");

                // Scroll as far right as the viewer allows: the preview's far edge must come into view
                viewer.Offset = new Vector(double.MaxValue, 0);
                window.UpdateLayout();
                Dispatcher.UIThread.RunJobs();
                var reach = viewer.Offset.X + viewer.Viewport.Width;
                Assert.True(reach >= right - 0.5, $"{paneName}: the preview reaches {right:0.#}px, scrolling shows up to {reach:0.#}px");
            }
        }
        finally
        {
            Close(window, lab, shown);
        }
    }

    /// <summary>A preview opens at its origin.</summary>
    [AvaloniaFact]
    public void A_wide_preview_opens_at_its_origin()
    {
        var (window, lab, shown) = Open("TabControl", null, 1440);
        try
        {
            AssertAtOrigin(window, "opened");
            lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == "Button");
            Settle(window);
            var viewer = window.GetVisualDescendants().OfType<LightDarkPreview>().First().GetVisualDescendants().OfType<ScrollViewer>().First();
            viewer.Offset = new Vector(40, 0);
            lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == "TabControl");
            Settle(window);
            AssertAtOrigin(window, "re-pointed");
        }
        finally
        {
            Close(window, lab, shown);
        }
    }

    private static void AssertAtOrigin(Window window, string when)
    {
        var preview = window.GetVisualDescendants().OfType<LightDarkPreview>().First();
        foreach (var viewer in preview.GetVisualDescendants().OfType<ScrollViewer>().Where(v => v.TemplatedParent == preview))
        {
            Assert.True(viewer.Extent.Width > viewer.Viewport.Width, $"{when}: the test needs a preview wider than its pane");
            Assert.True(viewer.Offset == default, $"{when}: a pane opened scrolled to {viewer.Offset}");
        }
    }

    private static void Settle(Window window)
    {
        for (var i = 0; i < 3; i++)
        {
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
        }
        Dispatcher.UIThread.RunJobs();
    }

    private static (MainWindow Window, MainWindowViewModel Lab, GalleryEntryViewModel? Shown) Open(string entry, Action<DefaultTheme>? theme, double width)
    {
        var lab = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
        var shown = lab.SelectedEntry;
        var window = new MainWindow { DataContext = lab, Width = width, Height = 900 };
        TestTheme.With(theme);
        lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == entry);
        window.Show();
        Settle(window);
        return (window, lab, shown);
    }

    private static void Close(Window window, MainWindowViewModel lab, GalleryEntryViewModel? shown)
    {
        window.Close();
        if (shown is not null)
            lab.SelectedEntry = shown;
    }

    private static Border Pane(LightDarkPreview preview, string name) =>
        preview.GetVisualDescendants().OfType<Border>().Single(b => b.Name == name);

    private static double Lightness(Border pane)
    {
        var colour = Assert.IsAssignableFrom<ISolidColorBrush>(pane.Background).Color;
        return (0.2126 * colour.R + 0.7152 * colour.G + 0.0722 * colour.B) / 255;
    }
}
