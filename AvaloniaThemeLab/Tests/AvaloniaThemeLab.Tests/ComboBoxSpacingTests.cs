using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>A combo box's spacing is the same on every side.</summary>
public sealed class ComboBoxSpacingTests
{
    [AvaloniaTheory]
    [InlineData(1)]
    [InlineData(3)]
    public void The_insets_are_equal_on_every_side(double spacing)
    {
        var box = new ComboBox { PlaceholderText = "Select…" };
        var window = new Window { Content = new StackPanel { Children = { box } }, Width = 600, Height = 300 };
        TestTheme.With(t => { t.SpacingScale = spacing; });
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var padding = box.Padding;
            Assert.True(padding.Left == padding.Top && padding.Top == padding.Bottom && padding.Left == padding.Right,
                $"Spacing {spacing}: the padding is {padding}");

            var frame = box.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "Background");
            var chevron = box.GetVisualDescendants().OfType<PathIcon>().Single(p => p.Name == "DropDown");
            var frameRight = frame.TranslatePoint(new Point(frame.Bounds.Width, 0), box)!.Value.X;
            var chevronRight = chevron.TranslatePoint(new Point(chevron.Bounds.Width, 0), box)!.Value.X;
            var frameLeft = frame.TranslatePoint(default, box)!.Value.X;
            var text = box.GetVisualDescendants().OfType<TextBlock>().Single(t => t.Name == "PlaceholderTextBlock");
            var textLeft = text.TranslatePoint(default, box)!.Value.X;
            Assert.Equal(textLeft - frameLeft, frameRight - chevronRight, 0.5);
        }
        finally
        {
            window.Close();
        }
    }
}
