using AngelSix.ThemeEngine;
using Avalonia.Styling;
using Avalonia.Themes.Prototype;

namespace AvaloniaThemeLab;

/// <summary>
/// The theme as a <c>Design.PreviewWith</c> block needs it: the prototype control themes, with
/// <see cref="BrandTheme"/> as the active token theme.
/// </summary>
/// <remarks>
/// <para>
/// A preview never runs <see cref="App"/>, so nothing has registered the application's token theme
/// by the time the block loads. <c>PrototypeTheme</c> notices there is no <see cref="ThemeContext"/>
/// and falls back to <c>DefaultTheme</c> — which declares no <c>Brand</c> hue, so every
/// <c>{colour:AccentBrush}</c> under <c>Accent.Kind="Brand"</c> resolves to nothing and the brand
/// styling previews as an unstyled button. Registering the real theme first is what makes a preview
/// of brand styling actually show the brand.
/// </para>
/// <para>
/// This is also the single place naming the underlying control theme for design-time. Swapping
/// <c>PrototypeTheme</c> for another library, or <c>BrandTheme</c> for another palette, is a change
/// here rather than in every preview block.
/// </para>
/// </remarks>
public class BrandPreviewTheme : Styles
{
    public BrandPreviewTheme()
    {
        // Constructing a ThemeContext registers it as the resolver, so this has to happen before
        // PrototypeTheme is built — otherwise its own DefaultTheme fallback gets there first.
        _ = new ThemeContext(new BrandTheme());

        Add(new PrototypeTheme());
    }
}
