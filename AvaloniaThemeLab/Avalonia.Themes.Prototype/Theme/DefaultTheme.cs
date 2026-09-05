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
/// <para>
/// All properties are <c>virtual</c> so subclasses can override individual token
/// values for alternative palettes (e.g. dark mode).
/// </para>
/// <para>
/// The class is <c>partial</c> because most of it is generated. A <c>[ColourRamp]</c> seed expands
/// into its ramp, twenty stage colours, twenty-one brushes and — for the fallback hue — the neutral
/// aliases; a <c>[SizeScale]</c> base expands into its ladder. What is left here is the material
/// that cannot be derived: the seed colours, the base sizes and the handful of values that are
/// genuinely one-offs.
/// </para>
/// </remarks>
[Theme(FallbackHue = "Neutral")]
public partial class DefaultTheme
{
    #region Identity

    public virtual string ThemeName => "Default";

    /// <summary>
    /// Whether this is a dark palette. Selects the mirrored ramp, so a control theme asking for
    /// <c>PrimaryDark3</c> as its text colour gets a readable one in either palette.
    /// </summary>
    /// <remarks>
    /// Settable, so a consumer can flip a live instance into its mirrored palette — e.g. for a
    /// scoped dark region.
    /// </remarks>
    public virtual bool IsDark { get; set; } = false;

    #endregion Identity

    #region Seed Colours

    // Every colour in the theme is a hue with a ramp. AccentNeutral is the palette's default —
    // [Theme(FallbackHue = "Neutral")] above — so a control that sets no Accent.Kind draws itself
    // from this one. Setting Accent.Kind is what opts a control into any of the others.
    //
    // [AccentHue] marks it a hue for the accent surface; [ColourRamp] is what makes the generator
    // expand it. Neutral additionally carries the overlay opacities, because the translucent tints
    // drawn *over* the palette — shadows and scrims — are built from its darkest stage.

    [AccentHue]
    [ColourRamp(OverlayLevels = [0.08, 0.16, 0.32])]
    public virtual Color AccentNeutral => Color.Parse("#2E2E2E");

    [AccentHue, ColourRamp] public virtual Color AccentPrimary => Color.Parse("#5BA3C9");
    [AccentHue, ColourRamp] public virtual Color AccentSuccess => Color.Parse("#6DB87E");
    [AccentHue, ColourRamp] public virtual Color AccentWarning => Color.Parse("#E0B860");
    [AccentHue, ColourRamp] public virtual Color AccentError => Color.Parse("#D47A7A");
    [AccentHue, ColourRamp] public virtual Color AccentInfo => Color.Parse("#E89F4A");
    [AccentHue, ColourRamp] public virtual Color AccentDestructive => Color.Parse("#C17070");
    [AccentHue, ColourRamp] public virtual Color AccentSubtle => Color.Parse("#B088C8");

    // The surface hue seeds the application canvas. In the light palette it keeps the paper seed
    // (#fdfdfd) and mirrors as usual; in the dark palette the whole ramp is re-centred on the
    // DarkSeed literal, so it behaves as if the seed itself had been that colour — every stage,
    // overlay and accent role derives off it, mirrored as usual. A downstream theme overriding
    // this seed must repeat the attribute including DarkSeed, or it loses the re-centring.
    [AccentHue]
    [ColourRamp(DarkSeed = "#032030")]
    public virtual Color AccentSurface => Color.Parse("#fdfdfd");

    // The focus ring is the one colour that is deliberately not a hue: a focused control should
    // announce itself the same way whatever accent it carries, so this must not follow Accent.Kind.
    // Its brush is generated — every Color gets one — so there is nothing to declare but the colour.
    //
    // There was an AccentBorder here too, for the resting border of every control. It was a mistake:
    // being a fixed colour it bypassed the accent entirely, so an accented control drew a border
    // that ignored its own hue. Borders now use {colour:AccentBrush Light6}, which follows the hue.
    public virtual Color AccentFocus => Color.Parse("#bf4aF9");

    #endregion Seed Colours

    #region Theme-Specific Defaults

    /// <summary>The application-wide background/canvas colour.</summary>
    /// <remarks>
    /// <para>
    /// Reads <see cref="SurfaceSeed"/>, the effective seed the generator emits for the surface
    /// hue: the raw seed (<see cref="AccentSurface"/>) in light mode, the <c>DarkSeed</c> value
    /// in dark mode. The canvas is therefore the paper seed when light and exactly the
    /// <c>DarkSeed</c> literal when dark, with no <see cref="IsDark"/> branching anywhere in
    /// hand-written code. Consumers write <c>{colour:SurfaceDefaultBrush}</c>: its brush
    /// companion is generated automatically (every Color gets one).
    /// </para>
    /// <para>
    /// This single property is the deliberate guard against proliferating paired
    /// brush/color/is-dark properties: one token, one spelling, both palettes — even though the
    /// two palettes now anchor on entirely different colours.
    /// </para>
    /// </remarks>
    public virtual Color SurfaceDefault => SurfaceSeed;

    // Accent roles: named aliases for one stage of the CONTROL'S OWN hue ramp. The property name
    // is the token and its RampStage value is the pinned stage; the generator emits two extensions
    // per role into AngelSix.ThemeEngine.Colours — {colour:X} (the colour) and {colour:XBrush}
    // (the brush) — which resolve against whatever Accent.Kind the target carries at render time.
    // Unlike SurfaceDefault above, a role never freezes a colour: it keeps following the hue and
    // the palette while giving every outlined control one shared spelling. Retuning a role
    // globally is editing its RampStage here and rebuilding.
    //
    // Deliberately internal: the generator bakes each stage into the generated extensions at
    // build time, so nothing ever reads these properties at runtime — they exist purely as the
    // single editable source of truth. internal keeps them out of the public API surface (and
    // every consumer's IntelliSense) while staying overridable inside this assembly.

    /// <summary>Outlined-control text ink.</summary>
    [AccentRole]
    internal virtual RampStage TextDefault => RampStage.Dark9;

    /// <summary>Outlined-control border.</summary>
    [AccentRole]
    internal virtual RampStage BorderDefault => RampStage.Light6;

    /// <summary>Outlined-control rest fill.</summary>
    [AccentRole]
    internal virtual RampStage BackgroundDefault => RampStage.Light10;

    /// <summary>Outlined-control hover fill.</summary>
    [AccentRole]
    internal virtual RampStage HoverBackgroundDefault => RampStage.Light8;

    /// <summary>Outlined-control hover text.</summary>
    [AccentRole]
    internal virtual RampStage HoverTextDefault => RampStage.Dark7;

    /// <summary>Outlined-control pressed fill.</summary>
    [AccentRole]
    internal virtual RampStage PressedBackgroundDefault => RampStage.Light6;

    /// <summary>Dimmed text: placeholders, captions, secondary labels.</summary>
    [AccentRole]
    internal virtual RampStage DimTextDefault => RampStage.Light2;

    #endregion Theme-Specific Defaults

    #region Size Scales

    /// <summary>Base size multiplier (user-adjustable 0.75-2.0). Every scale below rides on it.</summary>
    public virtual double BaseSize => 1.0;

    // One base per ladder; the multipliers spell out the steps. The ladders are not uniform —
    // spacing breaks its own doubling at the fourth step — so the factors are listed literally
    // rather than derived from a ratio.

    /// <summary>Spacing ladder: SpacingSm through SpacingXxl.</summary>
    [SizeScale(Multipliers = [2, 4, 8, 12, 16])]
    public virtual double Spacing => BaseSize;

    /// <summary>Type ladder: FontSizeSm through FontSizeXxl.</summary>
    [SizeScale(Multipliers = [12, 14, 16, 20, 24])]
    public virtual double FontSize => BaseSize;

    /// <summary>Control height ladder: ControlHeightSm, Md and Lg.</summary>
    [SizeScale(Multipliers = [32, 40, 48])]
    public virtual double ControlHeight => BaseSize;

    /// <summary>
    /// Corner radius ladder, emitted twice over: as <c>CornerRadius</c> for Border and friends, and
    /// as a bare <c>double</c> for <c>Shape.RadiusX/Y</c>, which does not take a CornerRadius.
    /// </summary>
    [SizeScale(Multipliers = [3, 6, 12, 16],
               Types = [typeof(CornerRadius), typeof(double)],
               TypeSuffixes = ["", "Double"])]
    public virtual double Radius => BaseSize;

    /// <summary>Uniform border/outline thicknesses: ThicknessSm through ThicknessXxl.</summary>
    [SizeScale(Multipliers = [1, 2, 3, 4, 6], Types = new[] { typeof(Thickness) })]
    public virtual double Thickness => BaseSize;

    /// <summary>Icon ladder: IconSizeSm through IconSizeXl. Md is the everyday glyph size.</summary>
    [SizeScale(Multipliers = [12, 16, 24, 32])]
    public virtual double IconSize => BaseSize;

    /// <summary>
    /// Widths for things that are surfaces rather than controls — a notification, a tooltip, a
    /// flyout pane, a file dialog.
    /// </summary>
    /// <remarks>
    /// These used to be spelled <c>{size:Scaled Value={size:ControlMinWidth}, By=5}</c>, which
    /// borrowed an unrelated token as an arithmetic base: a notification card's width has nothing
    /// to do with a button's minimum width, and the two could never move independently.
    /// </remarks>
    [SizeScale(Multipliers = [256, 320, 384], Labels = ["Sm", "Md", "Lg"])]
    public virtual double SurfaceWidth => BaseSize;

    #endregion Size Scales

    #region One-off Metrics

    /// <summary>
    /// The narrowest an interactive control may be, so a short-labelled button still presents a
    /// sane hit target. A floor, not a ladder — a control is either wide enough or it is not.
    /// </summary>
    public virtual double ControlMinWidth => 64 * BaseSize;

    // Typeface. The weights are a named vocabulary rather than anything derived from the family,
    // but generating them keeps the names identical across themes, so a control style written
    // against {theme:FontWeightSemiBold} survives a theme swap.
    [FontWeights]
    public virtual FontFamily FontFamily => new("Inter, $Default");

    // Accent visual properties (for highlighted/prominent elements)
    public virtual double AccentBorderStrokeThickness => 2 * BaseSize;

    // State visual properties
    public virtual double DisabledOpacity => 0.3;
    public virtual double PressedScale => 0.98;

    /// <summary>
    /// Animation timing, in milliseconds: AnimationFastMs, AnimationNormalMs and AnimationSlowMs.
    /// The base is the normal duration, so retuning the whole feel of the theme is one number.
    /// </summary>
    [SizeScale(Multipliers = [0.5, 1, 2],
               Labels = ["FastMs", "NormalMs", "SlowMs"],
               Types = [typeof(TimeSpan)])]
    public virtual double Animation => 150;

    // Control alignment defaults (for interactive controls: buttons, inputs, pickers, etc.)
    public virtual HorizontalAlignment ControlHorizontalAlignment => HorizontalAlignment.Left;
    public virtual VerticalAlignment ControlVerticalAlignment => VerticalAlignment.Center;
    public virtual HorizontalAlignment ControlHorizontalContentAlignment => HorizontalAlignment.Center;
    public virtual VerticalAlignment ControlVerticalContentAlignment => VerticalAlignment.Center;

    // Container alignment defaults (for structural elements that stretch to fill: list items, panels, etc.)
    public virtual HorizontalAlignment ContainerHorizontalAlignment => HorizontalAlignment.Stretch;
    public virtual VerticalAlignment ContainerVerticalAlignment => VerticalAlignment.Stretch;

    #endregion One-off Metrics

    #region Icon Gallery

    /// <summary>
    /// The icon gallery for this theme. Override this to swap the entire icon set.
    /// XAML accesses individual glyphs via <c>{icons:GlyphName}</c> (markup extension
    /// in <c>AngelSix.ThemeEngine.Generated</c>), which drills into this property.
    /// </summary>
    public virtual ThemeIconGallery Icons => new();

    #endregion Icon Gallery
}
