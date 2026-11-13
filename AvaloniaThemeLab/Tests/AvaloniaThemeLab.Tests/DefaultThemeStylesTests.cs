using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The theme's app-wide rules (the ones a ControlTheme cannot hold) live in DefaultThemeStyles.axaml, the XAML half of the DefaultThemeStyles class an app includes.</summary>
public sealed class DefaultThemeStylesTests
{
    [AvaloniaFact]
    public void An_app_page_takes_the_themes_clip_setting()
    {
        // Under the theme's default WebRenderStyle, ControlClipToBounds is false.
        var page = new AppPage { Content = new TextBlock { Text = "Page" } };
        var window = new Window { Content = page, Width = 400, Height = 200 };
        TestTheme.With(t => { t.WebRenderStyle = true; });
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.False(page.ClipToBounds, "A UserControl page still clips: it did not take ControlClipToBounds (false under WebRenderStyle)");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void A_tabbed_page_squares_the_corners_that_meet_its_strip()
    {
        // TabPlacement=Top: the hosted page's top pair meets the strip and squares; the bottom pair keeps the radius.
        var page = new ContentPage { Header = "Home", Content = new TextBlock { Text = "Home" } };
        var tabs = new TabbedPage { TabPlacement = TabPlacement.Top, ItemsSource = new[] { page } };
        var window = new Window { Content = tabs, Width = 600, Height = 400 };
        TestTheme.With(null);
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var corners = page.CornerRadius;
            Assert.True(corners.TopLeft == 0 && corners.TopRight == 0, $"Top corners should be square, were {corners}");
            Assert.True(corners.BottomLeft > 0 && corners.BottomRight > 0, $"Bottom corners should keep the theme radius, were {corners}");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>An app's own page: a UserControl subclass, as every page in an app is.</summary>
    private sealed class AppPage : UserControl;
}
