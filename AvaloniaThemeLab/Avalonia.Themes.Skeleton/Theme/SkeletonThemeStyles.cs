using AngelSix.ThemeEngine;

namespace Avalonia.Themes.Skeleton.Theme;

/// <summary>
/// The theme's entry point: <c>&lt;theme:SkeletonThemeStyles /&gt;</c> in <c>App.axaml</c>. It merges every
/// ControlTheme dictionary in this project (Controls/*.axaml) and, when no host has registered a
/// <see cref="ThemeContext"/>, registers a <see cref="SkeletonTheme"/> for the markup extensions to read.
/// </summary>
public class SkeletonThemeStyles : ThemeStyles<SkeletonTheme>
{
}
