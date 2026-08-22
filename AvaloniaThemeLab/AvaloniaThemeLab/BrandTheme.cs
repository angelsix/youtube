using AngelSix.ThemeEngine;
using Avalonia.Media;
using Avalonia.Themes.Prototype;

namespace AvaloniaThemeLab;

/// <summary>
/// An end-user theme living downstream of the theme library: takes the prototype palette and adds
/// an accent hue of its own, which the library's control styles were compiled long before.
/// </summary>
[Theme]
public class BrandTheme : DefaultTheme
{
    public override string ThemeName => "Brand";

    public virtual Color AccentBrand => Color.Parse("#7C4DFF");

    public virtual SolidColorBrush AccentBrandBrush => new(AccentBrand);
    public virtual SolidColorBrush AccentBrandDark1Brush => new(BrandShade(1));
    public virtual SolidColorBrush AccentBrandDark2Brush => new(BrandShade(2));
    public virtual SolidColorBrush AccentBrandHoverOverlayBrush => BrandOverlay(0.08);
    public virtual SolidColorBrush AccentBrandPressedOverlayBrush => BrandOverlay(0.20);

    private Color BrandShade(int shade)
    {
        var hsl = AccentBrand.ToHsl();
        var steps = new[] { 28.5, 49.0 };
        return new HslColor(hsl.A, hsl.H, hsl.S, hsl.L - (steps[shade - 1] / 255.0)).ToRgb();
    }

    private SolidColorBrush BrandOverlay(double opacity)
    {
        var hsl = AccentBrand.ToHsl();
        return new SolidColorBrush(new HslColor(opacity, hsl.H, hsl.S, hsl.L).ToRgb());
    }
}
