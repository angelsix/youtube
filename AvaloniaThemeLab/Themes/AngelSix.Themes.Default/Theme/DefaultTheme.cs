using System;
using AngelSix.Mvvm;
using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;

namespace AngelSix.Themes.Default;

/// <summary>The Default theme: its tokens, and the styles an app includes as <c>&lt;DefaultTheme /&gt;</c>.</summary>
[Theme(FallbackHue = "Neutral")]
public partial class DefaultTheme : ThemeBase
{
    #region Palette

    // One seed per hue; the engine derives every stage of its ramp, light and dark

    [AccentHue]
    [ColorRamp(OverlayLevels = [0.08, 0.16, 0.32], DarkSeed = "#D1D1D1", HueAware = true)]
    [Observable] public virtual partial Color AccentNeutral { get; set; } = Color.Parse("#2E2E2E");

    [AccentHue, ColorRamp(HueAware = true)] [Observable] public virtual partial Color AccentPrimary { get; set; } = Color.Parse("#5BA3C9");
    [AccentHue, ColorRamp(HueAware = true)] [Observable] public virtual partial Color AccentSuccess { get; set; } = Color.Parse("#6DB87E");
    [AccentHue, ColorRamp(HueAware = true)] [Observable] public virtual partial Color AccentWarning { get; set; } = Color.Parse("#E89A3C");
    [AccentHue, ColorRamp(HueAware = true)] [Observable] public virtual partial Color AccentFavourite { get; set; } = Color.Parse("#FFC400");
    [AccentHue, ColorRamp(HueAware = true)] [Observable] public virtual partial Color AccentError { get; set; } = Color.Parse("#D47A7A");
    [AccentHue, ColorRamp(HueAware = true)] [Observable] public virtual partial Color AccentInfo { get; set; } = Color.Parse("#4B5BE0");
    [AccentHue, ColorRamp(HueAware = true)] [Observable] public virtual partial Color AccentDestructive { get; set; } = Color.Parse("#A02A80");
    [AccentHue, ColorRamp(HueAware = true)] [Observable] public virtual partial Color AccentSubtle { get; set; } = Color.Parse("#B088C8");

    /// <summary>The application canvas.</summary>
    [AccentHue, ColorRamp(DarkSeed = "#222222")]
    [Observable] public virtual partial Color AccentSurface { get; set; } = Color.Parse("#fdfdfd");

    /// <summary>The focus ring: the one colour that is not a hue.</summary>
    [Observable] public virtual partial Color AccentFocus { get; set; } = Color.Parse("#bf4aF9");

    /// <summary>The canvas colour for the current palette.</summary>
    public virtual Color SurfaceDefault => SurfaceSeed;

    #endregion Palette

    #region Roles

    // Each role names one stage of the control's own hue, so it follows Accent.Kind and the palette

    // Surfaces
    [AccentRole] protected internal virtual RampStage BackgroundDefault => RampStage.Light10;
    [AccentRole] protected internal virtual RampStage SurfaceBorderless => RampStage.Light9;
    [AccentRole] protected internal virtual RampStage HoverBackgroundDefault => RampStage.Light8;
    [AccentRole] protected internal virtual RampStage PressedBackgroundDefault => RampStage.Light7;
    [AccentRole] protected internal virtual RampStage SelectedBackgroundDefault => RampStage.Light6;
    [AccentRole] protected internal virtual RampStage SelectedBackgroundHover => RampStage.Light5;
    [AccentRole] protected internal virtual RampStage SelectedBackgroundPressed => RampStage.Light4;

    // Text
    [AccentRole] protected internal virtual RampStage TextDefault => RampStage.Dark9;
    [AccentRole] protected internal virtual RampStage HoverTextDefault => RampStage.Dark7;
    [AccentRole] protected internal virtual RampStage DimTextDefault => RampStage.Dark2;

    /// <summary>Accent-coloured text and marks: a link, a caption, an error message, a tab's selection bar.</summary>
    [AccentRole] protected internal virtual RampStage AccentInkDefault => RampStage.Dark3;

    // Lines
    [AccentRole] protected internal virtual RampStage BorderDefault => RampStage.Light1;
    [AccentRole] protected internal virtual RampStage AccentDividerDefault => RampStage.Light7;

    /// <summary>The accent's own line and glyph: a frame, a caret, a chevron, a dot, a knob.</summary>
    [AccentRole] protected internal virtual RampStage AccentLineDefault => RampStage.Dark1;

    /// <summary>A focused accented control's border while <see cref="FocusColorOnAccents"/> is off.</summary>
    [AccentRole(NeutralHue = "Neutral", FocusColour = nameof(AccentFocus), FocusSwitch = nameof(FocusColorOnAccents))]
    protected internal virtual RampStage AccentFocusBorder => RampStage.Dark1;

    // Solid fills: a checked, toggled or starred face; a bright seed (yellow) takes the selected tint instead
    [AccentRole(BrightStage = nameof(SelectedBackgroundDefault))] protected internal virtual RampStage SolidFillDefault => RampStage.Dark3;
    [AccentRole(BrightStage = nameof(SelectedBackgroundHover))] protected internal virtual RampStage SolidFillHover => RampStage.Dark4;
    [AccentRole(BrightStage = nameof(SelectedBackgroundPressed))] protected internal virtual RampStage SolidFillPressed => RampStage.Dark5;

    /// <summary>Ink on a solid fill, picked by contrast against the fill.</summary>
    [AccentRole(ContrastAgainst = nameof(SolidFillDefault), Alternate = nameof(TextDefault))]
    protected internal virtual RampStage OnSolidFillInkDefault => RampStage.Light10;

    #endregion Roles

    #region Sizes

    // Every ladder grows from ThemeBase's BaseSize, and each also gets its own {Name}Scale, for resizing it alone

    [SizeScale(Multipliers = [2, 4, 8, 12, 16], Scalable = true)]
    public virtual double Spacing => BaseSize;

    [SizeScale(Multipliers = [32, 40, 48], Scalable = true)]
    public virtual double ControlHeight => BaseSize;

    [SizeScale(Multipliers = [6, 12, 24, 32], Types = [typeof(CornerRadius), typeof(double)], TypeSuffixes = ["", "Double"], Scalable = true)]
    public virtual double Radius => BaseSize;

    /// <summary>Border thickness ladder. Never below one pixel.</summary>
    [SizeScale(Multipliers = [1, 2, 3, 4, 6], Types = [typeof(Thickness), typeof(double)], TypeSuffixes = ["", "Double"], Floor = 1, Scalable = true)]
    public virtual double Thickness => BaseSize;

    [SizeScale(Multipliers = [12, 16, 24, 32], Scalable = true)]
    public virtual double IconSize => BaseSize;

    [SizeScale(Multipliers = [256, 320, 384], Labels = ["Sm", "Md", "Lg"])]
    public virtual double SurfaceWidth => BaseSize;

    /// <summary>The narrowest an interactive control may be, so a short label still gives a usable hit target.</summary>
    public virtual double ControlMinWidth => 64 * BaseSize;

    #endregion Sizes

    #region Typography

    [FontWeights(Weights = [FontWeight.Normal, FontWeight.Medium, FontWeight.SemiBold, FontWeight.Bold], Labels = ["Regular", "Medium", "SemiBold", "Bold"])]
    [Observable] public virtual partial FontFamily FontFamily { get; set; } = AngelSix.ThemeEngine.Fonts.ThemeFonts.Inter;

    [SizeScale(Multipliers = [12, 14, 16, 20, 24], Scalable = true)]
    public virtual double FontSize => BaseSize;

    #endregion Typography

    #region Motion

    /// <summary>Animation timing, in milliseconds: AnimationFastMs, AnimationNormalMs and AnimationSlowMs.</summary>
    [SizeScale(Multipliers = [0.5, 1, 2], Labels = ["FastMs", "NormalMs", "SlowMs"], Types = [typeof(TimeSpan)])]
    [Observable] public virtual partial double Animation { get; set; } = 150;

    [Observable] public virtual partial double HoverScale { get; set; } = 1.0486;

    [Observable] public virtual partial double PressedScale { get; set; } = 0.98;

    #endregion Motion

    #region States

    /// <summary>A disabled control's opacity; DisabledOpacityAccent (an accented fill) and DisabledOpacityChecked (a solid face) hold a little more so they still read.</summary>
    [SizeScale(Multipliers = [1.4, 2.5], Labels = ["Accent", "Checked"], Ceiling = 1)]
    [Observable] public virtual partial double DisabledOpacity { get; set; } = 0.3;

    /// <summary>A placeholder's fade, on top of its dim ink.</summary>
    public virtual double PlaceholderOpacity => 0.5;

    #endregion States

    #region Behaviour

    /// <summary>Whether controls draw the way the web does: they do not clip, and a grown hover paints over its neighbours.</summary>
    [Observable] public virtual partial bool WebRenderStyle { get; set; } = true;

    public virtual bool ControlClipToBounds => !WebRenderStyle;

    public virtual int HoverZIndex => WebRenderStyle ? 1 : 0;

    /// <summary>Whether a focused accented control takes <see cref="AccentFocus"/> for its border, as neutral controls do.</summary>
    [Observable] public virtual partial bool FocusColorOnAccents { get; set; } = false;

    #endregion Behaviour

    #region Alignment

    /// <summary>Everything that isn't pressed: where a control sits in its row, and where its text and content sit inside it.</summary>
    [Observable] public virtual partial Alignment StandardAlignment { get; set; } = new(HorizontalAlignment.Left, VerticalAlignment.Center);

    /// <summary>The content of anything pressed: buttons, toggles, tabs, menu items, spinner arrows.</summary>
    [Observable] public virtual partial Alignment ButtonAlignment { get; set; } = new(HorizontalAlignment.Center, VerticalAlignment.Center);

    #endregion Alignment

    /// <summary>The icon set.</summary>
    public override ThemeIconGallery Icons => new();
}
