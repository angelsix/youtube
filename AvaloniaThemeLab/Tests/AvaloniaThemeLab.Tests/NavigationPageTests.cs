using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The navigation bar holds the page's title, so control height is only its floor.</summary>
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
        TestTheme.With(t => { t.ControlHeightScale = controlHeight; });
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

    /// <summary>The edge under the bar sits at the bar's real bottom.</summary>
    [AvaloniaTheory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(2)]
    public void The_edge_under_the_bar_follows_the_bar(double controlHeight)
    {
        var nav = new NavigationPage { HasShadow = true };
        var window = new Window { Content = nav, Width = 400, Height = 300 };
        TestTheme.With(t => { t.ControlHeightScale = controlHeight; });
        try
        {
            window.Show();
            _ = nav.PushAsync(new ContentPage { Header = "Root page", Content = new TextBlock { Text = "Body" } });
            for (var i = 0; i < 4; i++)
            {
                Dispatcher.UIThread.RunJobs();
                window.UpdateLayout();
            }

            var bar = nav.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "PART_NavigationBar");
            var barBottom = bar.TranslatePoint(new Point(0, bar.Bounds.Height), nav)!.Value.Y;
            var painted = nav.GetVisualDescendants().OfType<Border>()
                .Where(b => b.Name is "PART_NavBarShadow" or "PART_BarEdge" && b.IsEffectivelyVisible && b.Opacity > 0 && b.Bounds.Height > 0)
                .ToList();
            Assert.NotEmpty(painted);
            foreach (var edge in painted)
            {
                var top = edge.TranslatePoint(default, nav)!.Value.Y;
                Assert.True(Math.Abs(top - barBottom) < 0.5, $"{edge.Name} starts at {top:0.#}, the bar ends at {barBottom:0.#}");
            }
        }
        finally
        {
            window.Close();
        }
    }
}
