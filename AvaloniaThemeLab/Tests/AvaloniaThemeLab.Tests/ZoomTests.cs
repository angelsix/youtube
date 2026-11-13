using AngelSix.ThemeEngine;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
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
    private static ZoomClock? clock;

    /// <summary>The engine whose dark-region twin follows live theme edits.</summary>
    private static readonly Version darkTwinFollowsEdits = new(1, 23, 0);

    /// <summary>Whether the referenced engine can zoom a dark region live (see <see cref="darkTwinFollowsEdits"/>).</summary>
    public static bool DarkPaneFollowsLiveEdits => typeof(ThemeContext).Assembly.GetName().Version >= darkTwinFollowsEdits;

    /// <summary>Every gallery entry, read off the booted lab so a new control is tested the day it is themed.</summary>
    public static TheoryData<string> Entries()
    {
        var session = HeadlessUnitTestSession.GetOrStartForAssembly(typeof(ZoomTests).Assembly);
        var names = session.Dispatch(() => Lab().Gallery.Catalogue.Entries.Select(e => e.Name).ToArray(), CancellationToken.None)
            .GetAwaiter().GetResult();
        return [.. names];
    }

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
            var one = SteadyShot(window, theme, inScope);
            theme.BaseSize = Zoom;
            window.Width = baseWindow.Width * Zoom;
            window.Height = baseWindow.Height * Zoom;
            var two = SteadyShot(window, theme, inScope);
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

    // Both shots rest at the same animation phase and the same scroll position: every settle
    // step pulses the clock a whole loop (so anything started mid-settle finishes too) and
    // zeroes the scroll offsets, which are where the user left the page, not layout
    private static Dictionary<string, ElementShot> SteadyShot(MainWindow window, DefaultTheme theme, Func<Visual, bool> inScope)
    {
        clock!.AttachEverywhere(window);
        void Step()
        {
            clock.Advance();
            foreach (var viewer in window.GetVisualDescendants().OfType<ScrollViewer>())
                viewer.Offset = default;
        }
        // Templates apply lazily on first layout: settle before swapping panels so both
        // shots see the same realised set, not one mid-rebuild
        ZoomProbe.Settle(window, Transitions(theme), Step);
        Devirtualize(window);
        return ZoomProbe.Shoot(window, Transitions(theme), inScope, Step);
    }

    // A virtualizing panel realises a viewport-dependent set of rows, so the two shots would
    // compare different element sets; a plain panel realises them all. Realisation is not a size
    private static void Devirtualize(Visual root)
    {
        foreach (var panel in root.GetVisualDescendants().OfType<VirtualizingStackPanel>().ToList())
        {
            if (panel.TemplatedParent is ItemsControl items)
                items.ItemsPanel = new PlainPanelTemplate(panel.Orientation);
        }
    }

    private sealed class PlainPanelTemplate(Orientation orientation) : ITemplate<Panel?>
    {
        public Panel? Build() => new StackPanel { Orientation = orientation };

        object? ITemplate.Build() => Build();
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
        clock = new ZoomClock();
        clock.Attach(gallery);
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
