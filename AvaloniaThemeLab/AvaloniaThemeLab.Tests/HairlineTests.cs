using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// A border never rounds away to nothing when BaseSize shrinks. Avalonia snaps a border's thickness to
/// whole device pixels (banker's rounding), so a 0.5px hairline at 1x scale paints as 0 and the control
/// loses its outline entirely. Drawn the way a person sees it: a real themed TextBox, rendered, and the
/// pixel on its top edge compared with the pixel just inside it.
/// </summary>
public sealed class HairlineTests
{
    [AvaloniaTheory]
    [InlineData(1.0)]
    [InlineData(0.75)]
    [InlineData(0.5)]
    [InlineData(0.25)]
    public void A_text_box_keeps_its_border_at_small_base_sizes(double baseSize)
    {
        var box = new TextBox { Width = 200, Text = string.Empty };
        var window = new Window { Width = 400, Height = 200, Content = new Panel { Margin = new Thickness(20), Children = { box } } };
        // A theme of the test's own, pinned to this window only: changing the shared theme re-evaluates every live
        // window's bindings, and a brush transition the accent resolver re-triggers can overflow the stack (ZoomProbe)
        ThemeRegion.SetTheme(window, new DefaultTheme { BaseSize = baseSize });
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            // The window's own theme reached the box: its minimum height is ControlHeightSm (32) at this BaseSize
            Assert.Equal(32 * baseSize, box.MinHeight, 3);

            // Not disposed: the headless platform owns the frame it hands back
            var frame = window.CaptureRenderedFrame()!;
            var origin = box.TranslatePoint(new Point(0, 0), window)!.Value;
            var x = (int)(origin.X + box.Bounds.Width / 2);
            var top = (int)Math.Round(origin.Y);

            var edge = Pixel(frame, x, top);
            var inside = Pixel(frame, x, top + (int)Math.Ceiling(box.Bounds.Height / 2));

            Assert.True(edge != inside,
                $"BaseSize {baseSize}: the TextBox's top edge (y={top}) paints {edge:X8}, the same as its interior, " +
                $"so its border has rounded away (BorderThickness {box.BorderThickness}).");
        }
        finally
        {
            window.Close();
        }
    }

    // Bounds-checked: a coordinate outside the frame fails the test with its reason rather than reading
    // past the pixel buffer, which takes the whole test process down with it
    private static uint Pixel(WriteableBitmap frame, int x, int y)
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

        return (uint)pixel[0];
    }

}
