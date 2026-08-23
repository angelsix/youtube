using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// Design token values for the Prototype theme. Marked with <c>[Theme]</c> so the
/// AngelSix.ThemeEngine source generator emits a markup extension for every public
/// property.
/// </summary>
/// <remarks>
/// All properties are <c>virtual</c> so subclasses can override individual token
/// values for alternative palettes (e.g. dark mode).
/// </remarks>
[Theme(FallbackHue = "Neutral")]
public class DefaultTheme
{
    #region Source Properties

    // Theme identity
    public virtual string ThemeName => "Default";

    /// <summary>
    /// Whether this is a dark palette. Selects the mirrored ramp, so a control theme asking for
    /// <c>PrimaryDark3</c> as its text colour gets a readable one in either palette.
    /// </summary>
    public virtual bool IsDark => false;

    // Base size multiplier (user-adjustable 0.75-2.0)
    public virtual double BaseSize => 1.0;

    // Every colour in the theme is a hue with a ramp. AccentNeutral is the palette's default —
    // [Theme(FallbackHue = "Neutral")] above — so a control that sets no Accent.Kind draws itself
    // from this one. Setting Accent.Kind is what opts a control into any of the others.
    [AccentHue] public virtual Color AccentNeutral => Color.Parse("#2E2E2E");

    [AccentHue] public virtual Color AccentPrimary => Color.Parse("#5BA3C9");
    [AccentHue] public virtual Color AccentSuccess => Color.Parse("#6DB87E");
    [AccentHue] public virtual Color AccentWarning => Color.Parse("#E0B860");
    [AccentHue] public virtual Color AccentError => Color.Parse("#D47A7A");
    [AccentHue] public virtual Color AccentInfo => Color.Parse("#E89F4A");
    [AccentHue] public virtual Color AccentDestructive => Color.Parse("#C17070");
    [AccentHue] public virtual Color AccentSubtle => Color.Parse("#B088C8");
    public virtual Color AccentBorder => Color.Parse("#B8A8C8");
    public virtual Color AccentFocus => Color.Parse("#4A90D9");

    #endregion Source Properties

    #region Colour Ramps

    // One ramp per hue, each centred on its own colour. Mirrored in a dark palette so a
    // control asking for Dark4 as its text colour still gets a readable one — see
    // MirroredColorRamp. Every stage is virtual and individually overridable.

    protected virtual ColorRamp NeutralRamp =>
        IsDark ? new MirroredColorRamp(AccentNeutral) : new ColorRamp(AccentNeutral);
    protected virtual ColorRamp PrimaryRamp =>
        IsDark ? new MirroredColorRamp(AccentPrimary) : new ColorRamp(AccentPrimary);
    protected virtual ColorRamp SuccessRamp =>
        IsDark ? new MirroredColorRamp(AccentSuccess) : new ColorRamp(AccentSuccess);
    protected virtual ColorRamp WarningRamp =>
        IsDark ? new MirroredColorRamp(AccentWarning) : new ColorRamp(AccentWarning);
    protected virtual ColorRamp ErrorRamp =>
        IsDark ? new MirroredColorRamp(AccentError) : new ColorRamp(AccentError);
    protected virtual ColorRamp InfoRamp =>
        IsDark ? new MirroredColorRamp(AccentInfo) : new ColorRamp(AccentInfo);
    protected virtual ColorRamp DestructiveRamp =>
        IsDark ? new MirroredColorRamp(AccentDestructive) : new ColorRamp(AccentDestructive);
    protected virtual ColorRamp SubtleRamp =>
        IsDark ? new MirroredColorRamp(AccentSubtle) : new ColorRamp(AccentSubtle);

    public virtual Color AccentNeutralLight1 => NeutralRamp.Light1;
    public virtual Color AccentNeutralLight2 => NeutralRamp.Light2;
    public virtual Color AccentNeutralLight3 => NeutralRamp.Light3;
    public virtual Color AccentNeutralLight4 => NeutralRamp.Light4;
    public virtual Color AccentNeutralLight5 => NeutralRamp.Light5;
    public virtual Color AccentNeutralLight6 => NeutralRamp.Light6;
    public virtual Color AccentNeutralLight7 => NeutralRamp.Light7;
    public virtual Color AccentNeutralLight8 => NeutralRamp.Light8;
    public virtual Color AccentNeutralLight9 => NeutralRamp.Light9;
    public virtual Color AccentNeutralLight10 => NeutralRamp.Light10;
    public virtual Color AccentNeutralDark1 => NeutralRamp.Dark1;
    public virtual Color AccentNeutralDark2 => NeutralRamp.Dark2;
    public virtual Color AccentNeutralDark3 => NeutralRamp.Dark3;
    public virtual Color AccentNeutralDark4 => NeutralRamp.Dark4;
    public virtual Color AccentNeutralDark5 => NeutralRamp.Dark5;
    public virtual Color AccentNeutralDark6 => NeutralRamp.Dark6;
    public virtual Color AccentNeutralDark7 => NeutralRamp.Dark7;
    public virtual Color AccentNeutralDark8 => NeutralRamp.Dark8;
    public virtual Color AccentNeutralDark9 => NeutralRamp.Dark9;
    public virtual Color AccentNeutralDark10 => NeutralRamp.Dark10;

    public virtual Color AccentPrimaryLight1 => PrimaryRamp.Light1;
    public virtual Color AccentPrimaryLight2 => PrimaryRamp.Light2;
    public virtual Color AccentPrimaryLight3 => PrimaryRamp.Light3;
    public virtual Color AccentPrimaryLight4 => PrimaryRamp.Light4;
    public virtual Color AccentPrimaryLight5 => PrimaryRamp.Light5;
    public virtual Color AccentPrimaryLight6 => PrimaryRamp.Light6;
    public virtual Color AccentPrimaryLight7 => PrimaryRamp.Light7;
    public virtual Color AccentPrimaryLight8 => PrimaryRamp.Light8;
    public virtual Color AccentPrimaryLight9 => PrimaryRamp.Light9;
    public virtual Color AccentPrimaryLight10 => PrimaryRamp.Light10;
    public virtual Color AccentPrimaryDark1 => PrimaryRamp.Dark1;
    public virtual Color AccentPrimaryDark2 => PrimaryRamp.Dark2;
    public virtual Color AccentPrimaryDark3 => PrimaryRamp.Dark3;
    public virtual Color AccentPrimaryDark4 => PrimaryRamp.Dark4;
    public virtual Color AccentPrimaryDark5 => PrimaryRamp.Dark5;
    public virtual Color AccentPrimaryDark6 => PrimaryRamp.Dark6;
    public virtual Color AccentPrimaryDark7 => PrimaryRamp.Dark7;
    public virtual Color AccentPrimaryDark8 => PrimaryRamp.Dark8;
    public virtual Color AccentPrimaryDark9 => PrimaryRamp.Dark9;
    public virtual Color AccentPrimaryDark10 => PrimaryRamp.Dark10;

    public virtual Color AccentSuccessLight1 => SuccessRamp.Light1;
    public virtual Color AccentSuccessLight2 => SuccessRamp.Light2;
    public virtual Color AccentSuccessLight3 => SuccessRamp.Light3;
    public virtual Color AccentSuccessLight4 => SuccessRamp.Light4;
    public virtual Color AccentSuccessLight5 => SuccessRamp.Light5;
    public virtual Color AccentSuccessLight6 => SuccessRamp.Light6;
    public virtual Color AccentSuccessLight7 => SuccessRamp.Light7;
    public virtual Color AccentSuccessLight8 => SuccessRamp.Light8;
    public virtual Color AccentSuccessLight9 => SuccessRamp.Light9;
    public virtual Color AccentSuccessLight10 => SuccessRamp.Light10;
    public virtual Color AccentSuccessDark1 => SuccessRamp.Dark1;
    public virtual Color AccentSuccessDark2 => SuccessRamp.Dark2;
    public virtual Color AccentSuccessDark3 => SuccessRamp.Dark3;
    public virtual Color AccentSuccessDark4 => SuccessRamp.Dark4;
    public virtual Color AccentSuccessDark5 => SuccessRamp.Dark5;
    public virtual Color AccentSuccessDark6 => SuccessRamp.Dark6;
    public virtual Color AccentSuccessDark7 => SuccessRamp.Dark7;
    public virtual Color AccentSuccessDark8 => SuccessRamp.Dark8;
    public virtual Color AccentSuccessDark9 => SuccessRamp.Dark9;
    public virtual Color AccentSuccessDark10 => SuccessRamp.Dark10;

    public virtual Color AccentWarningLight1 => WarningRamp.Light1;
    public virtual Color AccentWarningLight2 => WarningRamp.Light2;
    public virtual Color AccentWarningLight3 => WarningRamp.Light3;
    public virtual Color AccentWarningLight4 => WarningRamp.Light4;
    public virtual Color AccentWarningLight5 => WarningRamp.Light5;
    public virtual Color AccentWarningLight6 => WarningRamp.Light6;
    public virtual Color AccentWarningLight7 => WarningRamp.Light7;
    public virtual Color AccentWarningLight8 => WarningRamp.Light8;
    public virtual Color AccentWarningLight9 => WarningRamp.Light9;
    public virtual Color AccentWarningLight10 => WarningRamp.Light10;
    public virtual Color AccentWarningDark1 => WarningRamp.Dark1;
    public virtual Color AccentWarningDark2 => WarningRamp.Dark2;
    public virtual Color AccentWarningDark3 => WarningRamp.Dark3;
    public virtual Color AccentWarningDark4 => WarningRamp.Dark4;
    public virtual Color AccentWarningDark5 => WarningRamp.Dark5;
    public virtual Color AccentWarningDark6 => WarningRamp.Dark6;
    public virtual Color AccentWarningDark7 => WarningRamp.Dark7;
    public virtual Color AccentWarningDark8 => WarningRamp.Dark8;
    public virtual Color AccentWarningDark9 => WarningRamp.Dark9;
    public virtual Color AccentWarningDark10 => WarningRamp.Dark10;

    public virtual Color AccentErrorLight1 => ErrorRamp.Light1;
    public virtual Color AccentErrorLight2 => ErrorRamp.Light2;
    public virtual Color AccentErrorLight3 => ErrorRamp.Light3;
    public virtual Color AccentErrorLight4 => ErrorRamp.Light4;
    public virtual Color AccentErrorLight5 => ErrorRamp.Light5;
    public virtual Color AccentErrorLight6 => ErrorRamp.Light6;
    public virtual Color AccentErrorLight7 => ErrorRamp.Light7;
    public virtual Color AccentErrorLight8 => ErrorRamp.Light8;
    public virtual Color AccentErrorLight9 => ErrorRamp.Light9;
    public virtual Color AccentErrorLight10 => ErrorRamp.Light10;
    public virtual Color AccentErrorDark1 => ErrorRamp.Dark1;
    public virtual Color AccentErrorDark2 => ErrorRamp.Dark2;
    public virtual Color AccentErrorDark3 => ErrorRamp.Dark3;
    public virtual Color AccentErrorDark4 => ErrorRamp.Dark4;
    public virtual Color AccentErrorDark5 => ErrorRamp.Dark5;
    public virtual Color AccentErrorDark6 => ErrorRamp.Dark6;
    public virtual Color AccentErrorDark7 => ErrorRamp.Dark7;
    public virtual Color AccentErrorDark8 => ErrorRamp.Dark8;
    public virtual Color AccentErrorDark9 => ErrorRamp.Dark9;
    public virtual Color AccentErrorDark10 => ErrorRamp.Dark10;

    public virtual Color AccentInfoLight1 => InfoRamp.Light1;
    public virtual Color AccentInfoLight2 => InfoRamp.Light2;
    public virtual Color AccentInfoLight3 => InfoRamp.Light3;
    public virtual Color AccentInfoLight4 => InfoRamp.Light4;
    public virtual Color AccentInfoLight5 => InfoRamp.Light5;
    public virtual Color AccentInfoLight6 => InfoRamp.Light6;
    public virtual Color AccentInfoLight7 => InfoRamp.Light7;
    public virtual Color AccentInfoLight8 => InfoRamp.Light8;
    public virtual Color AccentInfoLight9 => InfoRamp.Light9;
    public virtual Color AccentInfoLight10 => InfoRamp.Light10;
    public virtual Color AccentInfoDark1 => InfoRamp.Dark1;
    public virtual Color AccentInfoDark2 => InfoRamp.Dark2;
    public virtual Color AccentInfoDark3 => InfoRamp.Dark3;
    public virtual Color AccentInfoDark4 => InfoRamp.Dark4;
    public virtual Color AccentInfoDark5 => InfoRamp.Dark5;
    public virtual Color AccentInfoDark6 => InfoRamp.Dark6;
    public virtual Color AccentInfoDark7 => InfoRamp.Dark7;
    public virtual Color AccentInfoDark8 => InfoRamp.Dark8;
    public virtual Color AccentInfoDark9 => InfoRamp.Dark9;
    public virtual Color AccentInfoDark10 => InfoRamp.Dark10;

    public virtual Color AccentDestructiveLight1 => DestructiveRamp.Light1;
    public virtual Color AccentDestructiveLight2 => DestructiveRamp.Light2;
    public virtual Color AccentDestructiveLight3 => DestructiveRamp.Light3;
    public virtual Color AccentDestructiveLight4 => DestructiveRamp.Light4;
    public virtual Color AccentDestructiveLight5 => DestructiveRamp.Light5;
    public virtual Color AccentDestructiveLight6 => DestructiveRamp.Light6;
    public virtual Color AccentDestructiveLight7 => DestructiveRamp.Light7;
    public virtual Color AccentDestructiveLight8 => DestructiveRamp.Light8;
    public virtual Color AccentDestructiveLight9 => DestructiveRamp.Light9;
    public virtual Color AccentDestructiveLight10 => DestructiveRamp.Light10;
    public virtual Color AccentDestructiveDark1 => DestructiveRamp.Dark1;
    public virtual Color AccentDestructiveDark2 => DestructiveRamp.Dark2;
    public virtual Color AccentDestructiveDark3 => DestructiveRamp.Dark3;
    public virtual Color AccentDestructiveDark4 => DestructiveRamp.Dark4;
    public virtual Color AccentDestructiveDark5 => DestructiveRamp.Dark5;
    public virtual Color AccentDestructiveDark6 => DestructiveRamp.Dark6;
    public virtual Color AccentDestructiveDark7 => DestructiveRamp.Dark7;
    public virtual Color AccentDestructiveDark8 => DestructiveRamp.Dark8;
    public virtual Color AccentDestructiveDark9 => DestructiveRamp.Dark9;
    public virtual Color AccentDestructiveDark10 => DestructiveRamp.Dark10;

    public virtual Color AccentSubtleLight1 => SubtleRamp.Light1;
    public virtual Color AccentSubtleLight2 => SubtleRamp.Light2;
    public virtual Color AccentSubtleLight3 => SubtleRamp.Light3;
    public virtual Color AccentSubtleLight4 => SubtleRamp.Light4;
    public virtual Color AccentSubtleLight5 => SubtleRamp.Light5;
    public virtual Color AccentSubtleLight6 => SubtleRamp.Light6;
    public virtual Color AccentSubtleLight7 => SubtleRamp.Light7;
    public virtual Color AccentSubtleLight8 => SubtleRamp.Light8;
    public virtual Color AccentSubtleLight9 => SubtleRamp.Light9;
    public virtual Color AccentSubtleLight10 => SubtleRamp.Light10;
    public virtual Color AccentSubtleDark1 => SubtleRamp.Dark1;
    public virtual Color AccentSubtleDark2 => SubtleRamp.Dark2;
    public virtual Color AccentSubtleDark3 => SubtleRamp.Dark3;
    public virtual Color AccentSubtleDark4 => SubtleRamp.Dark4;
    public virtual Color AccentSubtleDark5 => SubtleRamp.Dark5;
    public virtual Color AccentSubtleDark6 => SubtleRamp.Dark6;
    public virtual Color AccentSubtleDark7 => SubtleRamp.Dark7;
    public virtual Color AccentSubtleDark8 => SubtleRamp.Dark8;
    public virtual Color AccentSubtleDark9 => SubtleRamp.Dark9;
    public virtual Color AccentSubtleDark10 => SubtleRamp.Dark10;



    #endregion Colour Ramps

    #region Neutral Aliases

    // The neutral hue, pinned. {theme:AccentBrush Dark1} follows whatever Accent.Kind a control
    // carries, which is right for the control's own chrome but wrong for the parts that must stay
    // neutral inside an accented control — a caret, a disabled label, a divider. These resolve
    // straight to the neutral ramp and ignore Accent.Kind entirely.
    //
    // They live outside the Accent prefix deliberately: that is what keeps them visible in
    // IntelliSense while the Accent{Hue}{Stage} properties behind them stay suppressed.

    public virtual Color Neutral => AccentNeutral;
    public virtual SolidColorBrush NeutralBrush => AccentNeutralBrush;

    public virtual Color NeutralLight1 => AccentNeutralLight1;
    public virtual Color NeutralLight2 => AccentNeutralLight2;
    public virtual Color NeutralLight3 => AccentNeutralLight3;
    public virtual Color NeutralLight4 => AccentNeutralLight4;
    public virtual Color NeutralLight5 => AccentNeutralLight5;
    public virtual Color NeutralLight6 => AccentNeutralLight6;
    public virtual Color NeutralLight7 => AccentNeutralLight7;
    public virtual Color NeutralLight8 => AccentNeutralLight8;
    public virtual Color NeutralLight9 => AccentNeutralLight9;
    public virtual Color NeutralLight10 => AccentNeutralLight10;

    public virtual Color NeutralDark1 => AccentNeutralDark1;
    public virtual Color NeutralDark2 => AccentNeutralDark2;
    public virtual Color NeutralDark3 => AccentNeutralDark3;
    public virtual Color NeutralDark4 => AccentNeutralDark4;
    public virtual Color NeutralDark5 => AccentNeutralDark5;
    public virtual Color NeutralDark6 => AccentNeutralDark6;
    public virtual Color NeutralDark7 => AccentNeutralDark7;
    public virtual Color NeutralDark8 => AccentNeutralDark8;
    public virtual Color NeutralDark9 => AccentNeutralDark9;
    public virtual Color NeutralDark10 => AccentNeutralDark10;

    public virtual SolidColorBrush NeutralLight1Brush => AccentNeutralLight1Brush;
    public virtual SolidColorBrush NeutralLight2Brush => AccentNeutralLight2Brush;
    public virtual SolidColorBrush NeutralLight3Brush => AccentNeutralLight3Brush;
    public virtual SolidColorBrush NeutralLight4Brush => AccentNeutralLight4Brush;
    public virtual SolidColorBrush NeutralLight5Brush => AccentNeutralLight5Brush;
    public virtual SolidColorBrush NeutralLight6Brush => AccentNeutralLight6Brush;
    public virtual SolidColorBrush NeutralLight7Brush => AccentNeutralLight7Brush;
    public virtual SolidColorBrush NeutralLight8Brush => AccentNeutralLight8Brush;
    public virtual SolidColorBrush NeutralLight9Brush => AccentNeutralLight9Brush;
    public virtual SolidColorBrush NeutralLight10Brush => AccentNeutralLight10Brush;

    public virtual SolidColorBrush NeutralDark1Brush => AccentNeutralDark1Brush;
    public virtual SolidColorBrush NeutralDark2Brush => AccentNeutralDark2Brush;
    public virtual SolidColorBrush NeutralDark3Brush => AccentNeutralDark3Brush;
    public virtual SolidColorBrush NeutralDark4Brush => AccentNeutralDark4Brush;
    public virtual SolidColorBrush NeutralDark5Brush => AccentNeutralDark5Brush;
    public virtual SolidColorBrush NeutralDark6Brush => AccentNeutralDark6Brush;
    public virtual SolidColorBrush NeutralDark7Brush => AccentNeutralDark7Brush;
    public virtual SolidColorBrush NeutralDark8Brush => AccentNeutralDark8Brush;
    public virtual SolidColorBrush NeutralDark9Brush => AccentNeutralDark9Brush;
    public virtual SolidColorBrush NeutralDark10Brush => AccentNeutralDark10Brush;

    #endregion Neutral Aliases



    #region Source Properties

    // Spacing scale
    public virtual double SpacingSm => 2 * BaseSize;
    public virtual double SpacingMd => 4 * BaseSize;
    public virtual double SpacingLg => 8 * BaseSize;
    public virtual double SpacingXl => 12 * BaseSize;
    public virtual double SpacingXxl => 16 * BaseSize;

    // Type scale (font sizes)
    public virtual double FontSizeSm => 12 * BaseSize;
    public virtual double FontSizeMd => 14 * BaseSize;
    public virtual double FontSizeLg => 16 * BaseSize;
    public virtual double FontSizeXl => 20 * BaseSize;
    public virtual double FontSizeXxl => 24 * BaseSize;

    // Control height scale
    public virtual double ControlHeightSm => 32 * BaseSize;
    public virtual double ControlHeightMd => 40 * BaseSize;
    public virtual double ControlHeightLg => 48 * BaseSize;

    // Control metrics
    public virtual double ControlMinWidth => 64 * BaseSize;
    public virtual double IconSize => 16 * BaseSize;

    // Typeface
    public virtual FontFamily FontFamily => new FontFamily("Inter, $Default");
    public virtual FontWeight FontWeightRegular => FontWeight.Normal;
    public virtual FontWeight FontWeightSemiBold => FontWeight.SemiBold;
    public virtual FontWeight FontWeightBold => FontWeight.Bold;

    // Accent visual properties (for highlighted/prominent elements)
    public virtual double AccentBorderStrokeThickness => 2 * BaseSize;
    public virtual FontWeight AccentFontWeight => FontWeight.SemiBold;


    // State visual properties
    public virtual double DisabledOpacity => 0.3;
    public virtual double PressedScale => 0.98;

    // Animation timing (seconds)
    public virtual TimeSpan AnimationFastMs => TimeSpan.FromMilliseconds(75);
    public virtual TimeSpan AnimationNormalMs => TimeSpan.FromMilliseconds(150);
    public virtual TimeSpan AnimationSlowMs => TimeSpan.FromMilliseconds(300);

    // Control alignment defaults (for interactive controls: buttons, inputs, pickers, etc.)
    public virtual HorizontalAlignment ControlHorizontalAlignment => HorizontalAlignment.Left;
    public virtual VerticalAlignment ControlVerticalAlignment => VerticalAlignment.Center;
    public virtual HorizontalAlignment ControlHorizontalContentAlignment => HorizontalAlignment.Center;
    public virtual VerticalAlignment ControlVerticalContentAlignment => VerticalAlignment.Center;

    // Container alignment defaults (for structural elements that stretch to fill: list items, panels, etc.)
    public virtual HorizontalAlignment ContainerHorizontalAlignment => HorizontalAlignment.Stretch;
    public virtual VerticalAlignment ContainerVerticalAlignment => VerticalAlignment.Stretch;

    // Shape tokens
    public virtual CornerRadius RadiusSm => new CornerRadius(3 * BaseSize);
    public virtual CornerRadius RadiusMd => new CornerRadius(6 * BaseSize);

    // Corner radius as a plain double (for Shape.RadiusX/Y which accept double, not CornerRadius)
    public virtual double RadiusSmDouble => 3 * BaseSize;
    public virtual double RadiusMdDouble => 6 * BaseSize;

    // Thickness scale (uniform border/outline thicknesses, spacing scale halved)
    public virtual Thickness ThicknessSm => new Thickness(1 * BaseSize);
    public virtual Thickness ThicknessMd => new Thickness(2 * BaseSize);
    public virtual Thickness ThicknessLg => new Thickness(3 * BaseSize);
    public virtual Thickness ThicknessXl => new Thickness(4 * BaseSize);
    public virtual Thickness ThicknessXxl => new Thickness(6 * BaseSize);

    #endregion Source Properties

    #region Icon Gallery

    /// <summary>
    /// The icon gallery for this theme. Override this to swap the entire icon set.
    /// XAML accesses individual glyphs via <c>{icons:GlyphName}</c> (markup extension
    /// in <c>AngelSix.ThemeEngine.Generated</c>), which drills into this property.
    /// </summary>
    public virtual ThemeIconGallery Icons => new();

    #endregion

    #region Derived Properties
    // Colour brushes — one per hue, plus its ten stages. This is the set
    // {theme:AccentBrush Dark2} resolves against by composing Accent{Kind}{Variant}Brush.

    public virtual SolidColorBrush AccentNeutralBrush => new(AccentNeutral);
    public virtual SolidColorBrush AccentNeutralLight1Brush => new(AccentNeutralLight1);
    public virtual SolidColorBrush AccentNeutralLight2Brush => new(AccentNeutralLight2);
    public virtual SolidColorBrush AccentNeutralLight3Brush => new(AccentNeutralLight3);
    public virtual SolidColorBrush AccentNeutralLight4Brush => new(AccentNeutralLight4);
    public virtual SolidColorBrush AccentNeutralLight5Brush => new(AccentNeutralLight5);
    public virtual SolidColorBrush AccentNeutralLight6Brush => new(AccentNeutralLight6);
    public virtual SolidColorBrush AccentNeutralLight7Brush => new(AccentNeutralLight7);
    public virtual SolidColorBrush AccentNeutralLight8Brush => new(AccentNeutralLight8);
    public virtual SolidColorBrush AccentNeutralLight9Brush => new(AccentNeutralLight9);
    public virtual SolidColorBrush AccentNeutralLight10Brush => new(AccentNeutralLight10);
    public virtual SolidColorBrush AccentNeutralDark1Brush => new(AccentNeutralDark1);
    public virtual SolidColorBrush AccentNeutralDark2Brush => new(AccentNeutralDark2);
    public virtual SolidColorBrush AccentNeutralDark3Brush => new(AccentNeutralDark3);
    public virtual SolidColorBrush AccentNeutralDark4Brush => new(AccentNeutralDark4);
    public virtual SolidColorBrush AccentNeutralDark5Brush => new(AccentNeutralDark5);
    public virtual SolidColorBrush AccentNeutralDark6Brush => new(AccentNeutralDark6);
    public virtual SolidColorBrush AccentNeutralDark7Brush => new(AccentNeutralDark7);
    public virtual SolidColorBrush AccentNeutralDark8Brush => new(AccentNeutralDark8);
    public virtual SolidColorBrush AccentNeutralDark9Brush => new(AccentNeutralDark9);
    public virtual SolidColorBrush AccentNeutralDark10Brush => new(AccentNeutralDark10);

    public virtual SolidColorBrush AccentPrimaryBrush => new(AccentPrimary);
    public virtual SolidColorBrush AccentPrimaryLight1Brush => new(AccentPrimaryLight1);
    public virtual SolidColorBrush AccentPrimaryLight2Brush => new(AccentPrimaryLight2);
    public virtual SolidColorBrush AccentPrimaryLight3Brush => new(AccentPrimaryLight3);
    public virtual SolidColorBrush AccentPrimaryLight4Brush => new(AccentPrimaryLight4);
    public virtual SolidColorBrush AccentPrimaryLight5Brush => new(AccentPrimaryLight5);
    public virtual SolidColorBrush AccentPrimaryLight6Brush => new(AccentPrimaryLight6);
    public virtual SolidColorBrush AccentPrimaryLight7Brush => new(AccentPrimaryLight7);
    public virtual SolidColorBrush AccentPrimaryLight8Brush => new(AccentPrimaryLight8);
    public virtual SolidColorBrush AccentPrimaryLight9Brush => new(AccentPrimaryLight9);
    public virtual SolidColorBrush AccentPrimaryLight10Brush => new(AccentPrimaryLight10);
    public virtual SolidColorBrush AccentPrimaryDark1Brush => new(AccentPrimaryDark1);
    public virtual SolidColorBrush AccentPrimaryDark2Brush => new(AccentPrimaryDark2);
    public virtual SolidColorBrush AccentPrimaryDark3Brush => new(AccentPrimaryDark3);
    public virtual SolidColorBrush AccentPrimaryDark4Brush => new(AccentPrimaryDark4);
    public virtual SolidColorBrush AccentPrimaryDark5Brush => new(AccentPrimaryDark5);
    public virtual SolidColorBrush AccentPrimaryDark6Brush => new(AccentPrimaryDark6);
    public virtual SolidColorBrush AccentPrimaryDark7Brush => new(AccentPrimaryDark7);
    public virtual SolidColorBrush AccentPrimaryDark8Brush => new(AccentPrimaryDark8);
    public virtual SolidColorBrush AccentPrimaryDark9Brush => new(AccentPrimaryDark9);
    public virtual SolidColorBrush AccentPrimaryDark10Brush => new(AccentPrimaryDark10);

    public virtual SolidColorBrush AccentSuccessBrush => new(AccentSuccess);
    public virtual SolidColorBrush AccentSuccessLight1Brush => new(AccentSuccessLight1);
    public virtual SolidColorBrush AccentSuccessLight2Brush => new(AccentSuccessLight2);
    public virtual SolidColorBrush AccentSuccessLight3Brush => new(AccentSuccessLight3);
    public virtual SolidColorBrush AccentSuccessLight4Brush => new(AccentSuccessLight4);
    public virtual SolidColorBrush AccentSuccessLight5Brush => new(AccentSuccessLight5);
    public virtual SolidColorBrush AccentSuccessLight6Brush => new(AccentSuccessLight6);
    public virtual SolidColorBrush AccentSuccessLight7Brush => new(AccentSuccessLight7);
    public virtual SolidColorBrush AccentSuccessLight8Brush => new(AccentSuccessLight8);
    public virtual SolidColorBrush AccentSuccessLight9Brush => new(AccentSuccessLight9);
    public virtual SolidColorBrush AccentSuccessLight10Brush => new(AccentSuccessLight10);
    public virtual SolidColorBrush AccentSuccessDark1Brush => new(AccentSuccessDark1);
    public virtual SolidColorBrush AccentSuccessDark2Brush => new(AccentSuccessDark2);
    public virtual SolidColorBrush AccentSuccessDark3Brush => new(AccentSuccessDark3);
    public virtual SolidColorBrush AccentSuccessDark4Brush => new(AccentSuccessDark4);
    public virtual SolidColorBrush AccentSuccessDark5Brush => new(AccentSuccessDark5);
    public virtual SolidColorBrush AccentSuccessDark6Brush => new(AccentSuccessDark6);
    public virtual SolidColorBrush AccentSuccessDark7Brush => new(AccentSuccessDark7);
    public virtual SolidColorBrush AccentSuccessDark8Brush => new(AccentSuccessDark8);
    public virtual SolidColorBrush AccentSuccessDark9Brush => new(AccentSuccessDark9);
    public virtual SolidColorBrush AccentSuccessDark10Brush => new(AccentSuccessDark10);

    public virtual SolidColorBrush AccentWarningBrush => new(AccentWarning);
    public virtual SolidColorBrush AccentWarningLight1Brush => new(AccentWarningLight1);
    public virtual SolidColorBrush AccentWarningLight2Brush => new(AccentWarningLight2);
    public virtual SolidColorBrush AccentWarningLight3Brush => new(AccentWarningLight3);
    public virtual SolidColorBrush AccentWarningLight4Brush => new(AccentWarningLight4);
    public virtual SolidColorBrush AccentWarningLight5Brush => new(AccentWarningLight5);
    public virtual SolidColorBrush AccentWarningLight6Brush => new(AccentWarningLight6);
    public virtual SolidColorBrush AccentWarningLight7Brush => new(AccentWarningLight7);
    public virtual SolidColorBrush AccentWarningLight8Brush => new(AccentWarningLight8);
    public virtual SolidColorBrush AccentWarningLight9Brush => new(AccentWarningLight9);
    public virtual SolidColorBrush AccentWarningLight10Brush => new(AccentWarningLight10);
    public virtual SolidColorBrush AccentWarningDark1Brush => new(AccentWarningDark1);
    public virtual SolidColorBrush AccentWarningDark2Brush => new(AccentWarningDark2);
    public virtual SolidColorBrush AccentWarningDark3Brush => new(AccentWarningDark3);
    public virtual SolidColorBrush AccentWarningDark4Brush => new(AccentWarningDark4);
    public virtual SolidColorBrush AccentWarningDark5Brush => new(AccentWarningDark5);
    public virtual SolidColorBrush AccentWarningDark6Brush => new(AccentWarningDark6);
    public virtual SolidColorBrush AccentWarningDark7Brush => new(AccentWarningDark7);
    public virtual SolidColorBrush AccentWarningDark8Brush => new(AccentWarningDark8);
    public virtual SolidColorBrush AccentWarningDark9Brush => new(AccentWarningDark9);
    public virtual SolidColorBrush AccentWarningDark10Brush => new(AccentWarningDark10);

    public virtual SolidColorBrush AccentErrorBrush => new(AccentError);
    public virtual SolidColorBrush AccentErrorLight1Brush => new(AccentErrorLight1);
    public virtual SolidColorBrush AccentErrorLight2Brush => new(AccentErrorLight2);
    public virtual SolidColorBrush AccentErrorLight3Brush => new(AccentErrorLight3);
    public virtual SolidColorBrush AccentErrorLight4Brush => new(AccentErrorLight4);
    public virtual SolidColorBrush AccentErrorLight5Brush => new(AccentErrorLight5);
    public virtual SolidColorBrush AccentErrorLight6Brush => new(AccentErrorLight6);
    public virtual SolidColorBrush AccentErrorLight7Brush => new(AccentErrorLight7);
    public virtual SolidColorBrush AccentErrorLight8Brush => new(AccentErrorLight8);
    public virtual SolidColorBrush AccentErrorLight9Brush => new(AccentErrorLight9);
    public virtual SolidColorBrush AccentErrorLight10Brush => new(AccentErrorLight10);
    public virtual SolidColorBrush AccentErrorDark1Brush => new(AccentErrorDark1);
    public virtual SolidColorBrush AccentErrorDark2Brush => new(AccentErrorDark2);
    public virtual SolidColorBrush AccentErrorDark3Brush => new(AccentErrorDark3);
    public virtual SolidColorBrush AccentErrorDark4Brush => new(AccentErrorDark4);
    public virtual SolidColorBrush AccentErrorDark5Brush => new(AccentErrorDark5);
    public virtual SolidColorBrush AccentErrorDark6Brush => new(AccentErrorDark6);
    public virtual SolidColorBrush AccentErrorDark7Brush => new(AccentErrorDark7);
    public virtual SolidColorBrush AccentErrorDark8Brush => new(AccentErrorDark8);
    public virtual SolidColorBrush AccentErrorDark9Brush => new(AccentErrorDark9);
    public virtual SolidColorBrush AccentErrorDark10Brush => new(AccentErrorDark10);

    public virtual SolidColorBrush AccentInfoBrush => new(AccentInfo);
    public virtual SolidColorBrush AccentInfoLight1Brush => new(AccentInfoLight1);
    public virtual SolidColorBrush AccentInfoLight2Brush => new(AccentInfoLight2);
    public virtual SolidColorBrush AccentInfoLight3Brush => new(AccentInfoLight3);
    public virtual SolidColorBrush AccentInfoLight4Brush => new(AccentInfoLight4);
    public virtual SolidColorBrush AccentInfoLight5Brush => new(AccentInfoLight5);
    public virtual SolidColorBrush AccentInfoLight6Brush => new(AccentInfoLight6);
    public virtual SolidColorBrush AccentInfoLight7Brush => new(AccentInfoLight7);
    public virtual SolidColorBrush AccentInfoLight8Brush => new(AccentInfoLight8);
    public virtual SolidColorBrush AccentInfoLight9Brush => new(AccentInfoLight9);
    public virtual SolidColorBrush AccentInfoLight10Brush => new(AccentInfoLight10);
    public virtual SolidColorBrush AccentInfoDark1Brush => new(AccentInfoDark1);
    public virtual SolidColorBrush AccentInfoDark2Brush => new(AccentInfoDark2);
    public virtual SolidColorBrush AccentInfoDark3Brush => new(AccentInfoDark3);
    public virtual SolidColorBrush AccentInfoDark4Brush => new(AccentInfoDark4);
    public virtual SolidColorBrush AccentInfoDark5Brush => new(AccentInfoDark5);
    public virtual SolidColorBrush AccentInfoDark6Brush => new(AccentInfoDark6);
    public virtual SolidColorBrush AccentInfoDark7Brush => new(AccentInfoDark7);
    public virtual SolidColorBrush AccentInfoDark8Brush => new(AccentInfoDark8);
    public virtual SolidColorBrush AccentInfoDark9Brush => new(AccentInfoDark9);
    public virtual SolidColorBrush AccentInfoDark10Brush => new(AccentInfoDark10);

    public virtual SolidColorBrush AccentDestructiveBrush => new(AccentDestructive);
    public virtual SolidColorBrush AccentDestructiveLight1Brush => new(AccentDestructiveLight1);
    public virtual SolidColorBrush AccentDestructiveLight2Brush => new(AccentDestructiveLight2);
    public virtual SolidColorBrush AccentDestructiveLight3Brush => new(AccentDestructiveLight3);
    public virtual SolidColorBrush AccentDestructiveLight4Brush => new(AccentDestructiveLight4);
    public virtual SolidColorBrush AccentDestructiveLight5Brush => new(AccentDestructiveLight5);
    public virtual SolidColorBrush AccentDestructiveLight6Brush => new(AccentDestructiveLight6);
    public virtual SolidColorBrush AccentDestructiveLight7Brush => new(AccentDestructiveLight7);
    public virtual SolidColorBrush AccentDestructiveLight8Brush => new(AccentDestructiveLight8);
    public virtual SolidColorBrush AccentDestructiveLight9Brush => new(AccentDestructiveLight9);
    public virtual SolidColorBrush AccentDestructiveLight10Brush => new(AccentDestructiveLight10);
    public virtual SolidColorBrush AccentDestructiveDark1Brush => new(AccentDestructiveDark1);
    public virtual SolidColorBrush AccentDestructiveDark2Brush => new(AccentDestructiveDark2);
    public virtual SolidColorBrush AccentDestructiveDark3Brush => new(AccentDestructiveDark3);
    public virtual SolidColorBrush AccentDestructiveDark4Brush => new(AccentDestructiveDark4);
    public virtual SolidColorBrush AccentDestructiveDark5Brush => new(AccentDestructiveDark5);
    public virtual SolidColorBrush AccentDestructiveDark6Brush => new(AccentDestructiveDark6);
    public virtual SolidColorBrush AccentDestructiveDark7Brush => new(AccentDestructiveDark7);
    public virtual SolidColorBrush AccentDestructiveDark8Brush => new(AccentDestructiveDark8);
    public virtual SolidColorBrush AccentDestructiveDark9Brush => new(AccentDestructiveDark9);
    public virtual SolidColorBrush AccentDestructiveDark10Brush => new(AccentDestructiveDark10);

    public virtual SolidColorBrush AccentSubtleBrush => new(AccentSubtle);
    public virtual SolidColorBrush AccentSubtleLight1Brush => new(AccentSubtleLight1);
    public virtual SolidColorBrush AccentSubtleLight2Brush => new(AccentSubtleLight2);
    public virtual SolidColorBrush AccentSubtleLight3Brush => new(AccentSubtleLight3);
    public virtual SolidColorBrush AccentSubtleLight4Brush => new(AccentSubtleLight4);
    public virtual SolidColorBrush AccentSubtleLight5Brush => new(AccentSubtleLight5);
    public virtual SolidColorBrush AccentSubtleLight6Brush => new(AccentSubtleLight6);
    public virtual SolidColorBrush AccentSubtleLight7Brush => new(AccentSubtleLight7);
    public virtual SolidColorBrush AccentSubtleLight8Brush => new(AccentSubtleLight8);
    public virtual SolidColorBrush AccentSubtleLight9Brush => new(AccentSubtleLight9);
    public virtual SolidColorBrush AccentSubtleLight10Brush => new(AccentSubtleLight10);
    public virtual SolidColorBrush AccentSubtleDark1Brush => new(AccentSubtleDark1);
    public virtual SolidColorBrush AccentSubtleDark2Brush => new(AccentSubtleDark2);
    public virtual SolidColorBrush AccentSubtleDark3Brush => new(AccentSubtleDark3);
    public virtual SolidColorBrush AccentSubtleDark4Brush => new(AccentSubtleDark4);
    public virtual SolidColorBrush AccentSubtleDark5Brush => new(AccentSubtleDark5);
    public virtual SolidColorBrush AccentSubtleDark6Brush => new(AccentSubtleDark6);
    public virtual SolidColorBrush AccentSubtleDark7Brush => new(AccentSubtleDark7);
    public virtual SolidColorBrush AccentSubtleDark8Brush => new(AccentSubtleDark8);
    public virtual SolidColorBrush AccentSubtleDark9Brush => new(AccentSubtleDark9);
    public virtual SolidColorBrush AccentSubtleDark10Brush => new(AccentSubtleDark10);

    public virtual SolidColorBrush AccentBorderBrush => new(AccentBorder);

    public virtual SolidColorBrush AccentFocusBrush => new(AccentFocus);

    // Translucent tints for anything drawn *over* the palette: shadows and scrims. The ramp
    // only makes opaque colours, so these stay brushes — the opacity is the caller's choice
    // while the colour is the palette's. The mirrored dark ramp turns Dark5 into a lift.
    public virtual SolidColorBrush OverlayWeakBrush => ColorOverlayBrush(AccentNeutralDark10, 0.08);
    public virtual SolidColorBrush OverlayMediumBrush => ColorOverlayBrush(AccentNeutralDark10, 0.16);
    public virtual SolidColorBrush OverlayStrongBrush => ColorOverlayBrush(AccentNeutralDark10, 0.32);

    #endregion Derived Properties

    #region Methods

    private static SolidColorBrush ColorOverlayBrush(Color color, double opacity)
    {
        // HslColor's alpha is a double in the 0..1 range, so pass the opacity
        // straight through. Casting it to a 0-255 byte first would clamp every
        // overlay to fully opaque, collapsing hover/pressed into solid black.
        var hsl = color.ToHsl();
        return new SolidColorBrush(new HslColor(opacity, hsl.H, hsl.S, hsl.L).ToRgb());
    }



    #endregion Methods
}

/// <summary>
/// Dark variant of the default theme. Sets the neutral centre point and the palette direction;
/// every stage falls out of the mirrored ramp. Only the destructive accent differs beyond that.
/// </summary>
[Theme]
public class DefaultThemeDark : DefaultTheme
{
    #region Properties

    public override string ThemeName => "Default Dark";

    public override bool IsDark => true;

    // The neutral centre, as in the light palette: the default fill of a control.
    public override Color AccentNeutral => Color.Parse("#D8D8E4");

    // Destructive accent (darker red)
    public override Color AccentDestructive => Color.Parse("#C17070");

    #endregion Properties
}
