using AngelSix.ThemeEngine;
using AngelSix.Themes.Default;

namespace AvaloniaThemeLab.BuildingWidgets.Theme;

/// <summary>
/// Prototype theme extending <see cref="DefaultTheme"/> with <see cref="PrototypeIconGallery"/>.
/// </summary>
[Theme]
public sealed partial class PrototypeTheme : DefaultTheme
{
    #region Properties

    /// <inheritdoc />
    public override ThemeIconGallery Icons { get; } = new PrototypeIconGallery();

    #endregion Properties
}
