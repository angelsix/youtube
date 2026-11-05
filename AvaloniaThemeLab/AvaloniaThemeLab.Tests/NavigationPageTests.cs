using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// The navigation bar holds the page's title, so control height is only its floor: at control height 0 the title still
/// shows whole, and the page beneath starts below the bar rather than under it.
/// </summary>
public sealed class NavigationPageTests
{
    [AvaloniaTheory]
    [InlineData(1)]
    [InlineData(0)]
    public void The_bar_keeps_its_title_and_the_page_starts_below_it(double controlHeight)
    {
        var body = new TextBlock { Text = "Body" };
        var nav = new NavigationPage();
        var window = new Window { Content = nav, Width = 400, Height = 300 };
        ThemeRegion.SetTheme(window, new DefaultTheme { ControlHeightScale = controlHeight });
        try
        {
            window.Show();
            _ = nav.PushAsync(new ContentPage { Header = "Root page", Content = body });
            for (var i = 0; i < 4; i++)
            {
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
            }

            var bar = nav.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "PART_NavigationBar");
            var title = nav.GetVisualDescendants().OfType<TextBlock>().First(t => t.Text == "Root page");
            var titleTop = title.TranslatePoint(default, nav)!.Value.Y;
            var titleBottom = titleTop + title.Bounds.Height;
            var barBottom = bar.TranslatePoint(new Point(0, bar.Bounds.Height), nav)!.Value.Y;
            var bodyTop = body.TranslatePoint(default, nav)!.Value.Y;

            Assert.True(title.Bounds.Height >= title.FontSize, $"The title is {title.Bounds.Height:0.#}px tall for a {title.FontSize:0.#}px font");
            Assert.True(titleTop >= -0.5 && titleBottom <= barBottom + 0.5, $"The title spans {titleTop:0.#}-{titleBottom:0.#}, the bar ends at {barBottom:0.#}");
            Assert.True(bodyTop >= barBottom - 0.5, $"The page starts at {bodyTop:0.#}, under the bar ending at {barBottom:0.#}");
            if (controlHeight == 1)
                Assert.Equal(48, bar.Bounds.Height);
        }
        finally
        {
            window.Close();
        }
    }
}
