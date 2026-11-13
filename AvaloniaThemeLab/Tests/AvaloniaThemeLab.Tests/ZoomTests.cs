using AngelSix.ThemeEngine;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>BaseSize is a zoom.</summary>
public sealed class ZoomTests
{
    private const double Zoom = 2;
    private static readonly Size baseWindow = new(1200, 800);
    private static MainWindow? gallery;

    /// <summary>The engine whose dark-region twin follows live theme edits.</summary>
    private static readonly Version darkTwinFollowsEdits = new(1, 23, 0);

    /// <summary>Every gallery entry, read off the booted lab so a new control is tested the day it is themed.</summary>
    public static TheoryData<string> Entries()
    {
        var session = HeadlessUnitTestSession.GetOrStartForAssembly(typeof(ZoomTests).Assembly);
        var names = session.Dispatch(() => Lab().Gallery.Catalogue.Entries.Select(e => e.Name).ToArray(), CancellationToken.None)
            .GetAwaiter().GetResult();
        return [.. names];
    }

    /// <summary>Whether the referenced engine can zoom a dark region live (see <see cref="darkTwinFollowsEdits"/>).</summary>
    public static bool DarkPaneFollowsLiveEdits => typeof(ThemeContext).Assembly.GetName().Version >= darkTwinFollowsEdits;

    [AvaloniaTheory]
    [MemberData(nameof(Entries))]
    public void Light_pane_zooms_exactly(string entry)
    {
        AssertEntryZooms(entry, dark: false);
    }

    [AvaloniaTheory(Skip = "AngelSix.ThemeEngine below 1.23.0: its dark-region twin does not follow live edits, so the dark pane keeps BaseSize 1",
        SkipUnless = nameof(DarkPaneFollowsLiveEdits))]
    [MemberData(nameof(Entries))]
    public void Dark_pane_zooms_exactly(string entry)
    {
        AssertEntryZooms(entry, dark: true);
    }

    /// <summary>The window around the page: search box, sidebar, header and the page host itself.</summary>
    [AvaloniaFact]
    public void Main_window_chrome_zooms_exactly()
    {
        var (theme, lab) = Lab();
        var shown = lab.SelectedEntry;
        // No page, so the page host's own bars and frame are measured without a page's content behind them
        lab.SelectedEntry = null;
        try
        {
            var report = Measure(theme, visual => !IsPageContent(visual));
            Assert.True(report.Passed, report.Describe("Main window chrome", Zoom));
        }
        finally
        {
            lab.SelectedEntry = shown;
        }
    }

    /// <summary>The settings window draws with the app's one theme, so it zooms with the app it edits.</summary>
    [AvaloniaFact]
    public void Settings_window_follows_base_size()
    {
        var (theme, _) = Lab();
        var app = (App)Application.Current!;
        var window = new ThemeSettingsWindow { DataContext = app.Services.GetRequiredService<ThemeSettingsViewModel>() };
        window.Show();
        var original = theme.BaseSize;
        try
        {
            Dispatcher.UIThread.RunJobs();
            var before = window.FontSize;
            theme.BaseSize = Zoom;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(before * Zoom, window.FontSize, 3);
        }
        finally
        {
            theme.BaseSize = original;
            window.Close();
        }
    }

    private static void AssertEntryZooms(string entryName, bool dark)
    {
        var (theme, lab) = Lab();
        var entry = lab.Catalogue.Entries.Single(e => e.Name == entryName);
        var shown = lab.SelectedEntry;
        lab.SelectedEntry = entry;
        try
        {
            var report = Measure(theme, visual => IsPageContent(visual) && InDarkPane(visual) == dark);
            Assert.True(report.Passed, report.Describe($"{entryName} {(dark ? "dark" : "light")} pane", Zoom, "#PageHost"));
        }
        finally
        {
            lab.SelectedEntry = shown;
        }
    }

    // Lays the main window out at BaseSize 1, then at BaseSize 2 in a window twice the size
    private static ZoomReport Measure(DefaultTheme theme, Func<Visual, bool> inScope)
    {
        var window = Gallery();
        var original = theme.BaseSize;
        try
        {
            theme.BaseSize = 1;
            window.Width = baseWindow.Width;
            window.Height = baseWindow.Height;
            var one = ZoomProbe.Shoot(window, Transitions(theme), inScope);
            theme.BaseSize = Zoom;
            window.Width = baseWindow.Width * Zoom;
            window.Height = baseWindow.Height * Zoom;
            var two = ZoomProbe.Shoot(window, Transitions(theme), inScope);
            return ZoomProbe.Compare(one, two, Zoom);
        }
        finally
        {
            theme.BaseSize = original;
            window.Width = baseWindow.Width;
            window.Height = baseWindow.Height;
            ZoomProbe.Settle(window, Transitions(theme));
        }
    }

    // One main window for the run, re-pointed at each entry, as the app itself does.
    private static MainWindow Gallery()
    {
        if (gallery is { IsVisible: true })
            return gallery;
        var app = (App)Application.Current!;
        gallery = new MainWindow
        {
            DataContext = app.Services.GetRequiredService<MainWindowViewModel>(),
            Width = baseWindow.Width,
            Height = baseWindow.Height,
        };
        gallery.Styles.Add(new Style(x => x.Is<Layoutable>())
        {
            Setters = { new Setter(Layoutable.UseLayoutRoundingProperty, false) },
        });
        gallery.Show();
        return gallery;
    }

    // The slowest theme transition is twice the base Animation; a margin covers the first frame's delay
    private static TimeSpan Transitions(DefaultTheme theme)
    {
        return TimeSpan.FromMilliseconds(theme.Animation * 2 + 100);
    }

    private static (DefaultTheme Theme, MainWindowViewModel Gallery) Lab()
    {
        var app = (App)Application.Current!;
        var theme = TestTheme.Current;
        return (theme, app.Services.GetRequiredService<MainWindowViewModel>());
    }

    // What the page host scrolls: the page, not the host's own bars
    private static bool IsPageContent(Visual visual)
    {
        return visual.GetVisualAncestors().OfType<ScrollContentPresenter>()
            .Any(presenter => presenter.TemplatedParent is ScrollViewer { Name: "PageHost" });
    }

    private static bool InDarkPane(Visual visual)
    {
        return visual.GetSelfAndVisualAncestors().OfType<Border>().Any(border => border.Name == "PART_DarkPane");
    }
}
