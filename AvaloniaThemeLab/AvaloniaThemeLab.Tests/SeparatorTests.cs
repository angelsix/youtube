using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// A separator draws a line the length of its own box. A vertical rule given its length by MinHeight (the preview's
/// rule between two items) drew a 2px dot, because the template pinned the drawn line to the control's Height.
/// </summary>
public sealed class SeparatorTests
{
    [AvaloniaFact]
    public void A_vertical_rule_draws_the_full_length_of_its_box()
    {
        var rule = new Separator { Width = 2, MinHeight = 40 };
        var row = new Grid { ColumnDefinitions = new("*,Auto,*"), MinHeight = 40, Children = { new Label { Content = "First" }, rule, new Label { Content = "Second" } } };
        Grid.SetColumn(rule, 1);
        Grid.SetColumn(row.Children[2], 2);
        var window = new Window { Content = row, Width = 400, Height = 200 };
        ThemeRegion.SetTheme(window, new DefaultTheme());
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var line = rule.GetVisualDescendants().OfType<Border>().First();
            Assert.True(rule.Bounds.Height >= 40, $"The rule's box is {rule.Bounds.Height:0.#}px tall");
            Assert.True(line.Bounds.Height >= rule.Bounds.Height - 0.5, $"The line is {line.Bounds.Height:0.#}px of a {rule.Bounds.Height:0.#}px box");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void A_horizontal_rule_stays_a_hairline()
    {
        var rule = new Separator();
        var window = new Window { Content = new StackPanel { Children = { rule } }, Width = 400, Height = 200 };
        ThemeRegion.SetTheme(window, new DefaultTheme());
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var line = rule.GetVisualDescendants().OfType<Border>().First();
            Assert.Equal(rule.Height, line.Bounds.Height);
            Assert.True(line.Bounds.Width > 100, $"The line is {line.Bounds.Width:0.#}px wide");
        }
        finally
        {
            window.Close();
        }
    }
}
