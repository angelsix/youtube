using AngelSix.ThemeEngine;
using Avalonia.Media;
using Avalonia.Themes.Prototype;

namespace AvaloniaThemeLab;

/// <summary>
/// An end-user theme living downstream of the theme library: takes the prototype palette and adds
/// an accent hue of its own, which the library's control styles were compiled long before.
/// </summary>
/// <remarks>
/// The hue needs every stage the library's control themes ask for. Declaring a couple of shades by
/// hand is what used to leave <c>{colour:AccentBrush Light10}</c> unresolved, so a Brand control
/// rendered with no fill and no border at all. <c>[ColourRamp]</c> emits the whole family, so
/// adding a hue really is one property.
/// </remarks>
[Theme(FallbackHue = "Neutral")]
public partial class BrandTheme : DefaultTheme
{
    public override string ThemeName => "Brand";

    [AccentHue, ColourRamp] public virtual Color AccentBrand => Color.Parse("#7C4DFF");
}
