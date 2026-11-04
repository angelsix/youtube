using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ViewModels;
using AvaloniaThemeLab.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// With no spacing and no control height, an entry's preview still shows every line of its text whole: nothing a
/// control-height value sizes may crop the content inside it. The window carries a theme of its own.
/// </summary>
public sealed class GalleryCropTests
{
    public static TheoryData<string> Entries() => ["Expander", "TabControl", "TransitioningContentControl", "Separator", "ManagedFileChooser"];

    [AvaloniaTheory]
    [MemberData(nameof(Entries))]
    public void No_text_is_cropped_at_minimum_sizes(string entry)
    {
        var lab = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
        var shown = lab.SelectedEntry;
        var window = new MainWindow { DataContext = lab, Width = 1200, Height = 900 };
        ThemeRegion.SetTheme(window, new DefaultTheme { SpacingScale = 0, ControlHeightScale = 0 });
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
