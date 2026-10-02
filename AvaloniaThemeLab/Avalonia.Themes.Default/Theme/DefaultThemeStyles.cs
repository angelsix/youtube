using AngelSix.ThemeEngine;
using AngelSix.ThemeEngine.Generated;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Avalonia.Themes.Default;

/// <summary>
/// Includes the default theme in an application.
/// </summary>
/// <remarks>
/// <para>
/// There is no markup behind this. <see cref="ThemeStyles{TTheme}"/> merges every control theme
/// dictionary the assembly ships, from a list generated at build time, so the theme carries no entry
/// Styles file and no hand-kept set of includes — adding a control is adding its .axaml, nothing more.
/// </para>
/// <para>
/// The type argument names the token theme to fall back on when no host has registered a
/// <see cref="ThemeContext"/> — design-time previews and non-DI hosts. A DI-provided context always
/// wins; this only fills the gap when nothing else has.
/// </para>
/// <para>
/// The one style it adds carries <see cref="DefaultTheme.ControlClipToBounds"/> to every UserControl. Every
/// themed control gets that value from a setter in its own ControlTheme, but an app's pages are UserControl
/// subclasses, and a ControlTheme is found by exact type, so no theme can reach them.
/// </para>
/// </remarks>
public class DefaultThemeStyles : ThemeStyles<DefaultTheme>
{
    public DefaultThemeStyles()
    {
        // A page's local ClipToBounds, or a consumer Style nearer the page, still wins over this
        Add(new Style(selector => selector.Is<UserControl>())
        {
            Setters = { new Setter(Visual.ClipToBoundsProperty, new ControlClipToBoundsExtension().ProvideValue(null!)) },
        });
    }
}
