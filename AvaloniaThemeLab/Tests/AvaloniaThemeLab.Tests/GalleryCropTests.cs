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

/// <summary>With no spacing and no control height, an entry's preview still shows every line of its text whole.</summary>
public sealed class GalleryCropTests
{
    public static TheoryData<string> Entries() => ["Expander", "TabControl", "TransitioningContentControl", "Separator", "ManagedFileChooser", "NavigationPage", "Window"];

    [AvaloniaTheory]
    [MemberData(nameof(Entries))]
    public void No_text_is_cropped_at_minimum_sizes(string entry)
    {
        var lab = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
        var shown = lab.SelectedEntry;
        var window = new MainWindow { DataContext = lab, Width = 1200, Height = 900 };
        TestTheme.With(t => { t.SpacingScale = 0; t.ControlHeightScale = 0; });
        try
        {
            lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == entry);
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            var host = window.GetVisualDescendants().OfType<ScrollViewer>().First(s => s.Name == "PageHost");
            var cropped = host.GetVisualDescendants().OfType<TextBlock>()
                .Where(t => t.IsEffectivelyVisible && !string.IsNullOrWhiteSpace(t.Text))
                .Select(t => (Text: t, Visible: VisibleHeight(t, host, out var by)))
                .Where(x => x.Visible.Height < x.Text.FontSize - 0.5)
                .Select(x => $"'{x.Text.Text}' shows {x.Visible.Height:0.#}px of a {x.Text.FontSize:0.#}px font, cut by {x.Visible.By}")
                .ToList();

            Assert.True(cropped.Count == 0, $"{entry}: " + string.Join("; ", cropped.Take(6)));
        }
        finally
        {
            window.Close();
            lab.SelectedEntry = shown;
        }
    }

    /// <summary>The file chooser's columns are content, not gaps.</summary>
    [AvaloniaFact]
    public void File_chooser_columns_keep_their_widths_with_no_spacing()
    {
        var lab = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
        var shown = lab.SelectedEntry;
        var window = new MainWindow { DataContext = lab, Width = 1400, Height = 900 };
        TestTheme.With(t => { t.SpacingScale = 0; });
        try
        {
            lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == "ManagedFileChooser");
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            var headers = window.GetVisualDescendants().OfType<Grid>().First(g => g.Name == "FileHeaders");
            var cells = headers.Children.Where(c => c is TextBlock or GridSplitter).OrderBy(Grid.GetColumn).ToList();
            var problems = new List<string>();
            for (var i = 0; i < cells.Count; i++)
            {
                if (cells[i].Bounds.Width < 1)
                    problems.Add($"{cells[i].Name} is {cells[i].Bounds.Width:0.#}px wide");
                if (i > 0 && cells[i].Bounds.X < cells[i - 1].Bounds.Right - 0.5)
                    problems.Add($"{cells[i].Name} starts at {cells[i].Bounds.X:0.#}, inside {cells[i - 1].Name} (ends {cells[i - 1].Bounds.Right:0.#})");
            }

            Assert.True(problems.Count == 0, string.Join("; ", problems));
        }
        finally
        {
            window.Close();
            lab.SelectedEntry = shown;
        }
    }

    // How much of a text block's height survives every ancestor that clips, up to the page host
    private static (double Height, string By) VisibleHeight(Visual text, Visual host, out string by)
    {
        by = "-";
        var top = text.TranslatePoint(new Point(0, 0), host)!.Value.Y;
        var bottom = top + text.Bounds.Height;
        for (var v = text.GetVisualParent(); v is not null && v != host; v = v.GetVisualParent())
        {
            // A clip is the element's own box; layout clips a child that overflows a fixed-size parent the same way
            var vt = v.TranslatePoint(new Point(0, 0), host)!.Value.Y;
            // A scroll presenter's clip is a viewport: content below it scrolls into view, it is not lost
            if (v is Avalonia.Controls.Presenters.ScrollContentPresenter)
                continue;
            if (v.ClipToBounds || v is Avalonia.Layout.Layoutable { Height: not double.NaN })
            {
                var before = bottom - top;
                top = Math.Max(top, vt);
                bottom = Math.Min(bottom, vt + v.Bounds.Height);
                if (bottom - top < before)
                    by = $"{v.GetType().Name}#{(v as StyledElement)?.Name}";
            }
        }

        return (Math.Max(0, bottom - top), by);
    }
}
