using AngelSix.ThemeEngine;
using AngelSix.Themes.Default;

namespace AvaloniaThemeLab.Icons.Theme;

/// <summary>The app's theme: the default theme, plus a custom icon gallery.</summary>
[Theme(FallbackHue = "Neutral")]
public partial class IconsTheme : DefaultTheme
{
    public override AppIcons Icons => new();
}

/// <summary>
/// The app's icons. Deriving from <see cref="ThemeIconGallery"/> keeps every default icon
/// alive — <c>{icons:Close}</c> and friends still work — while the SVG folder and the
/// hand-written properties below add to it and redraw from it.
/// </summary>
[IconGallery(SvgFolder = "Icons/Svg")]
public partial class AppIcons : ThemeIconGallery
{
    /// <summary>Hand-written: one path string is the whole drawing.</summary>
    public virtual string Heart => "M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z";

    /// <summary>An alias: a hand-written name pointing at a generated one.</summary>
    public virtual string CoffeeCup => Coffee;

    /// <summary>Redrawing a default icon replaces it everywhere — this theme's templates included.</summary>
    public override string ChevronDown => "M5 8l7 7 7-7 2 2-9 9-9-9z";
}
