using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// A dialog window draws its own frame only where the operating system draws none. Windows gives a window no shadow or
/// outline of its own, so a dialog over another window needs the theme's border to stand apart; macOS already
/// outlines and shadows a dialog, and a second border doubles it.
/// </summary>
public sealed class DialogWindowTests
{
    [AvaloniaFact]
    public void A_dialog_draws_a_border_only_on_windows()
    {
        var dialog = new Window { Classes = { "dialog" }, Width = 300, Height = 200 };
        ThemeRegion.SetTheme(dialog, new DefaultTheme());
        try
        {
            dialog.Show();
            Dispatcher.UIThread.RunJobs();

            var expected = OperatingSystem.IsWindows() ? new Thickness(1) : default;
            Assert.Equal(expected, dialog.BorderThickness);
        }
        finally
        {
            dialog.Close();
        }
    }
}
