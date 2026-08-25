using AngelSix.ThemeEngine;
using Avalonia.Media;
using Avalonia.Themes.Prototype;

namespace AvaloniaThemeLab;

/// <summary>
/// An end-user theme living downstream of the theme library: takes the prototype palette and adds
/// an accent hue of its own, which the library's control styles were compiled long before.
/// </summary>
[Theme(FallbackHue = "Neutral")]
public class BrandTheme : DefaultTheme
{
    public override string ThemeName => "Brand";

    [AccentHue] public virtual Color AccentBrand => Color.Parse("#7C4DFF");

    // The hue needs every stage the library's control themes ask for. Declaring a couple of
    // shades by hand is what left {colour:AccentBrush Light10} unresolved, so a Brand control
    // rendered with no fill and no border at all.
    protected virtual ColorRamp BrandRamp =>
        IsDark ? new MirroredColorRamp(AccentBrand) : new ColorRamp(AccentBrand);

    public virtual SolidColorBrush AccentBrandBrush => new(AccentBrand);
    public virtual Color AccentBrandLight1 => BrandRamp.Light1;
    public virtual Color AccentBrandLight2 => BrandRamp.Light2;
    public virtual Color AccentBrandLight3 => BrandRamp.Light3;
    public virtual Color AccentBrandLight4 => BrandRamp.Light4;
    public virtual Color AccentBrandLight5 => BrandRamp.Light5;
    public virtual Color AccentBrandLight6 => BrandRamp.Light6;
    public virtual Color AccentBrandLight7 => BrandRamp.Light7;
    public virtual Color AccentBrandLight8 => BrandRamp.Light8;
    public virtual Color AccentBrandLight9 => BrandRamp.Light9;
    public virtual Color AccentBrandLight10 => BrandRamp.Light10;
    public virtual Color AccentBrandDark1 => BrandRamp.Dark1;
    public virtual Color AccentBrandDark2 => BrandRamp.Dark2;
    public virtual Color AccentBrandDark3 => BrandRamp.Dark3;
    public virtual Color AccentBrandDark4 => BrandRamp.Dark4;
    public virtual Color AccentBrandDark5 => BrandRamp.Dark5;
    public virtual Color AccentBrandDark6 => BrandRamp.Dark6;
    public virtual Color AccentBrandDark7 => BrandRamp.Dark7;
    public virtual Color AccentBrandDark8 => BrandRamp.Dark8;
    public virtual Color AccentBrandDark9 => BrandRamp.Dark9;
    public virtual Color AccentBrandDark10 => BrandRamp.Dark10;

    public virtual SolidColorBrush AccentBrandLight1Brush => new(AccentBrandLight1);
    public virtual SolidColorBrush AccentBrandLight2Brush => new(AccentBrandLight2);
    public virtual SolidColorBrush AccentBrandLight3Brush => new(AccentBrandLight3);
    public virtual SolidColorBrush AccentBrandLight4Brush => new(AccentBrandLight4);
    public virtual SolidColorBrush AccentBrandLight5Brush => new(AccentBrandLight5);
    public virtual SolidColorBrush AccentBrandLight6Brush => new(AccentBrandLight6);
    public virtual SolidColorBrush AccentBrandLight7Brush => new(AccentBrandLight7);
    public virtual SolidColorBrush AccentBrandLight8Brush => new(AccentBrandLight8);
    public virtual SolidColorBrush AccentBrandLight9Brush => new(AccentBrandLight9);
    public virtual SolidColorBrush AccentBrandLight10Brush => new(AccentBrandLight10);
    public virtual SolidColorBrush AccentBrandDark1Brush => new(AccentBrandDark1);
    public virtual SolidColorBrush AccentBrandDark2Brush => new(AccentBrandDark2);
    public virtual SolidColorBrush AccentBrandDark3Brush => new(AccentBrandDark3);
    public virtual SolidColorBrush AccentBrandDark4Brush => new(AccentBrandDark4);
    public virtual SolidColorBrush AccentBrandDark5Brush => new(AccentBrandDark5);
    public virtual SolidColorBrush AccentBrandDark6Brush => new(AccentBrandDark6);
    public virtual SolidColorBrush AccentBrandDark7Brush => new(AccentBrandDark7);
    public virtual SolidColorBrush AccentBrandDark8Brush => new(AccentBrandDark8);
    public virtual SolidColorBrush AccentBrandDark9Brush => new(AccentBrandDark9);
    public virtual SolidColorBrush AccentBrandDark10Brush => new(AccentBrandDark10);
}
