using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// Large corner radii, drawn the way a person sees them. RadiusScale 4 puts a 48px radius on a 24px strip and a
/// 24px radius on a list with no padding, which is where Avalonia's own geometry stops agreeing with itself.
/// </summary>
public sealed class CornerRadiusTests
{
    /// <summary>
    /// A hue strip's colour fills the same rounded shape as the checkerboard behind it. Uncapped, Avalonia draws a
    /// stroked border's fill as an oval when the radius is over half the strip, so the colour stops short of the
    /// checkerboard's pill at each end.
    /// </summary>
    [AvaloniaFact]
    public void A_hue_strip_fills_its_whole_pill_at_a_large_radius()
    {
        var slider = new ColorSlider
        {
            Orientation = Orientation.Vertical,
            ColorModel = ColorModel.Hsva,
            ColorComponent = ColorComponent.Component1,
            IsAlphaVisible = false,
            IsPerceptive = true,
            Height = 256,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };

        Render(slider, 4, 1, frame =>
        {
            var origin = slider.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(slider)!)!.Value;
            // Inside a 12px pill cap, outside the oval the uncapped fill draws
            var (r, g, b) = Rgb(frame, (int)origin.X + 2, (int)origin.Y + 14);
            var saturation = Math.Max(r, Math.Max(g, b)) - Math.Min(r, Math.Min(g, b));

            Assert.True(saturation > 60,
                $"At (2,14) inside the strip's pill the colour strip leaves a grey pixel ({r},{g},{b}): its fill stops short of the pill.");
        });
    }

    /// <summary>
    /// The strip's colour reaches both of its ends at any radius. The strip's gradient is a bitmap stretched Uniform
    /// into its background box; a box even half a stroke smaller than the bitmap scales it down and leaves a band of
    /// checkerboard at each end. Sampled on the strip's centre line, 3px in from each end.
    /// </summary>
    [AvaloniaTheory]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(4.0)]
    public void A_hue_strip_is_coloured_to_both_ends(double radiusScale)
    {
        var slider = new ColorSlider
        {
            Orientation = Orientation.Vertical,
            ColorModel = ColorModel.Hsva,
            ColorComponent = ColorComponent.Component1,
            IsAlphaVisible = false,
            IsPerceptive = true,
            Height = 256,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };

        Render(slider, radiusScale, 1, frame =>
        {
            var origin = slider.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(slider)!)!.Value;
            var x = (int)(origin.X + slider.Bounds.Width / 2);
            foreach (var y in new[] { (int)origin.Y + 3, (int)(origin.Y + slider.Bounds.Height) - 4 })
            {
                var (r, g, b) = Rgb(frame, x, y);
                Assert.True(Math.Max(r, Math.Max(g, b)) - Math.Min(r, Math.Min(g, b)) > 60,
                    $"RadiusScale {radiusScale}: ({x},{y}) on the strip's centre line paints grey ({r},{g},{b}): the colour stops short of the end.");
            }
        });
    }

    /// <summary>
    /// A list's frame keeps its rounded corner when it has no padding: the scroller inside must not paint a square
    /// background over the frame's arc.
    /// </summary>
    [AvaloniaFact]
    public void A_list_keeps_its_corner_arc_with_no_padding()
    {
        var list = new ListBox
        {
            Width = 200,
            Height = 200,
            ItemsSource = new[] { "AutoCompleteBox", "Button", "ButtonSpinner" },
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };

        Render(list, 4, 0, frame =>
        {
            var origin = list.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(list)!)!.Value;
            var radius = list.CornerRadius.TopLeft;
            // The frame's stroke at 45 degrees round its top-left arc
            var offset = (int)Math.Round(radius - radius / Math.Sqrt(2));
            var arc = Rgb(frame, (int)origin.X + offset, (int)origin.Y + offset);
            var inside = Rgb(frame, (int)origin.X + 100, (int)origin.Y + 190);

            Assert.True(arc != inside,
                $"The list's top-left arc (r={radius}) at ({offset},{offset}) paints {arc}, the same as its interior: something square covers the frame's corner.");
        });
    }

    /// <summary>
    /// A live change in either direction always reaches the drawn frames: raise the radius past half the strip, lower
    /// it again, raise it again, and every frame carries exactly the theme's radius. A stale radius was the bug: the strip stuck at a pill
    /// after the radius came back down, and the spectrum's frame stuck square from its first, zero-size layout.
    /// </summary>
    [AvaloniaFact]
    public void A_live_radius_change_always_reaches_the_drawn_frame()
    {
        var view = new ColorView { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
        var window = new Window { Width = 600, Height = 600, Content = view };
        ThemeRegion.SetTheme(window, new DefaultTheme { RadiusScale = 1 });
        try
        {
            window.Show();
            Settle(window);

            Border Strip() => view.GetVisualDescendants().OfType<ColorSlider>().First().GetVisualDescendants().OfType<Border>().Single(b => b.Name == "Strip");
            Border SpectrumFrame() => view.GetVisualDescendants().OfType<ColorSpectrum>().Single().GetVisualDescendants().OfType<Border>().Single(b => b.Name == "BorderRectangle");

            var report = new List<string>();
            foreach (var scale in new[] { 1.0, 4.0, 0.5, 4.0, 1.0 })
            {
                // A new pinned theme each step: the engine follows live edits to the shared theme only, and editing
                // that re-evaluates every live window (see Render). The controls see the same live radius change
                ThemeRegion.SetTheme(window, new DefaultTheme { RadiusScale = scale });
                Settle(window);
                // RadiusMd 12 on the 24px strip, RadiusSm 6 on the 256px spectrum, both times the scale
                var strip = Strip();
                var frame = SpectrumFrame();
                if (strip.CornerRadius != new CornerRadius(12 * scale))
                    report.Add($"RadiusScale {scale}: strip radius {strip.CornerRadius}, theme says {12 * scale}");
                if (frame.CornerRadius != new CornerRadius(6 * scale))
                    report.Add($"RadiusScale {scale}: spectrum frame radius {frame.CornerRadius}, theme says {6 * scale}");
                var outline = view.GetVisualDescendants().OfType<ColorSlider>().First().GetVisualDescendants().OfType<Border>().Single(b => b.Name == "Outline");
                if (outline.CornerRadius != new CornerRadius(12 * scale))
                    report.Add($"RadiusScale {scale}: strip outline radius {outline.CornerRadius}, theme says {12 * scale}");
            }

            Assert.True(report.Count == 0, string.Join("; ", report));
        }
        finally
        {
            window.Close();
        }
    }

    // A ColorSlider paints its strip from a bitmap it builds off the UI thread after each size change, so until the
    // bitmap for its current size arrives the strip still shows the previous one, stretched to fit. Wait for it
    private static void AwaitStrips(Window window)
    {
        static bool current(ColorSlider slider) =>
            slider.Background is ImageBrush { Source: Bitmap bitmap }
            && Math.Abs(bitmap.PixelSize.Width - slider.Bounds.Width) <= 1
            && Math.Abs(bitmap.PixelSize.Height - slider.Bounds.Height) <= 1;

        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (!window.GetVisualDescendants().OfType<ColorSlider>().All(current) && DateTime.UtcNow < deadline)
        {
            Thread.Sleep(10);
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static void Settle(Window window)
    {
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
    }

    // A theme of the test's own, pinned to this window only: changing the shared theme re-evaluates every live
    // window's bindings, and a brush transition the accent resolver re-triggers can overflow the stack (ZoomProbe)
    private static void Render(Control control, double radiusScale, double spacingScale, Action<WriteableBitmap> check)
    {
        var window = new Window { Width = 400, Height = 400, Content = new Panel { Margin = new Thickness(20), Children = { control } } };
        ThemeRegion.SetTheme(window, new DefaultTheme { RadiusScale = radiusScale, SpacingScale = spacingScale });
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();
            AwaitStrips(window);
            // Not disposed: the headless platform owns the frame it hands back
            check(window.CaptureRenderedFrame()!);
        }
        finally
        {
            window.Close();
        }
    }

    // Bounds-checked: a coordinate outside the frame fails the test rather than reading past the buffer
    private static (int R, int G, int B) Rgb(WriteableBitmap frame, int x, int y)
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

        // Skia's headless frame is BGRA
        var value = (uint)pixel[0];
        return ((int)((value >> 16) & 0xFF), (int)((value >> 8) & 0xFF), (int)(value & 0xFF));
    }

}
