using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>
/// Geometry the theme's templates promise, measured on real themed controls in a headless window.
/// </summary>
public sealed class TemplateGeometryTests
{
    /// <summary>A scroll bar's arrows sit dead centre in their buttons, across the bar and along it.</summary>
    [AvaloniaTheory]
    [InlineData(Orientation.Vertical)]
    [InlineData(Orientation.Horizontal)]
    public void Scroll_bar_arrows_are_centred_in_their_buttons(Orientation orientation)
    {
        var bar = new ScrollBar
        {
            Orientation = orientation,
            AllowAutoHide = false,
            Minimum = 0,
            Maximum = 100,
            ViewportSize = 50,
            Width = orientation == Orientation.Vertical ? double.NaN : 240,
            Height = orientation == Orientation.Vertical ? 240 : double.NaN,
        };
        var window = Show(bar);
        try
        {
            foreach (var (button, icon) in new[] { ("PART_LineUpButton", "PART_LineUpIcon"), ("PART_LineDownButton", "PART_LineDownIcon") })
            {
                var host = bar.GetVisualDescendants().OfType<RepeatButton>().Single(b => b.Name == button);
                var glyph = bar.GetVisualDescendants().OfType<PathIcon>().Single(p => p.Name == icon);
                var hostCentre = Centre(host, window);
                var glyphCentre = Centre(glyph, window);

                Assert.True(Math.Abs(hostCentre.X - glyphCentre.X) <= 0.5,
                    $"{orientation} {icon}: centre x {glyphCentre.X:0.##} but its button's is {hostCentre.X:0.##}");
                Assert.True(Math.Abs(hostCentre.Y - glyphCentre.Y) <= 0.5,
                    $"{orientation} {icon}: centre y {glyphCentre.Y:0.##} but its button's is {hostCentre.Y:0.##}");
            }
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>
    /// A ColorView is an editor made of parts that each draw their own frame, so it draws none itself: a frame
    /// round them doubles every outline it meets, and at a large radius with no padding the two arcs part.
    /// </summary>
    [AvaloniaFact]
    public void A_color_view_draws_no_frame_of_its_own()
    {
        var view = new ColorView();
        var window = Show(view);
        try
        {
            var parts = new HashSet<Visual>(view.GetVisualDescendants().Where(v => v is ColorSpectrum or ColorSlider or ColorPreviewer or TextBox));
            var chrome = view.GetVisualDescendants()
                .Where(v => !parts.Any(p => p == v || p.IsVisualAncestorOf(v)))
                .OfType<Control>()
                .Where(c => c switch
                {
                    Border b => b.Background is not null || b.BorderThickness != default,
                    Panel p => p.Background is not null,
                    ContentPresenter cp => cp.Background is not null || cp.BorderThickness != default,
                    _ => false,
                })
                .Select(c => $"{c.GetType().Name}#{c.Name}")
                .ToList();

            Assert.True(chrome.Count == 0, "ColorView paints chrome of its own: " + string.Join(", ", chrome));
        }
        finally
        {
            window.Close();
        }
    }

    private static Window Show(Control content)
    {
        var window = new Window { Width = 600, Height = 600, Content = content };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        return window;
    }

    private static Point Centre(Visual visual, Visual root)
    {
        var origin = visual.TranslatePoint(new Point(0, 0), root)!.Value;
        return new Point(origin.X + visual.Bounds.Width / 2, origin.Y + visual.Bounds.Height / 2);
    }
}
