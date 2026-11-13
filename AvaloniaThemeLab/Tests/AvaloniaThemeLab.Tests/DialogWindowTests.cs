using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>A dialog window draws its own frame only where the operating system draws none.</summary>
public sealed class DialogWindowTests
{
    [AvaloniaFact]
    public void A_dialog_draws_a_border_only_on_windows()
    {
        var dialog = new Window { Classes = { "dialog" }, Width = 300, Height = 200 };
        TestTheme.With(null);
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
