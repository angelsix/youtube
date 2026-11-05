using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// Disabled opacity is a 0 to 1 setting, and nothing derived from it may leave that range. The placeholder took
/// DisabledOpacity x 1.667, which passed 1 at 0.6: Avalonia refuses an opacity over 1 and every placeholder vanished.
/// A placeholder is not a disabled state, so it no longer follows the setting at all.
/// </summary>
public sealed class DisabledOpacityTests
{
    [AvaloniaTheory]
    [InlineData(0.3)]
    [InlineData(0.6)]
    [InlineData(0.8)]
    [InlineData(1.0)]
    public void Every_opacity_stays_in_range_and_placeholders_stay_visible(double disabledOpacity)
    {
        var field = new TextBox { PlaceholderText = "Placeholder" };
        var slider = new Slider { IsEnabled = false, Value = 50, Maximum = 100 };
        var toggle = new ToggleButton { Content = "On", IsChecked = true, IsEnabled = false };
        var window = new Window { Content = new StackPanel { Children = { field, slider, toggle } }, Width = 400, Height = 300 };
        ThemeRegion.SetTheme(window, new DefaultTheme { DisabledOpacity = disabledOpacity });
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var placeholder = field.GetVisualDescendants().OfType<TextBlock>().Single(t => t.Name == "PART_Placeholder");
            Assert.True(placeholder.IsEffectivelyVisible, "The placeholder is hidden");
            Assert.Equal(0.5, placeholder.Opacity, 3);
            Assert.InRange(slider.Opacity, disabledOpacity - 0.001, 1);
            Assert.InRange(toggle.Opacity, disabledOpacity - 0.001, 1);
        }
        finally
        {
            window.Close();
        }
    }
}
