using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The preview's vertical bar with its percentage shown is as wide as its turned text and that text's padding, so it follows the font size and the spacing, and the text always fits across it.</summary>
public sealed class ProgressBarTests
{
    [AvaloniaFact]
    public void The_vertical_text_bar_follows_font_size_and_spacing()
    {
        var plain = Measure(null);
        var bigText = Measure(t => { t.FontSizeScale = 2; });
        var noSpace = Measure(t => { t.SpacingScale = 0; });

        Assert.True(bigText.Bar > plain.Bar + 4, $"Font size 2: the bar is {bigText.Bar:0.#}px, at 1 it is {plain.Bar:0.#}px");
        Assert.True(noSpace.Bar < plain.Bar - 4, $"Spacing 0: the bar is {noSpace.Bar:0.#}px, at 1 it is {plain.Bar:0.#}px");
        foreach (var (name, m) in new[] { ("plain", plain), ("font 2", bigText), ("spacing 0", noSpace) })
            Assert.True(m.Text <= m.Bar + 0.5, $"{name}: the turned text is {m.Text:0.#}px across a {m.Bar:0.#}px bar");
    }

    private static (double Bar, double Text) Measure(Action<DefaultTheme>? theme)
    {
        var lab = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
        var shown = lab.SelectedEntry;
        var window = new MainWindow { DataContext = lab, Width = 1400, Height = 1400 };
        TestTheme.With(theme);
        try
        {
            lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == "ProgressBar");
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var bar = window.GetVisualDescendants().OfType<ProgressBar>()
                .First(p => p.Orientation == Orientation.Vertical && p.ShowProgressText);
            var text = bar.GetVisualDescendants().OfType<LayoutTransformControl>().Single(l => l.Name == "PART_LayoutTransformControl");
            return (bar.Bounds.Width, text.Bounds.Width);
        }
        finally
        {
            window.Close();
            lab.SelectedEntry = shown;
        }
    }
}
