using AngelSix.ThemeEngine;
using AngelSix.Themes.Default;

namespace AvaloniaThemeLab.StarterTheme.Theme;

/// <summary>
/// Custom starter theme extending <see cref="DefaultTheme"/> and attaching <see cref="AppIconGallery"/>.
/// Override any token, seed color, or control file here without forking the default theme.
/// </summary>
[Theme]
public sealed partial class AppTheme : DefaultTheme
{
    #region Properties

    /// <inheritdoc />
    public override ThemeIconGallery Icons { get; } = new AppIconGallery();

    #endregion Properties
}
