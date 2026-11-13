using AngelSix.ThemeEngine.Generated;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>A border never rounds away to nothing when BaseSize shrinks.</summary>
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
        // A theme of the test's own, pinned to this window only.
        TestTheme.With(t => { t.BaseSize = baseSize; });
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
