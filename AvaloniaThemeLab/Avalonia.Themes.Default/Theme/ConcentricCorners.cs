using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Avalonia.Themes.Default;

/// <summary>
/// A corner radius concentric with a control's rounded outline, for a part drawn inside or around it:
/// <c>{default:ConcentricCorners Radius={TemplateBinding CornerRadius}, Thickness={TemplateBinding BorderThickness}, Left=True}</c>.
/// </summary>
/// <remarks>
/// <para>
/// Avalonia puts a Border's CornerRadius on the middle of its stroke (GeometryBuilder.cs, "the corner radius is
/// defined to be the middle of the stroke"; BorderRenderHelper strokes the rect deflated by t/2 with the full
/// radius). So the stroke's inner edge, where a child is arranged, has radius r - t/2, and a square child paints
/// over the stroke at every corner. A part whose bounds sit <c>d</c> inside the control's bounds, and which strokes
/// its own outline <c>T</c> wide, follows the control's curve with radius <c>r + t/2 - d - T/2</c>.
/// </para>
/// <para>
/// <see cref="Inset"/> defaults to <see cref="Thickness"/>: a child of the control's frame. A ring drawn outside the
/// control passes its own negative Margin. A corner the control does not round stays square, and the flags pick
/// the corners the part actually touches, as with the engine's <c>{size:Corners}</c>. Theme-local for now; it is a
/// candidate for the engine.
/// </para>
/// </remarks>
public class ConcentricCorners : MarkupExtension
{
    #region Properties

    // The control's own CornerRadius, middle of its stroke
    public BindingBase? Radius { get; set; }

    // The control's BorderThickness
    public BindingBase? Thickness { get; set; }

    // Where the part's bounds sit inside the control's bounds; negative outside. Defaults to Thickness
    public BindingBase? Inset { get; set; }

    // The part's own BorderThickness, when it strokes an outline of its own
    public BindingBase? Stroke { get; set; }

    // Individual corners
    public bool TopLeft { get; set; }
    public bool TopRight { get; set; }
    public bool BottomRight { get; set; }
    public bool BottomLeft { get; set; }

    // Convenience pairs
    public bool Top { set { TopLeft = TopRight = value; } }
    public bool Bottom { set { BottomLeft = BottomRight = value; } }
    public bool Left { set { TopLeft = BottomLeft = value; } }
    public bool Right { set { TopRight = BottomRight = value; } }
    public bool All { set { TopLeft = TopRight = BottomRight = BottomLeft = value; } }

    #endregion Properties

    #region Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var mask = (TopLeft ? 1 : 0) | (TopRight ? 2 : 0) | (BottomRight ? 4 : 0) | (BottomLeft ? 8 : 0);
        var none = new Binding { Source = default(Thickness) };
        return new MultiBinding
        {
            Converter = ConcentricConverter.Instance,
            ConverterParameter = (mask, Inset is null),
            Bindings =
            {
                Radius ?? new Binding { Source = default(CornerRadius) },
                Thickness ?? none,
                Inset ?? none,
                Stroke ?? none,
            },
        };
    }

    private sealed class ConcentricConverter : IMultiValueConverter
    {
        public static readonly ConcentricConverter Instance = new();

        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            var (mask, insetIsThickness) = parameter is ValueTuple<int, bool> p ? p : (0, true);
            var r = values.Count > 0 && values[0] is CornerRadius c ? c : default;
            var t = values.Count > 1 && values[1] is Thickness th ? th : default;
            var d = insetIsThickness ? t : values.Count > 2 && values[2] is Thickness inset ? inset : default;
            var s = values.Count > 3 && values[3] is Thickness stroke ? stroke : default;

            // A corner's offsets are the mean of its two sides; on a uniform outline that is exact
            double corner(int bit, double radius, double a, double b, double da, double db, double sa, double sb) =>
                (mask & bit) == 0 || radius <= 0 ? 0 : Math.Max(0, radius + (a + b) / 4 - (da + db) / 2 - (sa + sb) / 4);

            return new CornerRadius(
                corner(1, r.TopLeft, t.Left, t.Top, d.Left, d.Top, s.Left, s.Top),
                corner(2, r.TopRight, t.Right, t.Top, d.Right, d.Top, s.Right, s.Top),
                corner(4, r.BottomRight, t.Right, t.Bottom, d.Right, d.Bottom, s.Right, s.Bottom),
                corner(8, r.BottomLeft, t.Left, t.Bottom, d.Left, d.Bottom, s.Left, s.Bottom));
        }
    }

    #endregion Methods
}
