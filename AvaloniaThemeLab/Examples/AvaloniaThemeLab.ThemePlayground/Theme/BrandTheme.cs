using AngelSix.ThemeEngine;
using AngelSix.Themes.Default;
using Avalonia.Media;

namespace AvaloniaThemeLab.ThemePlayground;

/// <summary>An end-user theme living downstream of the theme library.</summary>
[Theme(FallbackHue = "Neutral")]
public partial class BrandTheme : DefaultTheme
{
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentBrand => Color.Parse("#7C4DFF");
}
