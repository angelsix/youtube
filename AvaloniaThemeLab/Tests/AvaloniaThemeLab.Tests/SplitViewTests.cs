using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>A split view's pane is content width, not a hit target.</summary>
public sealed class SplitViewTests
{
    [AvaloniaFact]
    public void Pane_lengths_do_not_follow_control_height()
    {
        var full = Lengths(null);
        var none = Lengths(t => { t.ControlHeightScale = 0; });

        Assert.NotEmpty(full);
        Assert.Equal(full, none);
    }

    [AvaloniaFact]
    public void A_bare_split_view_keeps_its_compact_pane_at_control_height_0()
    {
        var view = new SplitView();
        var window = new Window { Content = view, Width = 600, Height = 300 };
        TestTheme.With(t => { t.ControlHeightScale = 0; });
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(48, view.CompactPaneLength);
        }
        finally
        {
            window.Close();
        }
    }

    private static List<string> Lengths(Action<DefaultTheme>? theme)
    {
        var lab = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
        var shown = lab.SelectedEntry;
        var window = new MainWindow { DataContext = lab, Width = 1400, Height = 900 };
        TestTheme.With(theme);
        try
        {
            lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == "SplitView");
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            return window.GetVisualDescendants().OfType<SplitView>()
                .Select(v => $"{v.PanePlacement} {v.DisplayMode}: open {v.OpenPaneLength:0.#}, compact {v.CompactPaneLength:0.#}")
                .ToList();
        }
        finally
        {
            window.Close();
            lab.SelectedEntry = shown;
        }
    }
}
