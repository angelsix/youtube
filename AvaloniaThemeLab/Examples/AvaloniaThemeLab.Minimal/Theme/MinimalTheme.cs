using AngelSix.Mvvm;
using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Media;

namespace AvaloniaThemeLab.Minimal.Theme;

/// <summary>The minimal example's design tokens.</summary>
[Theme(FallbackHue = "Neutral")]
public partial class MinimalTheme : ThemeBase
{
    #region Palette

    [AccentHue, ColorRamp(HueAware = true, DarkSeed = "#D1D1D1")]
    [Observable] public virtual partial Color AccentNeutral { get; set; } = Color.Parse("#2E2E2E");

    [AccentHue, ColorRamp(HueAware = true)]
    [Observable] public virtual partial Color AccentPrimary { get; set; } = Color.Parse("#2F6FEB");

    [AccentHue, ColorRamp(HueAware = true)]
    [Observable] public virtual partial Color AccentDanger { get; set; } = Color.Parse("#D0453A");

    [AccentHue, ColorRamp(DarkSeed = "#1E1E1E")]
    [Observable] public virtual partial Color AccentSurface { get; set; } = Color.Parse("#FAFAFA");

    #endregion Palette

    #region Roles

    /// <summary>The window canvas: the surface seed, or its DarkSeed in the dark palette.</summary>
    public virtual Color WindowBackground => SurfaceSeed;

    [AccentRole(PinHue = "Neutral")] internal virtual RampStage WindowText => RampStage.Dark9;

    [AccentRole] internal virtual RampStage ButtonText => RampStage.Dark9;

    [AccentRole] internal virtual RampStage ButtonBorder => RampStage.Light1;

    [AccentRole] internal virtual RampStage ButtonBackground => RampStage.Light10;

    [AccentRole] internal virtual RampStage ButtonHoverBackground => RampStage.Light8;

    [AccentRole] internal virtual RampStage ButtonPressedBackground => RampStage.Light6;

    #endregion Roles

    #region Sizes

    // Every ladder grows from ThemeBase's BaseSize

    [SizeScale(Multipliers = [4, 8, 12, 16], Labels = ["Sm", "Md", "Lg", "Xl"])]
    public virtual double Spacing => BaseSize;

    [SizeScale(Multipliers = [12, 14, 20], Labels = ["Sm", "Md", "Lg"])]
    public virtual double FontSize => BaseSize;

    [SizeScale(Multipliers = [4, 8], Labels = ["Sm", "Md"], Types = [typeof(CornerRadius)], TypeSuffixes = [""])]
    public virtual double Radius => BaseSize;

    [SizeScale(Multipliers = [1, 2], Labels = ["Sm", "Md"], Types = [typeof(Thickness)], TypeSuffixes = [""], Floor = 1)]
    public virtual double Thickness => BaseSize;

    #endregion Sizes

    public virtual FontFamily FontFamily => AngelSix.ThemeEngine.Fonts.ThemeFonts.Inter;
}
