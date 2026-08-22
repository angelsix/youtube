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
[Theme]
public class DefaultTheme
{
    #region Source Properties

    // Theme identity
    public virtual string ThemeName => "Default";

    // Base size multiplier (user-adjustable 0.75-2.0)
    public virtual double BaseSize => 1.0;

    // Colour tokens (13)
    public virtual Color Surface => Color.Parse("#FFFFFFFF");
    public virtual Color OnSurface => Color.Parse("#1E1E2E");
    public virtual Color OnSurfaceDim => Color.Parse("#7A7A8E");
    public virtual Color SurfaceOverlay => Color.Parse("#000000");

    public virtual Color AccentPrimary => Color.Parse("#5BA3C9");
    public virtual Color AccentSuccess => Color.Parse("#6DB87E");
    public virtual Color AccentWarning => Color.Parse("#E0B860");
    public virtual Color AccentError => Color.Parse("#D47A7A");
    public virtual Color AccentInfo => Color.Parse("#E89F4A");
    public virtual Color AccentDestructive => Color.Parse("#C17070");
    public virtual Color AccentSubtle => Color.Parse("#B088C8");
    public virtual Color AccentNeutral => Color.Parse("#B0B0B8");
    public virtual Color AccentBorder => Color.Parse("#B8A8C8");
    public virtual Color AccentFocus => Color.Parse("#4A90D9");

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

    // Accent overlay states — light overlay of the accent colour itself, matching the button's
    // default SurfaceOverlay pattern but keyed to the accent hue for a tonally-consistent highlight.
    public virtual SolidColorBrush AccentHoverOverlayBrush => ColorOverlayBrush(AccentPrimary, 0.08);
    public virtual SolidColorBrush AccentPressedOverlayBrush => ColorOverlayBrush(AccentPrimary, 0.20);

    // Per-hue overlay states. The pair above is hardcoded to AccentPrimary, which breaks the
    // Accent{Kind}{Variant}Brush naming convention {theme:AccentBrush HoverOverlay} resolves
    // against, so every hue gets its own pair.
    public virtual SolidColorBrush AccentPrimaryHoverOverlayBrush => ColorOverlayBrush(AccentPrimary, 0.08);
    public virtual SolidColorBrush AccentPrimaryPressedOverlayBrush => ColorOverlayBrush(AccentPrimary, 0.20);
    public virtual SolidColorBrush AccentSuccessHoverOverlayBrush => ColorOverlayBrush(AccentSuccess, 0.08);
    public virtual SolidColorBrush AccentSuccessPressedOverlayBrush => ColorOverlayBrush(AccentSuccess, 0.20);
    public virtual SolidColorBrush AccentWarningHoverOverlayBrush => ColorOverlayBrush(AccentWarning, 0.08);
    public virtual SolidColorBrush AccentWarningPressedOverlayBrush => ColorOverlayBrush(AccentWarning, 0.20);
    public virtual SolidColorBrush AccentErrorHoverOverlayBrush => ColorOverlayBrush(AccentError, 0.08);
    public virtual SolidColorBrush AccentErrorPressedOverlayBrush => ColorOverlayBrush(AccentError, 0.20);
    public virtual SolidColorBrush AccentInfoHoverOverlayBrush => ColorOverlayBrush(AccentInfo, 0.08);
    public virtual SolidColorBrush AccentInfoPressedOverlayBrush => ColorOverlayBrush(AccentInfo, 0.20);
    public virtual SolidColorBrush AccentDestructiveHoverOverlayBrush => ColorOverlayBrush(AccentDestructive, 0.08);
    public virtual SolidColorBrush AccentDestructivePressedOverlayBrush => ColorOverlayBrush(AccentDestructive, 0.20);
    public virtual SolidColorBrush AccentSubtleHoverOverlayBrush => ColorOverlayBrush(AccentSubtle, 0.08);
    public virtual SolidColorBrush AccentSubtlePressedOverlayBrush => ColorOverlayBrush(AccentSubtle, 0.20);
    public virtual SolidColorBrush AccentNeutralHoverOverlayBrush => ColorOverlayBrush(AccentNeutral, 0.08);
    public virtual SolidColorBrush AccentNeutralPressedOverlayBrush => ColorOverlayBrush(AccentNeutral, 0.20);
    public virtual SolidColorBrush AccentBorderHoverOverlayBrush => ColorOverlayBrush(AccentBorder, 0.08);
    public virtual SolidColorBrush AccentBorderPressedOverlayBrush => ColorOverlayBrush(AccentBorder, 0.20);
    public virtual SolidColorBrush AccentFocusHoverOverlayBrush => ColorOverlayBrush(AccentFocus, 0.08);
    public virtual SolidColorBrush AccentFocusPressedOverlayBrush => ColorOverlayBrush(AccentFocus, 0.20);

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

    // Colour brushes
    public virtual SolidColorBrush SurfaceBrush => new(Surface);
    public virtual SolidColorBrush OnSurfaceBrush => new(OnSurface);
    public virtual SolidColorBrush OnSurfaceDimBrush => new(OnSurfaceDim);

    public virtual SolidColorBrush AccentPrimaryBrush => new(AccentPrimary);
    public virtual SolidColorBrush AccentSuccessBrush => new(AccentSuccess);
    public virtual SolidColorBrush AccentWarningBrush => new(AccentWarning);
    public virtual SolidColorBrush AccentErrorBrush => new(AccentError);
    public virtual SolidColorBrush AccentInfoBrush => new(AccentInfo);
    public virtual SolidColorBrush AccentDestructiveBrush => new(AccentDestructive);
    public virtual SolidColorBrush AccentSubtleBrush => new(AccentSubtle);
    public virtual SolidColorBrush AccentNeutralBrush => new(AccentNeutral);
    public virtual SolidColorBrush AccentBorderBrush => new(AccentBorder);
    public virtual SolidColorBrush AccentFocusBrush => new(AccentFocus);

    // Overlay brushes (derived from SurfaceOverlay with opacity)
    public virtual SolidColorBrush SurfaceOverlayWeakBrush => ColorOverlayBrush(SurfaceOverlay, 0.08);
    public virtual SolidColorBrush SurfaceOverlayMediumBrush => ColorOverlayBrush(SurfaceOverlay, 0.16);
    public virtual SolidColorBrush SurfaceOverlayStrongBrush => ColorOverlayBrush(SurfaceOverlay, 0.32);

    // Derived accent shades - Dark (9 accents x 3)
    public virtual Color AccentPrimaryDark1 => GetDarkShade(AccentPrimary, 1);
    public virtual Color AccentPrimaryDark2 => GetDarkShade(AccentPrimary, 2);
    public virtual Color AccentPrimaryDark3 => GetDarkShade(AccentPrimary, 3);
    public virtual Color AccentSuccessDark1 => GetDarkShade(AccentSuccess, 1);
    public virtual Color AccentSuccessDark2 => GetDarkShade(AccentSuccess, 2);
    public virtual Color AccentSuccessDark3 => GetDarkShade(AccentSuccess, 3);
    public virtual Color AccentWarningDark1 => GetDarkShade(AccentWarning, 1);
    public virtual Color AccentWarningDark2 => GetDarkShade(AccentWarning, 2);
    public virtual Color AccentWarningDark3 => GetDarkShade(AccentWarning, 3);
    public virtual Color AccentErrorDark1 => GetDarkShade(AccentError, 1);
    public virtual Color AccentErrorDark2 => GetDarkShade(AccentError, 2);
    public virtual Color AccentErrorDark3 => GetDarkShade(AccentError, 3);
    public virtual Color AccentInfoDark1 => GetDarkShade(AccentInfo, 1);
    public virtual Color AccentInfoDark2 => GetDarkShade(AccentInfo, 2);
    public virtual Color AccentInfoDark3 => GetDarkShade(AccentInfo, 3);
    public virtual Color AccentDestructiveDark1 => GetDarkShade(AccentDestructive, 1);
    public virtual Color AccentDestructiveDark2 => GetDarkShade(AccentDestructive, 2);
    public virtual Color AccentDestructiveDark3 => GetDarkShade(AccentDestructive, 3);
    public virtual Color AccentSubtleDark1 => GetDarkShade(AccentSubtle, 1);
    public virtual Color AccentSubtleDark2 => GetDarkShade(AccentSubtle, 2);
    public virtual Color AccentSubtleDark3 => GetDarkShade(AccentSubtle, 3);
    public virtual Color AccentNeutralDark1 => GetDarkShade(AccentNeutral, 1);
    public virtual Color AccentNeutralDark2 => GetDarkShade(AccentNeutral, 2);
    public virtual Color AccentNeutralDark3 => GetDarkShade(AccentNeutral, 3);
    public virtual Color AccentBorderDark1 => GetDarkShade(AccentBorder, 1);
    public virtual Color AccentBorderDark2 => GetDarkShade(AccentBorder, 2);
    public virtual Color AccentBorderDark3 => GetDarkShade(AccentBorder, 3);

    // Derived accent shades - Light (9 accents x 3)
    public virtual Color AccentPrimaryLight1 => GetLightShade(AccentPrimary, 1);
    public virtual Color AccentPrimaryLight2 => GetLightShade(AccentPrimary, 2);
    public virtual Color AccentPrimaryLight3 => GetLightShade(AccentPrimary, 3);
    public virtual Color AccentSuccessLight1 => GetLightShade(AccentSuccess, 1);
    public virtual Color AccentSuccessLight2 => GetLightShade(AccentSuccess, 2);
    public virtual Color AccentSuccessLight3 => GetLightShade(AccentSuccess, 3);
    public virtual Color AccentWarningLight1 => GetLightShade(AccentWarning, 1);
    public virtual Color AccentWarningLight2 => GetLightShade(AccentWarning, 2);
    public virtual Color AccentWarningLight3 => GetLightShade(AccentWarning, 3);
    public virtual Color AccentErrorLight1 => GetLightShade(AccentError, 1);
    public virtual Color AccentErrorLight2 => GetLightShade(AccentError, 2);
    public virtual Color AccentErrorLight3 => GetLightShade(AccentError, 3);
    public virtual Color AccentInfoLight1 => GetLightShade(AccentInfo, 1);
    public virtual Color AccentInfoLight2 => GetLightShade(AccentInfo, 2);
    public virtual Color AccentInfoLight3 => GetLightShade(AccentInfo, 3);
    public virtual Color AccentDestructiveLight1 => GetLightShade(AccentDestructive, 1);
    public virtual Color AccentDestructiveLight2 => GetLightShade(AccentDestructive, 2);
    public virtual Color AccentDestructiveLight3 => GetLightShade(AccentDestructive, 3);
    public virtual Color AccentSubtleLight1 => GetLightShade(AccentSubtle, 1);
    public virtual Color AccentSubtleLight2 => GetLightShade(AccentSubtle, 2);
    public virtual Color AccentSubtleLight3 => GetLightShade(AccentSubtle, 3);
    public virtual Color AccentNeutralLight1 => GetLightShade(AccentNeutral, 1);
    public virtual Color AccentNeutralLight2 => GetLightShade(AccentNeutral, 2);
    public virtual Color AccentNeutralLight3 => GetLightShade(AccentNeutral, 3);
    public virtual Color AccentBorderLight1 => GetLightShade(AccentBorder, 1);
    public virtual Color AccentBorderLight2 => GetLightShade(AccentBorder, 2);
    public virtual Color AccentBorderLight3 => GetLightShade(AccentBorder, 3);

    // Derived shade brushes
    public virtual SolidColorBrush AccentPrimaryDark1Brush => new(AccentPrimaryDark1);
    public virtual SolidColorBrush AccentPrimaryDark2Brush => new(AccentPrimaryDark2);
    public virtual SolidColorBrush AccentPrimaryDark3Brush => new(AccentPrimaryDark3);
    public virtual SolidColorBrush AccentPrimaryLight1Brush => new(AccentPrimaryLight1);
    public virtual SolidColorBrush AccentPrimaryLight2Brush => new(AccentPrimaryLight2);
    public virtual SolidColorBrush AccentPrimaryLight3Brush => new(AccentPrimaryLight3);
    public virtual SolidColorBrush AccentSuccessDark1Brush => new(AccentSuccessDark1);
    public virtual SolidColorBrush AccentSuccessDark2Brush => new(AccentSuccessDark2);
    public virtual SolidColorBrush AccentSuccessDark3Brush => new(AccentSuccessDark3);
    public virtual SolidColorBrush AccentSuccessLight1Brush => new(AccentSuccessLight1);
    public virtual SolidColorBrush AccentSuccessLight2Brush => new(AccentSuccessLight2);
    public virtual SolidColorBrush AccentSuccessLight3Brush => new(AccentSuccessLight3);
    public virtual SolidColorBrush AccentWarningDark1Brush => new(AccentWarningDark1);
    public virtual SolidColorBrush AccentWarningDark2Brush => new(AccentWarningDark2);
    public virtual SolidColorBrush AccentWarningDark3Brush => new(AccentWarningDark3);
    public virtual SolidColorBrush AccentWarningLight1Brush => new(AccentWarningLight1);
    public virtual SolidColorBrush AccentWarningLight2Brush => new(AccentWarningLight2);
    public virtual SolidColorBrush AccentWarningLight3Brush => new(AccentWarningLight3);
    public virtual SolidColorBrush AccentErrorDark1Brush => new(AccentErrorDark1);
    public virtual SolidColorBrush AccentErrorDark2Brush => new(AccentErrorDark2);
    public virtual SolidColorBrush AccentErrorDark3Brush => new(AccentErrorDark3);
    public virtual SolidColorBrush AccentErrorLight1Brush => new(AccentErrorLight1);
    public virtual SolidColorBrush AccentErrorLight2Brush => new(AccentErrorLight2);
    public virtual SolidColorBrush AccentErrorLight3Brush => new(AccentErrorLight3);
    public virtual SolidColorBrush AccentInfoDark1Brush => new(AccentInfoDark1);
    public virtual SolidColorBrush AccentInfoDark2Brush => new(AccentInfoDark2);
    public virtual SolidColorBrush AccentInfoDark3Brush => new(AccentInfoDark3);
    public virtual SolidColorBrush AccentInfoLight1Brush => new(AccentInfoLight1);
    public virtual SolidColorBrush AccentInfoLight2Brush => new(AccentInfoLight2);
    public virtual SolidColorBrush AccentInfoLight3Brush => new(AccentInfoLight3);
    public virtual SolidColorBrush AccentDestructiveDark1Brush => new(AccentDestructiveDark1);
    public virtual SolidColorBrush AccentDestructiveDark2Brush => new(AccentDestructiveDark2);
    public virtual SolidColorBrush AccentDestructiveDark3Brush => new(AccentDestructiveDark3);
    public virtual SolidColorBrush AccentDestructiveLight1Brush => new(AccentDestructiveLight1);
    public virtual SolidColorBrush AccentDestructiveLight2Brush => new(AccentDestructiveLight2);
    public virtual SolidColorBrush AccentDestructiveLight3Brush => new(AccentDestructiveLight3);
    public virtual SolidColorBrush AccentSubtleDark1Brush => new(AccentSubtleDark1);
    public virtual SolidColorBrush AccentSubtleDark2Brush => new(AccentSubtleDark2);
    public virtual SolidColorBrush AccentSubtleDark3Brush => new(AccentSubtleDark3);
    public virtual SolidColorBrush AccentSubtleLight1Brush => new(AccentSubtleLight1);
    public virtual SolidColorBrush AccentSubtleLight2Brush => new(AccentSubtleLight2);
    public virtual SolidColorBrush AccentSubtleLight3Brush => new(AccentSubtleLight3);
    public virtual SolidColorBrush AccentNeutralDark1Brush => new(AccentNeutralDark1);
    public virtual SolidColorBrush AccentNeutralDark2Brush => new(AccentNeutralDark2);
    public virtual SolidColorBrush AccentNeutralDark3Brush => new(AccentNeutralDark3);
    public virtual SolidColorBrush AccentNeutralLight1Brush => new(AccentNeutralLight1);
    public virtual SolidColorBrush AccentNeutralLight2Brush => new(AccentNeutralLight2);
    public virtual SolidColorBrush AccentNeutralLight3Brush => new(AccentNeutralLight3);
    public virtual SolidColorBrush AccentBorderDark1Brush => new(AccentBorderDark1);
    public virtual SolidColorBrush AccentBorderDark2Brush => new(AccentBorderDark2);
    public virtual SolidColorBrush AccentBorderDark3Brush => new(AccentBorderDark3);
    public virtual SolidColorBrush AccentBorderLight1Brush => new(AccentBorderLight1);
    public virtual SolidColorBrush AccentBorderLight2Brush => new(AccentBorderLight2);
    public virtual SolidColorBrush AccentBorderLight3Brush => new(AccentBorderLight3);

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

    private static Color GetDarkShade(Color baseColor, int shade)
    {
        var steps = new[] { 28.5, 49.0, 74.5 };
        var hsl = baseColor.ToHsl();
        return new HslColor(hsl.A, hsl.H, hsl.S, hsl.L - (steps[shade - 1] / 255.0)).ToRgb();
    }

    private static Color GetLightShade(Color baseColor, int shade)
    {
        var steps = new[] { 39.0, 70.0, 103.0 };
        var hsl = baseColor.ToHsl();
        return new HslColor(hsl.A, hsl.H, hsl.S, hsl.L + (steps[shade - 1] / 255.0)).ToRgb();
    }

    #endregion Methods
}

/// <summary>
/// Dark variant of the default theme. Overrides surface/overlay tokens and
/// the destructive accent; all other accent tokens remain identical to the
/// light theme.
/// </summary>
[Theme]
public class DefaultThemeDark : DefaultTheme
{
    #region Properties

    public override string ThemeName => "Default Dark";

    // Surface tokens
    public override Color Surface => Color.Parse("#1E1E2E");
    public override Color OnSurface => Color.Parse("#E0E0F0");
    public override Color OnSurfaceDim => Color.Parse("#8888A0");
    public override Color SurfaceOverlay => Color.Parse("#FFFFFF");

    // Destructive accent (darker red)
    public override Color AccentDestructive => Color.Parse("#C17070");

    #endregion Properties
}
