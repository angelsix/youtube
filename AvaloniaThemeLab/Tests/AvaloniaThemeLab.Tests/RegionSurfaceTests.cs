using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// An element given its own ThemeRegion.IsDark draws the region's surface behind it. Without that, a control whose
/// theme leaves it transparent (TextBox) or a bare Border draws dark-palette text straight onto the light window.
/// </summary>
public sealed class RegionSurfaceTests
{
    [AvaloniaFact]
    public void A_text_box_set_dark_on_its_own_draws_a_dark_field_in_a_light_window()
    {
        var box = new TextBox { Width = 240, Text = "Hi" };
        ThemeRegion.SetIsDark(box, true);

        Render(box, (frame, origin) =>
        {
            // Inside the field, right of the text and clear of the border
            var lightness = Lightness(frame, (int)(origin.X + box.Bounds.Width - 12), (int)(origin.Y + box.Bounds.Height / 2));
            Assert.True(lightness < 0.5, $"The dark TextBox's field is {lightness:0.00} lightness on a light window");
        });
    }

    [AvaloniaFact]
    public void A_bare_border_set_dark_draws_a_dark_surface()
    {
        var border = new Border { Width = 200, Height = 80, Child = new TextBlock { Text = "Dark" } };
        ThemeRegion.SetIsDark(border, true);

        Render(border, (frame, origin) =>
        {
            var lightness = Lightness(frame, (int)(origin.X + border.Bounds.Width - 10), (int)(origin.Y + border.Bounds.Height - 10));
            Assert.True(lightness < 0.5, $"The dark region's surface is {lightness:0.00} lightness on a light window");
        });
    }

    [AvaloniaFact]
    public void A_background_set_on_the_region_wins()
    {
        var border = new Border { Width = 200, Height = 80, Background = Brushes.Red };
        ThemeRegion.SetIsDark(border, true);

        Render(border, (_, _) => Assert.Equal(Brushes.Red, border.Background));
    }

    [AvaloniaFact]
    public void A_control_inside_a_dark_region_keeps_its_own_transparent_background()
    {
        var box = new TextBox { Width = 200 };
        var border = new Border { Padding = new Thickness(10), Child = box };
        ThemeRegion.SetIsDark(border, true);

        Render(border, (_, _) =>
        {
            // Only the element the region is set on paints the surface; the region's contents sit on it
            var colour = (box.Background as ISolidColorBrush)?.Color ?? Colors.Transparent;
            Assert.Equal(0, colour.A);
        });
    }

    private static void Render(Control control, Action<WriteableBitmap, Point> check)
    {
        var window = new Window { Width = 400, Height = 200, Content = new Panel { Margin = new Thickness(20), Children = { control } } };
        TestTheme.With(null);
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            // Not disposed: the headless platform owns the frame it hands back
            var frame = window.CaptureRenderedFrame()!;
            check(frame, control.TranslatePoint(new Point(0, 0), window)!.Value);
        }
        finally
        {
            window.Close();
        }
    }

    // Mean of the three colour channels, so byte order does not matter
    private static double Lightness(WriteableBitmap frame, int x, int y)
    {
        Assert.InRange(x, 0, frame.PixelSize.Width - 1);
        Assert.InRange(y, 0, frame.PixelSize.Height - 1);

        var pixel = new int[1];
        var handle = System.Runtime.InteropServices.GCHandle.Alloc(pixel, System.Runtime.InteropServices.GCHandleType.Pinned);
        try
        {
            frame.CopyPixels(new PixelRect(x, y, 1, 1), handle.AddrOfPinnedObject(), sizeof(int), sizeof(int));
        }
        finally
        {
            handle.Free();
        }

        var value = (uint)pixel[0];
        return (((value >> 16) & 0xFF) + ((value >> 8) & 0xFF) + (value & 0xFF)) / 3.0 / 255;
    }
}
