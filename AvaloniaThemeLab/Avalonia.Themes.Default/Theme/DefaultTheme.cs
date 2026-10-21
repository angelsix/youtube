using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;

namespace Avalonia.Themes.Default;

/// <summary>
/// Design token values for the Default theme. Marked with <c>[Theme]</c> so the
/// AngelSix.ThemeEngine source generator emits a markup extension for every public
/// property.
/// </summary>
/// <remarks>
/// <para>
/// All properties are <c>virtual</c> so subclasses can override individual token
/// values for alternative palettes (e.g. dark mode).
/// </para>
/// <para>
/// The class is <c>partial</c> because most of it is generated. A <c>[ColorRamp]</c> seed expands
/// into its ramp, twenty stage colors, twenty-one brushes and — for the fallback hue — the neutral
/// aliases; a <c>[SizeScale]</c> base expands into its ladder. What is left here is the material
/// that cannot be derived: the seed colors, the base sizes and the handful of values that are
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
    /// <c>PrimaryDark3</c> as its text color gets a readable one in either palette.
    /// </summary>
    /// <remarks>
    /// Settable, so a consumer can flip a live instance into its mirrored palette — e.g. for a
    /// scoped dark region.
    /// </remarks>
    public virtual bool IsDark { get; set; } = false;

    #endregion Identity

    #region Render Style

    /// <summary>
    /// Whether controls draw the way the web does: a control does not clip to its own bounds (CSS
    /// <c>overflow: visible</c>), and a grown hover state paints above its neighbours. Off restores
    /// Avalonia's defaults: every templated control clips, and nothing is lifted.
    /// </summary>
    /// <remarks>
    /// Clipping stays where something is a viewport: a scroll presenter clips on its own, and a
    /// template that slides content (TransitioningContentControl) clips its own root. See ThemeRules.md.
    /// </remarks>
    public virtual bool WebRenderStyle => true;

    /// <summary>
    /// ClipToBounds for every templated control, as a setter in each control theme, so a consumer's
    /// ControlTheme, Style or local value overrides it like any other theme value.
    /// </summary>
    public virtual bool ControlClipToBounds => !WebRenderStyle;

    /// <summary>
    /// ZIndex of a control in a state that grows it, so the growth paints over the sibling it overlaps.
    /// ZIndex orders siblings of one parent only.
    /// </summary>
    public virtual int HoverZIndex => WebRenderStyle ? 1 : 0;

    /// <summary>
    /// Whether a focused ACCENTED control takes the dedicated focus colour (<c>AccentFocus</c>) for its
    /// border. Off (the default) the focus colour is reserved for NEUTRAL controls: a Warn, Success or
    /// other accented control keeps its own hue when focused, drawn at the focus thickness, so focus is
    /// still announced without borrowing the focus role's color. On restores today's behaviour: every
    /// focused control, whatever its hue, takes <c>{color:AccentFocusBrush}</c>.
    /// </summary>
    /// <remarks>
    /// Consumed by the {default:FocusBrush} and {default:FocusThickness} extensions
    /// (Theme/FocusBrush.cs), which a control's focus state uses instead of the raw focus tokens.
    /// Those extensions read this switch off the ACTIVE THEME INSTANCE and combine it with the
    /// TARGET ELEMENT'S EFFECTIVE hue, so one override here flips every focused accented control at
    /// once. Documented as Rule 21 in ThemeRules.md. A downstream theme flips it by overriding this
    /// property to return true.
    /// </remarks>
    public virtual bool FocusColorOnAccents => false;

    #endregion Render Style

    #region Seed Colors

    // Every color in the theme is a hue with a ramp. AccentNeutral is the palette's default —
    // [Theme(FallbackHue = "Neutral")] above — so a control that sets no Accent.Kind draws itself
    // from this one. Setting Accent.Kind is what opts a control into any of the others.
    //
    // [AccentHue] marks it a hue for the accent surface; [ColorRamp] is what makes the generator
    // expand it. Neutral additionally carries the overlay opacities, because the translucent tints
    // drawn *over* the palette — shadows and scrims — are built from its darkest stage.
    //
    // Neutral's seed is the ink, and dark mode's ink is light, so its dark half re-centres on the
    // seed's reflection (255 - 0x2E = 0xD1). Without it the dark ramp centres on #2E2E2E, beside the
    // #222222 canvas, and every border, fill and dim text drawn from it collapses into the canvas.
    // Dark Light stages travel towards the canvas (the Surface hue's DarkSeed) by engine default.
    //
    // HueAware puts stage N at one OKLCH lightness in every hue, so a role pinned to a stage means
    // the same contrast whatever Accent.Kind a control carries: Dark3 is 7.7-8.6:1 on the light
    // canvas and 6.0-6.6:1 on the dark one in all eight hues. The seed gives only hue and chroma,
    // so the seed color itself (Accent{Hue}) is unchanged. Surface stays off: its stages are the
    // canvas's own tints. A downstream theme overriding a seed must repeat HueAware = true.

    [AccentHue]
    [ColorRamp(OverlayLevels = [0.08, 0.16, 0.32], DarkSeed = "#D1D1D1", HueAware = true)]
    public virtual Color AccentNeutral => Color.Parse("#2E2E2E");

    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentPrimary => Color.Parse("#5BA3C9");
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentSuccess => Color.Parse("#6DB87E");
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentWarning => Color.Parse("#E0B860");
    // Favourite (task 101): a vibrant golden-yellow that sits between Warning (a muted gold) and the
    // oranges (Info). Declared like every other hue, so the source generator derives its full ramp,
    // its AccentKind member and its {color:Favourite…} extensions — Accent.Kind="Favourite" therefore
    // works everywhere with no other wiring. Its light ink stages are hand-placed below (task 93's
    // pattern) so the new hue ships vibrant in the light palette rather than settling near-black.
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentFavourite => Color.Parse("#FFC400");
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentError => Color.Parse("#D47A7A");
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentInfo => Color.Parse("#E89F4A");
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentDestructive => Color.Parse("#C17070");
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentSubtle => Color.Parse("#B088C8");

    // The surface hue seeds the application canvas. In the light palette it keeps the paper seed
    // (#fdfdfd) and mirrors as usual; in the dark palette the whole ramp is re-centred on the
    // DarkSeed literal, so it behaves as if the seed itself had been that color — every stage,
    // overlay and accent role derives off it, mirrored as usual. A downstream theme overriding
    // this seed must repeat the attribute including DarkSeed, or it loses the re-centring.
    [AccentHue]
    [ColorRamp(DarkSeed = "#222222")]
    public virtual Color AccentSurface => Color.Parse("#fdfdfd");

    // The focus ring is the one color that is deliberately not a hue: a focused control should
    // announce itself the same way whatever accent it carries, so this must not follow Accent.Kind.
    // Its brush is generated — every Color gets one — so there is nothing to declare but the color.
    //
    // There was an AccentBorder here too, for the resting border of every control. It was a mistake:
    // being a fixed color it bypassed the accent entirely, so an accented control drew a border
    // that ignored its own hue. Borders now use {color:BorderDefault}, which follows the hue.
    public virtual Color AccentFocus => Color.Parse("#bf4aF9");

    #endregion Seed Colors

    #region Light Palette Ink Stages (task 93)

    // The hue-aware ramp parks every stage at one shared OKLCH lightness, and its chroma is capped
    // at whatever the gamut carries there — so on the light palette the deep ink stages of the hued
    // accents settle near-black (Primary Dark3 #005677, Error Dark5 #5C0A16), while the dark
    // palette's mirrored versions read vibrant. Luke's second review: the LIGHT theme's dark accent
    // stages must be more saturated and less near-black; the dark theme is correct and stays
    // byte-identical.
    //
    // The engine exposes no light-side lever for this (HueAwareCentre and ExtremeAmount are fixed
    // constants; the seeds feed BOTH palettes, so retuning a seed would move the dark theme too).
    // These declarations therefore re-place the light palette's deep ink stages individually: each
    // keeps its stage's OKLCH hue, gains a little lightness (+0.03 .. +0.005, shallowest first) and
    // wears the full chroma the gamut allows at the new lightness. Declaring a stage here makes the
    // source generator withhold its own emission of that member (verified: the generated partial
    // simply omits it while keeping the Brush companion), so the body below IS the token — the
    // {color:...} extensions bind straight to it. In dark mode every body delegates to the same
    // ramp the generator would have used ({Hue}Ramp.Dark(N)), so the dark palette computes exactly
    // what it always did.
    //
    // Contrast floors held in every hue: the stage on the light canvas >= 4.5:1 (worst now 5.19,
    // Warning Dark3 after task 101 lifted Success and Warning out of the near-black mud; was 6.75
    // Success Dark3 under task 93, originally 7.7-8.6) and Light10 text on the stage >= 4.5:1.
    // Ordering within a ramp is preserved (each stage stays darker than the one above it). Neutral
    // is deliberately left alone: its grey ink is not "near-black" and its dim-text role reads
    // correctly as-is.

    public virtual Color AccentPrimaryDark3 => IsDark ? PrimaryRamp.Dark(3) : Color.Parse("#005F83");
    public virtual Color AccentPrimaryDark4 => IsDark ? PrimaryRamp.Dark(4) : Color.Parse("#004B69");
    public virtual Color AccentPrimaryDark5 => IsDark ? PrimaryRamp.Dark(5) : Color.Parse("#003950");
    public virtual Color AccentPrimaryDark6 => IsDark ? PrimaryRamp.Dark(6) : Color.Parse("#00283A");
    public virtual Color AccentPrimaryDark7 => IsDark ? PrimaryRamp.Dark(7) : Color.Parse("#001825");

    // Task 101: the light Success and Warning fills read too dark and muddy (most visible on a
    // checked CheckBox's fill). Lifted at the source so every control that draws those stages follows.
    // Dark3 doubles as the accent-text ink, so its binding floor is 4.5:1 against the light canvas
    // (stricter than the 3:1 a check glyph needs); each stage below keeps its hue and sits ~5.2:1 on
    // the canvas — clearly less muddy than the old 6.75/7.19 while holding both floors. The dark
    // palette still delegates to the engine ramp, so it stays byte-identical.
    public virtual Color AccentSuccessDark3 => IsDark ? SuccessRamp.Dark(3) : Color.Parse("#3A7848");
    public virtual Color AccentSuccessDark4 => IsDark ? SuccessRamp.Dark(4) : Color.Parse("#2E5E39");
    public virtual Color AccentSuccessDark5 => IsDark ? SuccessRamp.Dark(5) : Color.Parse("#214529");
    public virtual Color AccentSuccessDark6 => IsDark ? SuccessRamp.Dark(6) : Color.Parse("#152B1A");
    public virtual Color AccentSuccessDark7 => IsDark ? SuccessRamp.Dark(7) : Color.Parse("#08110A");

    public virtual Color AccentWarningDark3 => IsDark ? WarningRamp.Dark(3) : Color.Parse("#89661B");
    public virtual Color AccentWarningDark4 => IsDark ? WarningRamp.Dark(4) : Color.Parse("#694E14");
    public virtual Color AccentWarningDark5 => IsDark ? WarningRamp.Dark(5) : Color.Parse("#49360E");
    public virtual Color AccentWarningDark6 => IsDark ? WarningRamp.Dark(6) : Color.Parse("#291E08");
    public virtual Color AccentWarningDark7 => IsDark ? WarningRamp.Dark(7) : Color.Parse("#090602");

    public virtual Color AccentErrorDark3 => IsDark ? ErrorRamp.Dark(3) : Color.Parse("#A50025");
    public virtual Color AccentErrorDark4 => IsDark ? ErrorRamp.Dark(4) : Color.Parse("#83001D");
    public virtual Color AccentErrorDark5 => IsDark ? ErrorRamp.Dark(5) : Color.Parse("#660015");
    public virtual Color AccentErrorDark6 => IsDark ? ErrorRamp.Dark(6) : Color.Parse("#4B000C");
    public virtual Color AccentErrorDark7 => IsDark ? ErrorRamp.Dark(7) : Color.Parse("#310005");

    public virtual Color AccentInfoDark3 => IsDark ? InfoRamp.Dark(3) : Color.Parse("#7B4A00");
    public virtual Color AccentInfoDark4 => IsDark ? InfoRamp.Dark(4) : Color.Parse("#623A00");
    public virtual Color AccentInfoDark5 => IsDark ? InfoRamp.Dark(5) : Color.Parse("#4B2B00");
    public virtual Color AccentInfoDark6 => IsDark ? InfoRamp.Dark(6) : Color.Parse("#361E00");
    public virtual Color AccentInfoDark7 => IsDark ? InfoRamp.Dark(7) : Color.Parse("#231100");

    public virtual Color AccentDestructiveDark3 => IsDark ? DestructiveRamp.Dark(3) : Color.Parse("#A40028");
    public virtual Color AccentDestructiveDark4 => IsDark ? DestructiveRamp.Dark(4) : Color.Parse("#84001D");
    public virtual Color AccentDestructiveDark5 => IsDark ? DestructiveRamp.Dark(5) : Color.Parse("#660015");
    public virtual Color AccentDestructiveDark6 => IsDark ? DestructiveRamp.Dark(6) : Color.Parse("#4B000C");
    public virtual Color AccentDestructiveDark7 => IsDark ? DestructiveRamp.Dark(7) : Color.Parse("#310005");

    public virtual Color AccentSubtleDark3 => IsDark ? SubtleRamp.Dark(3) : Color.Parse("#8000AE");
    public virtual Color AccentSubtleDark4 => IsDark ? SubtleRamp.Dark(4) : Color.Parse("#66008C");
    public virtual Color AccentSubtleDark5 => IsDark ? SubtleRamp.Dark(5) : Color.Parse("#4E006D");
    public virtual Color AccentSubtleDark6 => IsDark ? SubtleRamp.Dark(6) : Color.Parse("#390050");
    public virtual Color AccentSubtleDark7 => IsDark ? SubtleRamp.Dark(7) : Color.Parse("#240034");

    // Favourite (task 101): a brand-new hue, so without these its light DeepInk would settle near-black
    // like every hued accent did before task 93 — reintroducing exactly the mudiness Luke rejected. Placed
    // with the same method as Success and Warning above: keep the seed's golden-yellow hue, land Dark3 at
    // ~5.3:1 on the light canvas (holds the 4.5:1 text floor and the 3:1 glyph floor), step down monotonically.
    // The dark palette delegates to the engine ramp, so it is computed by the standard HueAware path.
    public virtual Color AccentFavouriteDark3 => IsDark ? FavouriteRamp.Dark(3) : Color.Parse("#856600");
    public virtual Color AccentFavouriteDark4 => IsDark ? FavouriteRamp.Dark(4) : Color.Parse("#5E4900");
    public virtual Color AccentFavouriteDark5 => IsDark ? FavouriteRamp.Dark(5) : Color.Parse("#382B00");
    public virtual Color AccentFavouriteDark6 => IsDark ? FavouriteRamp.Dark(6) : Color.Parse("#120E00");
    public virtual Color AccentFavouriteDark7 => IsDark ? FavouriteRamp.Dark(7) : Color.Parse("#000000");

    #endregion Light Palette Ink Stages (task 93)

    #region Bright Solid-Fill Stages (task 131)

    // The four solid-fill roles name the FILLED-COLUMN stages of ThemeRules.md (Fill=Dark3, Hover=Dark4,
    // Pressed=Dark5, Text=Light10) — the exact stages the existing filled controls already draw. They are
    // pinned to real engine stages with NO hand-placed hex and NO IsDark branching, so every slot resolves
    // to the engine's own stage in BOTH palettes and the two palettes can never disagree. In light the
    // fills pick up the task-101 lifted ink stages (Dark3..Dark7); in dark they take the engine's mirrored
    // ramp. Measured across all nine hues, the Light10 glyph/text clears >= 4.5:1 on every rest/hover/
    // pressed fill in both palettes (worst: Warning light rest 5.11, Error dark rest 5.84), so ticks, dots,
    // icons and on-fill labels stay legible as the face deepens. Coloured TEXT is untouched: it keeps the
    // 4.5:1 ink stage from task 101 (Dark3).
    //
    // History: tasks 131/134 first tried to make the light fills "vivid" by hijacking spare ramp slots
    // (Light3/Dark10/Light11/Light12) and hand-placing light-only hexes there. That made the two palettes
    // disagree (near-black light ticks, washed dark checks) and invented checkbox-specific values, so those
    // overrides were deleted and the roles re-pointed at the genuine filled-column stages above.

    /// <summary>Solid-filled face (checked/toggled/starred): the filled-column REST stage, Dark3 — the same stage the existing filled controls draw today, taken from the engine's own ramp (no hand-placed hex, no IsDark branching).</summary>
    [AccentRole]
    internal virtual RampStage SolidFillDefault => RampStage.Dark3;

    /// <summary>Ink drawn ON a solid fill (tick, dot, icon, on-fill text): the filled-column TEXT stage, Light10 — the stage the theme already uses for text on filled faces.</summary>
    [AccentRole]
    internal virtual RampStage OnSolidFillInkDefault => RampStage.Light10;

    /// <summary>Solid-filled face under the pointer (checked:hover): the filled-column HOVER stage, Dark4 — one stage deeper than the rest, as Button's outlined hover steps through its stages.</summary>
    [AccentRole]
    internal virtual RampStage SolidFillHover => RampStage.Dark4;

    /// <summary>Solid-filled face while pressed (checked:pressed): the filled-column PRESSED stage, Dark5 — two stages deeper than the rest.</summary>
    [AccentRole]
    internal virtual RampStage SolidFillPressed => RampStage.Dark5;

    #endregion Bright Solid-Fill Stages (tasks 131, 134)

    #region Theme-Specific Defaults

    /// <summary>The application-wide background/canvas color.</summary>
    /// <remarks>
    /// <para>
    /// Reads <see cref="SurfaceSeed"/>, the effective seed the generator emits for the surface
    /// hue: the raw seed (<see cref="AccentSurface"/>) in light mode, the <c>DarkSeed</c> value
    /// in dark mode. The canvas is therefore the paper seed when light and exactly the
    /// <c>DarkSeed</c> literal when dark, with no <see cref="IsDark"/> branching anywhere in
    /// hand-written code. Consumers write <c>{color:SurfaceDefaultBrush}</c>: its brush
    /// companion is generated automatically (every Color gets one).
    /// </para>
    /// <para>
    /// This single property is the deliberate guard against proliferating paired
    /// brush/color/is-dark properties: one token, one spelling, both palettes — even though the
    /// two palettes now anchor on entirely different colors.
    /// </para>
    /// </remarks>
    public virtual Color SurfaceDefault => SurfaceSeed;

    // Accent roles: named aliases for one stage of the CONTROL'S OWN hue ramp. The property name
    // is the token and its RampStage value is the pinned stage; the generator emits two extensions
    // per role into AngelSix.ThemeEngine.Colors — {color:X} (the color) and {color:XBrush}
    // (the brush) — which resolve against whatever Accent.Kind the target carries at render time.
    // Unlike SurfaceDefault above, a role never freezes a color: it keeps following the hue and
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

    /// <summary>Outlined-control and container border: 3.16-3.71:1 on the canvas in every hue, light and dark, over the 3:1 non-text minimum.</summary>
    [AccentRole]
    internal virtual RampStage BorderDefault => RampStage.Light1;

    /// <summary>Outlined-control rest fill.</summary>
    [AccentRole]
    internal virtual RampStage BackgroundDefault => RampStage.Light10;

    /// <summary>Borderless-surface fill: the rest tint one shade deeper (Light9) for a surface that draws no border of its own.</summary>
    /// <remarks>
    /// A rest fill (BackgroundDefault, Light10) reads as a distinct surface only when a border outlines it; on a borderless
    /// surface it is nearly indistinguishable from the canvas. This role sits one stage darker than the rest fill and one lighter
    /// than the hover fill (HoverBackgroundDefault, Light8), so a borderless surface announces itself without competing with any
    /// interaction state. Like every role it follows the control's own hue. Distinct from SurfaceDefault, which is the canvas.
    /// </remarks>
    [AccentRole]
    internal virtual RampStage SurfaceBorderless => RampStage.Light9;

    /// <summary>Outlined-control hover fill.</summary>
    [AccentRole]
    internal virtual RampStage HoverBackgroundDefault => RampStage.Light8;

    /// <summary>Outlined-control hover text.</summary>
    [AccentRole]
    internal virtual RampStage HoverTextDefault => RampStage.Dark7;

    /// <summary>Outlined-control pressed fill.</summary>
    [AccentRole]
    internal virtual RampStage PressedBackgroundDefault => RampStage.Light6;

    /// <summary>Dimmed text: placeholders, captions, secondary labels, disabled text.</summary>
    /// <remarks>
    /// A role, so it follows the control's hue. That is safe only because the ramps are hue-aware:
    /// Dark3 reads 7.7-8.6:1 on the light canvas and 6.0-6.6:1 on the dark one in every hue, against
    /// 20.4:1 and 13.6:1 for TextDefault, so it is always readable and always visibly dimmer.
    /// </remarks>
    [AccentRole]
    internal virtual RampStage DimTextDefault => RampStage.Dark3;

    /// <summary>The stage a focused ACCENTED control draws its border at while <see cref="FocusColorOnAccents"/> is off.</summary>
    /// <remarks>
    /// The hue's accent-text stage (Dark3, the same ink Label and HyperlinkButton use): the same hue as the
    /// resting border, heavier, so focus is announced in the control's own colour rather than the focus
    /// role's, and it reads as the same colour as that hue's text everywhere. It was Dark2 until 20 Oct
    /// 2026: the light palette's hand-lifted ink band starts at Dark3, so light Dark2 sat DARKER than Dark3
    /// (Warning #785900 vs #89661B) and focused accented controls looked muddy. Clears the non-text floor
    /// in every hue (Dark3 is >= 4.5:1). Consumed by {default:FocusBrush} (Theme/FocusBrush.cs) as
    /// Accent{Hue}{this stage}Brush — see ThemeRules.md, Rule 21.
    /// </remarks>
    [AccentRole]
    internal virtual RampStage AccentFocusBorder => RampStage.Dark3;

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
    [SizeScale(Multipliers = [6, 12, 24, 32],
               Types = [typeof(CornerRadius), typeof(double)],
               TypeSuffixes = ["", "Double"])]
    public virtual double Radius => BaseSize;

    /// <summary>
    /// Uniform border/outline thicknesses, emitted twice over: as <c>Thickness</c> for
    /// <c>Border.BorderThickness</c> and friends, and as a bare <c>double</c> for the slots that take
    /// a plain number — a track's <c>Height</c>, a dot's <c>Width</c>, a glyph's <c>StrokeThickness</c>.
    /// A Thickness token in one of those slots silently resolves to 0 (the wrong-type trap), so a
    /// plain-number slot must read the <c>…Double</c> twin, exactly as <see cref="Radius"/> does.
    /// </summary>
    [SizeScale(Multipliers = [1, 2, 3, 4, 6],
               Types = [typeof(Thickness), typeof(double)],
               TypeSuffixes = ["", "Double"])]
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

    /// <summary>
    /// The default content padding for interactive controls: the breathing room between a control's
    /// border and its content. Derived from the spacing ladder's default step, so retuning spacing
    /// retunes padding with it. Compose per-edge with {size:Edges}; button-like faces step the
    /// horizontal edges up with <see cref="ContentPaddingWide"/>.
    /// </summary>
    public virtual double ContentPadding => SpacingLg;

    /// <summary>
    /// The horizontal content padding for button-like faces (Button, RepeatButton, ToggleButton,
    /// SplitButton, DropDownButton, HyperlinkButton): one spacing-ladder step above
    /// <see cref="ContentPadding"/>, because a wide face reads better with a wide label gap.
    /// Vertical edges stay on <see cref="ContentPadding"/>.
    /// </summary>
    public virtual double ContentPaddingWide => SpacingXl;

    // Typeface. The weights are a named vocabulary rather than anything derived from the family,
    // but generating them keeps the names identical across themes, so a control style written
    // against {theme:FontWeightSemiBold} survives a theme swap. Medium sits between Regular and
    // SemiBold for the emphasis a capsule button label carries without reading as a heading. The
    // labels are stated because SemiBold shares its enum value with DemiBold, and an explicit list
    // without labels would be spelt FontWeightDemiBold.
    [FontWeights(Weights = [FontWeight.Normal, FontWeight.Medium, FontWeight.SemiBold, FontWeight.Bold],
                 Labels = ["Regular", "Medium", "SemiBold", "Bold"])]
    // WithInterFont() registers Inter as the "fonts:Inter" collection only. The bare name "Inter" matched no
    // registered family, so every text fell back to the platform default (Helvetica on macOS) until 29 Sep 2026.
    public virtual FontFamily FontFamily => new("fonts:Inter#Inter, $Default");

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
    public virtual HorizontalAlignment ControlHorizontalContentAlignment => HorizontalAlignment.Left;
    public virtual VerticalAlignment ControlVerticalContentAlignment => VerticalAlignment.Center;

    // Button content alignment (for every button-like control: Button, RepeatButton, ToggleButton, SplitButton, ...)
    // A label or icon sits in the middle of the face it presses, whatever the general content alignment is
    public virtual HorizontalAlignment ButtonHorizontalContentAlignment => HorizontalAlignment.Center;

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
