using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Themes.Default;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ViewModels;
using AvaloniaThemeLab.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The table, driven the way a person uses it, on a theme pinned to its own window.</summary>
public sealed class TableViewTests
{
    private static TableView Table(out TableViewColumn first)
    {
        first = new TableViewColumn { Header = "Command", Binding = new Binding(nameof(NativeMenuItem.Header)), Width = new GridLength(140) };
        var table = new TableView { Width = 384, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
        table.Columns.Add(first);
        table.Columns.Add(new TableViewColumn { Header = "Shortcut", Binding = new Binding(nameof(NativeMenuItem.Gesture)), Width = new GridLength(1, GridUnitType.Star) });
        table.ItemsSource = new[] { new NativeMenuItem { Header = "Zoom in" }, new NativeMenuItem { Header = "Zoom out" } };
        return table;
    }

    /// <summary>Dragging the grip on a column's right edge widens the column by the distance dragged.</summary>
    [AvaloniaTheory]
    [InlineData(1.0, 1.0)]
    [InlineData(0.0, 1.0)]
    [InlineData(0.0, 0.0)]
    public void Dragging_a_column_edge_resizes_the_column(double spacing, double controlHeight)
    {
        var table = Table(out var first);
        var window = Show(table, new DefaultTheme { SpacingScale = spacing, ControlHeightScale = controlHeight });
        try
        {
            var header = table.GetVisualDescendants().OfType<TableViewColumnHeader>().First();
            var grip = header.GetVisualDescendants().OfType<Thumb>().Single(t => t.Name == "PART_Resizer");
            Assert.True(grip.IsEffectivelyVisible && grip.IsEffectivelyEnabled, "The first column's resize grip is not live");
            Assert.True(grip.Bounds.Width >= 4, $"The resize grip is {grip.Bounds.Width:0.#}px wide: too thin to grab");

            var before = first.ActualWidth;
            var start = grip.TranslatePoint(new Point(grip.Bounds.Width / 2, grip.Bounds.Height / 2), window)!.Value;
            AwaitRendered(window, start);
            var hit = window.InputHitTest(start) as Visual;
            string Describe(Point p) => (window.InputHitTest(p) as Visual) is { } v ? $"{v.GetType().Name}#{(v as StyledElement)?.Name}" : "nothing";
            Assert.True(hit == grip || (hit is not null && grip.IsVisualAncestorOf(hit)),
                $"The pointer at {start} over the grip ({grip.Bounds}) lands on {Describe(start)}; left 3 {Describe(start + new Point(-3, 0))}, right 3 {Describe(start + new Point(3, 0))}; window {window.Bounds} client {window.ClientSize}");
            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            for (var dx = 10; dx <= 40; dx += 10)
            {
                window.MouseMove(start + new Point(dx, 0));
                Dispatcher.UIThread.RunJobs();
            }

            window.MouseUp(start + new Point(40, 0), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            Assert.True(Math.Abs(first.ActualWidth - (before + 40)) <= 1,
                $"Dragged the grip 40px: the column went from {before:0.#} to {first.ActualWidth:0.#}");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>
    /// In the gallery, the table the page says can be resized really resizes: drag the first column's edge in the
    /// TableView entry and the column widens by the distance dragged.
    /// </summary>
    [AvaloniaFact]
    public void The_gallery_table_resizes_when_its_column_edge_is_dragged()
    {
        var lab = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
        var shown = lab.SelectedEntry;
        // Tall enough that the whole entry is on screen: the pointer can only reach what is in view
        var window = new MainWindow { DataContext = lab, Width = 1200, Height = 3000 };
        ThemeRegion.SetTheme(window, new DefaultTheme());
        try
        {
            lab.SelectedEntry = lab.Catalogue.Entries.Single(e => e.Name == "TableView");
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            // The table under the "Drag a column edge to resize" caption, in the light pane
            var caption = window.GetVisualDescendants().OfType<TextBlock>().First(t => t.Text == "Drag a column edge to resize");
            var panel = (Panel)caption.GetVisualParent()!;
            var table = (TableView)panel.Children[panel.Children.IndexOf(caption) + 1];
            table.BringIntoView();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();

            var first = table.Columns[0];
            var header = table.GetVisualDescendants().OfType<TableViewColumnHeader>().First(h => h.Column == first);
            var grip = header.GetVisualDescendants().OfType<Thumb>().Single(t => t.Name == "PART_Resizer");
            var before = first.ActualWidth;
            var start = grip.TranslatePoint(new Point(grip.Bounds.Width / 2, grip.Bounds.Height / 2), window)!.Value;
            AwaitRendered(window, start);

            // What the pointer actually lands on at the grip's centre must be the grip itself
            var hit = window.InputHitTest(start) as Visual;
            Assert.True(hit == grip || (hit is not null && grip.IsVisualAncestorOf(hit)),
                $"The pointer over the grip at {start} lands on {hit?.GetType().Name}#{(hit as StyledElement)?.Name} under {string.Join(" < ", (hit as Visual)?.GetVisualAncestors().Take(6).Select(a => a.GetType().Name + "#" + (a as StyledElement)?.Name) ?? [])}, not the grip");

            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            for (var dx = 10; dx <= 40; dx += 10)
            {
                window.MouseMove(start + new Point(dx, 0));
                Dispatcher.UIThread.RunJobs();
            }

            window.MouseUp(start + new Point(40, 0), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            Assert.True(Math.Abs(first.ActualWidth - (before + 40)) <= 1,
                $"Dragged the gallery table's grip 40px: the column went from {before:0.#} to {first.ActualWidth:0.#}");
        }
        finally
        {
            window.Close();
            lab.SelectedEntry = shown;
        }
    }

    /// <summary>A table's frame keeps its bottom corner arcs with no spacing: nothing inside paints square over them.</summary>
    [AvaloniaFact]
    public void A_table_keeps_its_bottom_corner_arcs_with_no_spacing()
    {
        var table = Table(out _);
        table.Height = 200;
        var window = Show(table, new DefaultTheme { SpacingScale = 0, RadiusScale = 4 });
        try
        {
            var frame = window.CaptureRenderedFrame()!;
            var origin = table.TranslatePoint(new Point(0, 0), window)!.Value;
            var radius = table.CornerRadius.BottomLeft;
            var offset = (int)Math.Round(radius - radius / Math.Sqrt(2));
            var bottom = (int)(origin.Y + table.Bounds.Height) - 1 - offset;
            var inside = Pixel(frame, (int)(origin.X + table.Bounds.Width / 2), (int)(origin.Y + table.Bounds.Height) - (int)radius);
            foreach (var x in new[] { (int)origin.X + offset, (int)(origin.X + table.Bounds.Width) - 1 - offset })
                Assert.True(Pixel(frame, x, bottom) != inside,
                    $"The table's bottom arc (r={radius}) at ({x - (int)origin.X},{bottom - (int)origin.Y}) paints its interior colour: something square covers it.");
        }
        finally
        {
            window.Close();
        }
    }

    // The pointer is hit-tested against the last rendered frame; a fresh headless window may not have one yet, so
    // render until the point reaches something (a second at most), as a person only clicks what is on screen
    private static void AwaitRendered(Window window, Point point)
    {
        var deadline = DateTime.UtcNow.AddSeconds(1);
        while (window.InputHitTest(point) is null && DateTime.UtcNow < deadline)
        {
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(5);
        }
    }

    private static Window Show(Control content, DefaultTheme theme)
    {
        var window = new Window { Width = 600, Height = 400, Content = new Panel { Margin = new Thickness(20), Children = { content } } };
        ThemeRegion.SetTheme(window, theme);
        window.Show();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        // The pointer is hit-tested against the last rendered frame, so render one before driving input
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
        return window;
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
