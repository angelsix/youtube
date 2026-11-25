using AngelSix.ThemeEngine;
using AngelSix.Themes.Default;

namespace AvaloniaThemeLab.StarterTheme.Theme;

/// <summary>
/// Custom icon gallery extending <see cref="ThemeIconGallery"/> with app-specific glyphs.
/// Every built-in icon from the default theme is inherited automatically.
/// </summary>
[IconGallery]
public partial class AppIconGallery : ThemeIconGallery
{
    /// <summary>Four-point sparkle glyph, available in XAML as <c>{icons:Sparkle}</c>.</summary>
    public virtual string Sparkle =>
        "M 12 2 L 14.4 9.6 L 22 12 L 14.4 14.4 L 12 22 L 9.6 14.4 L 2 12 L 9.6 9.6 Z";
}
