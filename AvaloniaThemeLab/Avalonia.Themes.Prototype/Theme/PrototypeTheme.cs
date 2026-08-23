using AngelSix.ThemeEngine;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// Includes the prototype theme in an application.
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
/// </remarks>
public class PrototypeTheme : ThemeStyles<DefaultTheme>
{
}
