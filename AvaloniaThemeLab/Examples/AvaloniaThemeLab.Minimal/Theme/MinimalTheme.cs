using AngelSix.Mvvm;
using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Media;

namespace AvaloniaThemeLab.Minimal.Theme;

/// <summary>A custom theme built from scratch on <see cref="ThemeBase"/> with warm stone surfaces and an emerald/terracotta palette.</summary>
[Theme(FallbackHue = "Neutral")]
public partial class MinimalTheme : ThemeBase
{
    #region Defaults

    // What the theme falls back to: a plain reuse of a seed first, then the stage each part of a control defaults to

    /// <summary>The window canvas: the surface seed, or its DarkSeed in the dark palette.</summary>
    public virtual Color WindowBackground => SurfaceSeed;

    // Surfaces
    [StageDefault] internal virtual Stage CardBackground => Stage.Light10;

    [StageDefault] internal virtual Stage ButtonBackground => Stage.Light9;

    [StageDefault] internal virtual Stage ButtonHoverBackground => Stage.Light7;

    [StageDefault] internal virtual Stage ButtonPressedBackground => Stage.Light5;

    // Text
    [StageDefault(PinHue = "Neutral")] internal virtual Stage WindowText => Stage.Dark9;

    [StageDefault(PinHue = "Neutral")] internal virtual Stage MutedText => Stage.Dark2;

    [StageDefault] internal virtual Stage ButtonText => Stage.Dark9;

    // Lines
    [StageDefault] internal virtual Stage CardBorder => Stage.Light3;

    [StageDefault] internal virtual Stage ButtonBorder => Stage.Light1;

    #endregion Defaults

    #region Seeds

    [AccentHue, ColorRamp(HueAware = true, DarkSeed = "#DCD9D0")]
    [Observable] public virtual partial Color AccentNeutral { get; set; } = Color.Parse("#232625");

    [AccentHue, ColorRamp(HueAware = true)]
    [Observable] public virtual partial Color AccentPrimary { get; set; } = Color.Parse("#1B7F5C");

    [AccentHue, ColorRamp(HueAware = true)]
    [Observable] public virtual partial Color AccentDanger { get; set; } = Color.Parse("#C84B31");

    [AccentHue, ColorRamp(DarkSeed = "#171A19")]
    [Observable] public virtual partial Color AccentSurface { get; set; } = Color.Parse("#F4F1EA");

    #endregion Seeds

    #region Sizes

    // Every ladder grows from ThemeBase's BaseSize

    [SizeScale(Multipliers = [4, 8, 12, 20], Labels = ["Sm", "Md", "Lg", "Xl"])]
    public virtual double Spacing => BaseSize;

    [SizeScale(Multipliers = [12, 14, 22], Labels = ["Sm", "Md", "Lg"])]
    public virtual double FontSize => BaseSize;

    [SizeScale(Multipliers = [6, 12], Labels = ["Sm", "Md"], Types = [typeof(CornerRadius)], TypeSuffixes = [""])]
    public virtual double Radius => BaseSize;

    [SizeScale(Multipliers = [1, 2], Labels = ["Sm", "Md"], Types = [typeof(Thickness)], TypeSuffixes = [""], Floor = 1)]
    public virtual double Thickness => BaseSize;

    #endregion Sizes

    public virtual FontFamily FontFamily => AngelSix.ThemeEngine.Fonts.ThemeFonts.Inter;
}
