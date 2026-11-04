using AngelSix.ThemeEngine.Controls;
using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Path = Avalonia.Controls.Shapes.Path;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// Each size ladder moves only what it names: control height moves hit targets and control faces, icon size moves
/// glyphs, spacing moves gaps. Every test pins a theme of its own to its window and never edits the shared one.
/// </summary>
public sealed class SizeLadderTests
{
    /// <summary>A command bar's buttons fit their own icon and label at any control height; none collapses or wraps.</summary>
    [AvaloniaFact]
    public void Command_bar_buttons_fit_their_content_at_zero_control_height()
    {
        var bar = new CommandBar
        {
            PrimaryCommands =
            {
                new CommandBarButton { Label = "Rest", Icon = new PathIcon { Data = Geometry() } },
                new CommandBarToggleButton { Label = "Done", IsChecked = true, Icon = new PathIcon { Data = Geometry() } },
            },
        };

        Show(bar, new DefaultTheme { ControlHeightScale = 0 }, window =>
        {
            foreach (var button in bar.GetVisualDescendants().OfType<Button>().Where(b => b is CommandBarButton or CommandBarToggleButton))
            {
                var label = button.GetVisualDescendants().OfType<TextBlock>().Single(t => t.Name == "PART_Label");
                label.Measure(Size.Infinity);
                var oneLine = label.DesiredSize;
                Assert.True(button.Bounds.Width >= oneLine.Width,
                    $"{button.GetType().Name} '{label.Text}' is {button.Bounds.Width:0.##} wide, narrower than its label's {oneLine.Width:0.##}");
                Assert.True(label.Bounds.Height <= oneLine.Height + 0.5,
                    $"{button.GetType().Name} '{label.Text}' label is {label.Bounds.Height:0.##} tall, more than one line ({oneLine.Height:0.##}): it wrapped");
            }
        });
    }

    /// <summary>A calendar's frame keeps its top corner arcs with no padding: its header must not paint square over them.</summary>
    [AvaloniaFact]
    public void Calendar_keeps_its_top_corner_arcs_with_no_padding()
    {
        var calendar = new CalendarView { Width = 300, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

        Show(calendar, new DefaultTheme { SpacingScale = 0, RadiusScale = 4 }, window =>
        {
            var frame = window.CaptureRenderedFrame()!;
            var origin = calendar.TranslatePoint(new Point(0, 0), window)!.Value;
            var radius = calendar.CornerRadius.TopLeft;
            var offset = (int)Math.Round(radius - radius / Math.Sqrt(2));
            foreach (var x in new[] { (int)origin.X + offset, (int)(origin.X + calendar.Bounds.Width) - 1 - offset })
            {
                var arc = Pixel(frame, x, (int)origin.Y + offset);
                var inside = Pixel(frame, (int)(origin.X + calendar.Bounds.Width / 2), (int)origin.Y + (int)radius);
                Assert.True(arc != inside,
                    $"The calendar's top arc (r={radius}) at ({x - (int)origin.X},{offset}) paints {arc:X8}, its interior colour: something square covers it.");
            }
        });
    }

    /// <summary>A pager's dots are glyphs: they stay drawn with no spacing at all.</summary>
    [AvaloniaFact]
    public void Pager_dots_stay_drawn_with_no_spacing()
    {
        var pager = new PipsPager { NumberOfPages = 3, SelectedPageIndex = 0 };

        Show(pager, new DefaultTheme { SpacingScale = 0 }, _ =>
        {
            var dots = pager.GetVisualDescendants().OfType<Ellipse>().Where(e => e.Name == "Pip").ToList();
            Assert.Equal(3, dots.Count);
            Assert.All(dots, dot => Assert.True(dot.Bounds.Width > 0 && dot.Bounds.Height > 0, $"A pip is {dot.Bounds.Size} with no spacing"));
        });
    }

    /// <summary>A pager's arrows and a colour strip are hit targets: they grow and shrink with control height.</summary>
    [AvaloniaTheory]
    [InlineData(1.0)]
    [InlineData(2.0)]
    public void Pager_arrows_and_colour_strips_follow_control_height(double scale)
    {
        var pager = new PipsPager { NumberOfPages = 3, IsPreviousButtonVisible = true, IsNextButtonVisible = true };
        var slider = new ColorSlider { Width = 200, ColorModel = ColorModel.Hsva, ColorComponent = ColorComponent.Component1 };
        var content = new StackPanel { Children = { pager, slider } };

        Show(content, new DefaultTheme { ControlHeightScale = scale }, _ =>
        {
            // ControlHeightSm (32) three quarters: 24 at scale 1
            var expected = 24 * scale;
            var arrow = pager.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "PART_PreviousButton");
            Assert.Equal(expected, arrow.Bounds.Height, 3);
            Assert.Equal(expected, slider.Bounds.Height, 3);
        });
    }

    /// <summary>A drawer's hamburger button always contains its icon, centred, at any control height or spacing.</summary>
    [AvaloniaTheory]
    [InlineData(1.0, 1.0)]
    [InlineData(0.0, 0.0)]
    [InlineData(1.0, 3.0)]
    public void Drawer_hamburger_contains_its_icon_centred(double controlHeight, double spacing)
    {
        var drawer = new DrawerPage { Width = 360, Height = 160, Drawer = new ContentPage(), Content = new ContentPage() };

        Show(drawer, new DefaultTheme { ControlHeightScale = controlHeight, SpacingScale = spacing }, window =>
        {
            foreach (var button in drawer.GetVisualDescendants().OfType<ToggleButton>().Where(b => b.IsEffectivelyVisible && b.Name is "PART_PaneButton" or "PART_CompactPaneToggle"))
                AssertCentredInside(button, button.GetVisualDescendants().OfType<PathIcon>().First(), window);
        });
    }

    /// <summary>A drawn window caption button (the settings window's close) centres its icon and never crops it.</summary>
    [AvaloniaTheory]
    [InlineData(1.0)]
    [InlineData(0.0)]
    public void Caption_buttons_centre_their_icon(double controlHeight)
    {
        var theme = (Avalonia.Styling.ControlTheme)Application.Current!.FindResource("DefaultDrawnCaptionButton")!;
        var icon = new PathIcon { Data = Geometry() };
        var button = new Button { Theme = theme, Content = icon };

        Show(button, new DefaultTheme { ControlHeightScale = controlHeight }, window => AssertCentredInside(button, icon, window));
    }

    /// <summary>An expander's chevron button always contains its chevron, centred, at any control height.</summary>
    [AvaloniaTheory]
    [InlineData(1.0)]
    [InlineData(0.0)]
    public void Expander_chevron_contains_its_glyph(double controlHeight)
    {
        var expander = new Expander { Header = "Header", Content = "Content", IsExpanded = true, Width = 300 };

        Show(expander, new DefaultTheme { ControlHeightScale = controlHeight }, window =>
        {
            var chevron = expander.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "ExpandCollapseChevronBorder");
            AssertCentredInside(chevron, chevron.GetVisualDescendants().OfType<Control>().First(c => c is PathIcon or Path), window);
        });
    }

    /// <summary>A toggle switch's track is a control face: it grows and shrinks with control height.</summary>
    [AvaloniaTheory]
    [InlineData(1.0)]
    [InlineData(2.0)]
    public void Toggle_switch_follows_control_height(double scale)
    {
        var toggle = new ToggleSwitch { IsChecked = true };

        Show(toggle, new DefaultTheme { ControlHeightScale = scale }, _ =>
        {
            // The track is 40x20 at the default scale
            var track = toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "OuterBorder");
            Assert.Equal(40 * scale, track.Bounds.Width, 3);
            Assert.Equal(20 * scale, track.Bounds.Height, 3);
        });
    }

    /// <summary>Every scroll bar is exactly its theme thickness, standing alone or in a scroller, in either orientation.</summary>
    [AvaloniaTheory]
    [InlineData(1.0)]
    [InlineData(0.5)]
    public void Scroll_bars_are_one_thickness_everywhere(double iconScale)
    {
        var vertical = new ScrollBar { Orientation = Avalonia.Layout.Orientation.Vertical, AllowAutoHide = false, Height = 100, Maximum = 100, ViewportSize = 50 };
        var horizontal = new ScrollBar { Orientation = Avalonia.Layout.Orientation.Horizontal, AllowAutoHide = false, Width = 200, Maximum = 100, ViewportSize = 50 };
        // A wide caption beside the vertical bar, as the gallery lays them out: the bar must not stretch to it
        var column = new StackPanel { Children = { vertical, new TextBlock { Text = "A long caption under the bar" } } };
        var content = new StackPanel { Children = { horizontal, column } };

        Show(content, new DefaultTheme { IconSizeScale = iconScale }, _ =>
        {
            // IconSizeMd (16) and a quarter: 20 at the default scale
            var thickness = 20 * iconScale;
            Assert.Equal(thickness, vertical.Bounds.Width, 3);
            Assert.Equal(thickness, horizontal.Bounds.Height, 3);
        });
    }

    private static void AssertCentredInside(Visual host, Visual glyph, Window window)
    {
        // Centres mapped point to point, so a rotated glyph (a turned chevron) is measured where it is drawn
        var hc = host.TranslatePoint(new Point(host.Bounds.Width / 2, host.Bounds.Height / 2), window)!.Value;
        var gc = glyph.TranslatePoint(new Point(glyph.Bounds.Width / 2, glyph.Bounds.Height / 2), window)!.Value;
        Assert.True(host.Bounds.Width >= glyph.Bounds.Width - 0.5 && host.Bounds.Height >= glyph.Bounds.Height - 0.5,
            $"{host.GetType().Name}#{host.Name} is {host.Bounds.Size}, smaller than its {glyph.Bounds.Size} glyph");
        Assert.True(Math.Abs(hc.X - gc.X) <= 0.5 && Math.Abs(hc.Y - gc.Y) <= 0.5,
            $"{host.GetType().Name}#{host.Name} centre {hc} but its glyph's is {gc}");
    }

    private static Avalonia.Media.Geometry Geometry() => Avalonia.Media.Geometry.Parse("M0,0 L10,0 L10,10 Z");

    private static void Show(Control content, DefaultTheme theme, Action<Window> check)
    {
        content.HorizontalAlignment = HorizontalAlignment.Left;
        content.VerticalAlignment = VerticalAlignment.Top;
        var window = new Window { Width = 600, Height = 600, Content = new Panel { Margin = new Thickness(20), Children = { content } } };
        ThemeRegion.SetTheme(window, theme);
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();
            check(window);
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
