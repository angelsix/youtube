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
    // Task 148: Warning is an AMBER/ORANGE warning, distinct from Favourite's clear yellow. It used to
    // sit at a muted gold (#E0B860) that read identically to Favourite in both palettes; the seed moves
    // towards orange so the two hues separate in the engine's own ramps (no per-control hex).
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentWarning => Color.Parse("#E89A3C");
    // Favourite (task 101): a vibrant golden-yellow that sits between Warning (now amber/orange) and the
    // blues (Info). Declared like every other hue, so the source generator derives its full ramp,
    // its AccentKind member and its {color:Favourite…} extensions — Accent.Kind="Favourite" therefore
    // works everywhere with no other wiring. Its light ink stages are hand-placed below (task 93's
    // pattern) so the new hue ships vibrant in the light palette rather than settling near-black.
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentFavourite => Color.Parse("#FFC400");
    // Task 148: Error stays the BRIGHTEST red. Destructive rotates toward magenta (raspberry) and deepens,
    // so the two reds finally differ instead of reading as one.
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentError => Color.Parse("#D47A7A");
    // Task 148: Info was a muddy orange (#E89F4A) that collided with Warning/Favourite. Light blue is taken
    // by Primary, so Info takes INDIGO — the hue furthest from Primary and every other hue (measured min
    // RGB-Euclidean distance to the other seven seeds: 77, versus teal 60 / cyan-green 72). Clearly cool,
    // clearly informational, unmistakably not the primary sky-blue.
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentInfo => Color.Parse("#4B5BE0");
    // Task 148: Destructive rotates hard toward MAGENTA (raspberry, hue ~316°) and deepens, so it finally
    // differs from Error's pure red in BOTH palettes — including the dark one, where the engine's shared
    // lightness compresses chroma and a mild shift stayed too close. A stronger rotation clears that.
    [AccentHue, ColorRamp(HueAware = true)] public virtual Color AccentDestructive => Color.Parse("#A02A80");
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

    // Task 148: the old gold band (#89661B…) read identically to Favourite's golden-yellow in the light
    // palette. Replaced with an amber/orange band that keeps the raised seed's hue, lands Dark3 at ~4.7:1
    // on the light canvas (holds the 4.5:1 accent-text floor) and steps down monotonically. Favourite's
    // golden band below is untouched, so the two hues now differ by 53 RGB units at Dark3 (was 0).
    public virtual Color AccentWarningDark3 => IsDark ? WarningRamp.Dark(3) : Color.Parse("#B05E1E");
    public virtual Color AccentWarningDark4 => IsDark ? WarningRamp.Dark(4) : Color.Parse("#8D4B18");
    public virtual Color AccentWarningDark5 => IsDark ? WarningRamp.Dark(5) : Color.Parse("#6A3812");
    public virtual Color AccentWarningDark6 => IsDark ? WarningRamp.Dark(6) : Color.Parse("#46260C");
    public virtual Color AccentWarningDark7 => IsDark ? WarningRamp.Dark(7) : Color.Parse("#231306");

    public virtual Color AccentErrorDark3 => IsDark ? ErrorRamp.Dark(3) : Color.Parse("#A50025");
    public virtual Color AccentErrorDark4 => IsDark ? ErrorRamp.Dark(4) : Color.Parse("#83001D");
    public virtual Color AccentErrorDark5 => IsDark ? ErrorRamp.Dark(5) : Color.Parse("#660015");
    public virtual Color AccentErrorDark6 => IsDark ? ErrorRamp.Dark(6) : Color.Parse("#4B000C");
    public virtual Color AccentErrorDark7 => IsDark ? ErrorRamp.Dark(7) : Color.Parse("#310005");

    // Task 148: the old muddy-orange band (#7B4A00…) collided with Warning and Favourite. Replaced with an
    // indigo band that keeps the raised seed's hue, lands Dark3 at ~6.0:1 on the light canvas (well over the
    // 4.5:1 accent-text floor) and steps down monotonically. Indigo is a COOL hue, so Info now reads as a
    // distinct informational colour instead of yet another warm tone; it sits 105 RGB units from Primary's
    // ink at Dark3 and 77 from Primary's seed, furthest of any candidate from every other hue.
    public virtual Color AccentInfoDark3 => IsDark ? InfoRamp.Dark(3) : Color.Parse("#4655D1");
    public virtual Color AccentInfoDark4 => IsDark ? InfoRamp.Dark(4) : Color.Parse("#3844A7");
    public virtual Color AccentInfoDark5 => IsDark ? InfoRamp.Dark(5) : Color.Parse("#2A337D");
    public virtual Color AccentInfoDark6 => IsDark ? InfoRamp.Dark(6) : Color.Parse("#1C2254");
    public virtual Color AccentInfoDark7 => IsDark ? InfoRamp.Dark(7) : Color.Parse("#0E112A");

    // Task 148: this band was BYTE-IDENTICAL to Error's (#A40028…), which is exactly why Luke saw the two reds
    // as one. Replaced with a deeper raspberry band that keeps the strongly-magenta seed's hue, lands Dark3 at
    // ~8.4:1 on the light canvas and steps down monotonically. Error stays the brighter pure-red; Destructive
    // now reads as a darker, magenta-shifted red — 85 RGB units apart at Dark3 in light (was 0), and the
    // stronger rotation also separates the engine-computed dark stages past the 40-unit floor.
    public virtual Color AccentDestructiveDark3 => IsDark ? DestructiveRamp.Dark(3) : Color.Parse("#87236C");
    public virtual Color AccentDestructiveDark4 => IsDark ? DestructiveRamp.Dark(4) : Color.Parse("#6C1C56");
    public virtual Color AccentDestructiveDark5 => IsDark ? DestructiveRamp.Dark(5) : Color.Parse("#511541");
    public virtual Color AccentDestructiveDark6 => IsDark ? DestructiveRamp.Dark(6) : Color.Parse("#360E2B");
    public virtual Color AccentDestructiveDark7 => IsDark ? DestructiveRamp.Dark(7) : Color.Parse("#1B0716");

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

    #region Solid-Fill Policy (task 148 follow-up)

    // The four roles above name the FILLED COLUMN, which is the deep ink column: Dark3/Dark4/Dark5 with
    // Light10 on it. That column is right for every hue whose seed is deep, but a hue whose SEED is light
    // cannot wear it in the LIGHT palette: the ramps are hue-aware, so every stage sits at one OKLCH
    // lightness across all hues, and yellow at the Dark3 lightness is OLIVE. Measured: Favourite's light
    // Dark3 is #856600 — a muddy olive, not a yellow (Luke, 21 Oct 2026: the light Favourite must be a
    // clear yellow).
    //
    // So the solid-fill policy has TWO columns, and WHICH one a hue wears is decided in ONE place
    // (Theme/SolidFillBrush.cs), from a threshold declared HERE. A hue whose seed reaches
    // SolidFillBrightSeedLightness in OKLCH lightness wears the BRIGHT column in the light palette; every
    // other hue keeps the deep column. The DARK palette never uses the bright column: there the ink column
    // already reads bright (Favourite's dark Dark3 is the clear gold #CB9C00), so the deep stage is not
    // muddy and the dark picture is unchanged.
    //
    // The threshold is measured from the SEEDS, so it is a property of the palette rather than a per-control
    // value: Favourite #FFC400 sits at OKLCH L 0.8498 and crosses it; Warning #E89A3C at 0.7483, Success
    // 0.7183, Primary 0.6835, Error 0.6777, Subtle 0.6876, Info 0.5379, Destructive 0.4981 and Neutral
    // 0.3012 do not. Amber keeps the deep column because amber still reads as a COLOUR at a low lightness —
    // only yellow turns to mud — so Warning's face stays the deep #B05E1E it has always been.

    /// <summary>
    /// The seed lightness (OKLCH L) at or above which a hue wears the BRIGHT solid face in the light palette.
    /// </summary>
    /// <remarks>
    /// Public and virtual so a downstream theme can move the line: a palette whose hues are all pale would
    /// raise it, one whose hues are all deep would lower it. The decision itself lives in
    /// <see cref="SolidFillBrush"/>; this is the threshold that decision reads.
    /// </remarks>
    public virtual double SolidFillBrightSeedLightness => 0.80;

    /// <summary>
    /// The BRIGHT solid face: the stage a light-seeded hue wears as its checked/toggled face in the light
    /// palette — close to the hue's own seed, and clear of the canvas.
    /// </summary>
    /// <remarks>
    /// Light6 for Favourite is #F8BF00 (OKLCH L 0.8330, HSL hue 46.2°): a clear yellow one step off the
    /// #FFC400 seed, against a near-black ink at 12.36:1. Light4/Light5/Light6 are the hover/pressed steps
    /// below, and Light4 (#DAA700) is the same stage the header accent picker's swatch paints for Favourite,
    /// so the checked face and the swatch agree.
    /// </remarks>
    [AccentRole]
    public virtual RampStage SolidFillBrightFace => RampStage.Light6;

    /// <summary>The BRIGHT solid face under the pointer: one stage deeper than the bright rest face.</summary>
    [AccentRole]
    public virtual RampStage SolidFillBrightHover => RampStage.Light5;

    /// <summary>The BRIGHT solid face while pressed: two stages deeper than the bright rest face.</summary>
    [AccentRole]
    public virtual RampStage SolidFillBrightPressed => RampStage.Light4;

    #endregion Solid-Fill Policy (task 148 follow-up)

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
    /// <remarks>
    /// Task 149: this was Light6, which did FOUR jobs at once — the outlined control's pressed fill, a
    /// selected row's tint, a page/track surface and the hairline divider — so pressing an unselected
    /// list row painted exactly the colour a SELECTED row wears, and the two states were
    /// indistinguishable. It is now Light7, one step shallower than the selected row's Light6, so the
    /// fill ladder reads as one monotonic sequence: rest Light10, hover Light8, pressed Light7,
    /// selected Light6, selected+hover Light5, selected+pressed Light4. Pressing an unselected row now
    /// paints a PALE tint that is visibly weaker than selection, instead of the selection colour.
    /// </remarks>
    [AccentRole]
    internal virtual RampStage PressedBackgroundDefault => RampStage.Light7;

    /// <summary>Dimmed text: placeholders, captions, secondary labels, disabled text.</summary>
    /// <remarks>
    /// A role, so it follows the control's hue. That is safe only because the ramps are hue-aware:
    /// the stage reads 5.0-7.0:1 on the light canvas and 5.0-5.7:1 on the dark one in every hue,
    /// against 13.6-20.4:1 for TextDefault, so it is always readable and always visibly dimmer.
    /// <para>
    /// Task 149: it was Dark3, the SAME stage as the accent-text role, so an accent-coloured label and
    /// a grey caption were the same colour and the accent never read as the accent — one of Luke's
    /// three named symptoms. It is now <b>Dark2</b>, and the choice is measured, not assumed
    /// (Scratch/ThemeLabProof/task-149-roles/settle_dim_final.py scores every candidate pair).
    /// </para>
    /// <para>
    /// WHY Dark2 AND NOT Dark4 — the two palettes' ink ladders run in OPPOSITE directions, so "one
    /// stage deeper" does not mean "quieter" in both. In the LIGHT palette the hand-placed band
    /// (Dark3..Dark7) starts AT Dark3, so Dark3 is the LIGHTEST and most saturated ink there and
    /// Dark4 is DARKER and heavier: measured, a dim role at Dark4 is HEAVIER than the accent ink
    /// in 8 of 8 hues in the light palette and 8 of 8 in the dark one — it would make every caption
    /// louder than every accent label, the same bug in a new place. Dark2 is the stage that is
    /// quieter than the accent ink in the DARK palette in 8 of 8 hues (the order that holds for
    /// every hue, since the engine ramp there runs Dark1 darkest to Dark6 lightest) and in the
    /// neutral hue in both palettes — and neutral is where the overwhelming majority of the 21
    /// dim-text sites sit, since the role follows the library's default hue.
    /// </para>
    /// <para>
    /// The alternatives, measured: Dark1 misses the 4.5:1 text floor (Error dark 4.46:1,
    /// Destructive dark 4.29:1) and sits only 7.2 RGB units from the accent ink; Dark4 fails the
    /// quietness order in 16 of 16 hue/palette readings; and leaving it at Dark3 is the collision
    /// itself (0.0 RGB units). Dark2 clears the 4.5:1 floor everywhere (worst: Destructive dark
    /// 4.99:1) and sits 15.0-27.7 RGB units from the accent ink — a measurable difference, where
    /// there was none.
    /// </para>
    /// <para>
    /// Dark2 is a stage the engine already computes in every hue, so nothing is hand-placed and the
    /// dark palette is untouched.
    /// </para>
    /// </remarks>
    [AccentRole]
    internal virtual RampStage DimTextDefault => RampStage.Dark2;

    #region Default Accent Role Tokens (task 149)

    // Task 149: the theme had NO vocabulary for the accent. Every accent reference was a raw ramp
    // stage — {color:AccentBrush Dark1}, {color:AccentBrush Light6} — so the SAME accent appeared at
    // different shades across the library: the TextBox's warning focus border was very dark in light,
    // the CheckBox's warning fill read more saturated, and the Label's warning text was a third
    // colour again, all because each site spelled its own stage. Luke's ask: a small set of DEFAULT
    // ACCENT ROLE TOKENS, like TextDefault is for text — accent border, accent text, accent fill —
    // each used consistently everywhere for its purpose, with every stage taken from the engine's
    // ramps and roles and never a hand-placed hex.
    //
    // The roles below are the accent's vocabulary. Each is pinned to a real engine stage, the same
    // stage in both palettes, so the two palettes can never disagree about what a role means (the
    // pattern tasks 131/134 settled on). The stages are chosen from the measured ramp dump
    // (Scratch/ThemeLabProof/task-149-roles/ramp-dump.txt), and the reasoning for each is in the
    // role's own comment.
    //
    // The vocabulary deliberately separates the three jobs Luke named, so a border is never the
    // colour of a text run and never the colour of a solid block:
    //
    //   accent TEXT      AccentTextDefault      Dark3   — ink on the canvas
    //   accent BORDER    AccentBorderDefault    Dark1   — the accent's own hairline
    //   accent INDICATOR AccentIndicatorDefault Dark3   — the selection bar
    //   accent GLYPH     AccentGlyphDefault     Dark1   — a glyph/icon mark
    //   accent TINT      AccentTintDefault      Light2  — a soft coloured band
    //   accent FILL      SolidFillDefault       Dark3   — a solid checked/toggled face
    //   accent SOFT FILL SelectedBackground*   L6/L5/L4 — a selected row's tint
    //
    // WHY THE STAGES DIFFER. Light Dark2 is DARKER than light Dark3 (#865100 6.47:1 vs #B05E1E
    // 4.62:1) because the hand-placed light ink band starts at Dark3, so a site still on Dark2
    // renders visibly darker and muddier than one on Dark3. Light Dark1 (#9C5F00, 5.10:1) is
    // LIGHTER and more saturated than Dark3, and dark Dark1 (#C37800, 4.55:1) is DARKER and less
    // orange than dark Dark3 (#DD902F, 6.14:1) — so Dark1 is the hairline stage in both palettes
    // and Dark3 is the ink stage in both.

    /// <summary>Accent-coloured TEXT ink: a coloured label, a link, a coloured caption, an error message.</summary>
    /// <remarks>
    /// The stage 11 of the 15 accent-text sites already used, and the stage the theme's own
    /// documentation calls the accent-text stage. Measured, it holds the 4.5:1 text floor on the
    /// canvas in EVERY hue and BOTH palettes: light 4.62:1 (Warning, the worst) to 7.86:1 (Error),
    /// dark 5.83:1 (Destructive, the worst) to 6.60:1 (Success). It is also the only deep-ink stage
    /// that keeps the hues apart: the minimum pairwise RGB distance between hues at Dark3 is 53.0 in
    /// light and 51.7 in dark, and the Warning/Favourite gap Luke named is 53.0/51.7 — at Dark1 the
    /// same gap is only 20.2/24.7, so a Dark1 text run would be nearly indistinguishable from a
    /// Favourite one.
    /// <para>
    /// It replaces the Dark2 and Dark4 strays: in the light palette Dark2 (#865100) is measurably
    /// DARKER and muddier than Dark3, and Dark4 is a step further down, so those four sites were the
    /// "same accent at a different shade" symptom Luke saw.
    /// </para>
    /// <para>
    /// COLLISION NOTE: this role and <see cref="DimTextDefault"/> were BOTH on Dark3, so an
    /// accent-coloured label and a grey caption were the same colour and the accent never read as
    /// the accent. The collision is resolved by moving <see cref="DimTextDefault"/> UP to Dark2
    /// (see there), not by moving this role: the accent-text stage is the one the ramps were tuned
    /// for (the hand-placed light ink band starts at Dark3 precisely because Dark3 is the accent-text
    /// stage), and it is the stage that keeps the hues apart.
    /// </para>
    /// </remarks>
    [AccentRole]
    internal virtual RampStage AccentTextDefault => RampStage.Dark3;

    /// <summary>The accent's OWN BORDER: a control's frame line, a caret, a day-marker dot, a strong accent mark.</summary>
    /// <remarks>
    /// Dark1 — the stage 5 accented control frames already used. Measured in the light palette it is
    /// LIGHTER and more saturated than Dark3 (#9C5F00 5.10:1 vs #B05E1E 4.62:1), so the accent's
    /// hairline is neither "very dark" nor muddy — which was Luke's complaint about the TextBox's
    /// warning focus border. In the dark palette it is DARKER and less orange than Dark3 (#C37800
    /// 4.55:1 vs #DD902F 6.14:1), so it reads as a deep gold rather than "too close to yellow".
    /// <para>
    /// It clears the 3:1 non-text floor in every hue and both palettes: light 4.69:1 (Success, the
    /// worst) to 5.23:1 (Error), dark 4.29:1 (Destructive) to 4.95:1 (Success).
    /// </para>
    /// <para>
    /// Distinct from <see cref="AccentTextDefault"/> on purpose: the border is a hairline of the
    /// accent, the text is ink on the canvas, and they must not be the same colour. It is also
    /// distinct from <see cref="BorderDefault"/> (Light1), which is the outlined control's own
    /// hairline on a rest state.
    /// </para>
    /// </remarks>
    [AccentRole]
    internal virtual RampStage AccentBorderDefault => RampStage.Dark1;

    /// <summary>The accent's INDICATOR: the selection bar that marks the current tab or page.</summary>
    /// <remarks>
    /// Dark3 — the stage all three indicator sites (TabItem's selected pipe, TabStripItem's selected
    /// pipe, TabbedPage's page underline) already used, so this role needs a name, not a new stage.
    /// The bar is a solid mark on the canvas, so it takes the same ink stage as the accent's text:
    /// measured 4.62:1 (Warning, light, the worst) to 7.86:1 (Error) and 5.83:1 (Destructive, dark,
    /// the worst) to 6.60:1 (Success), every one of them over the 3:1 a mark needs.
    /// </remarks>
    [AccentRole]
    internal virtual RampStage AccentIndicatorDefault => RampStage.Dark3;

    /// <summary>The accent's GLYPH: an accent icon, a chevron, a pip, a spinner, a knob — a mark drawn in the accent, not text.</summary>
    /// <remarks>
    /// Dark1 — the stage 15 accent glyph sites already used (the menu chevron, the pager pips, the
    /// window caption glyphs, the switch's off knob). A glyph is a MARK, not a text run, so it takes
    /// the accent's own border stage rather than its text stage: a chevron drawn at Dark3 would sit
    /// visibly heavier than the label beside it, and the accent border's own stage keeps the mark and
    /// the frame it decorates in one colour.
    /// <para>
    /// Measured, Dark1 clears the 3:1 mark floor in every hue and both palettes: light 4.69:1
    /// (Success) to 5.23:1 (Error), dark 4.29:1 (Destructive) to 4.95:1 (Success).
    /// </para>
    /// </remarks>
    [AccentRole]
    internal virtual RampStage AccentGlyphDefault => RampStage.Dark1;

    /// <summary>The accent's TINT: a soft coloured band that marks a surface's kind (a notification's header strip).</summary>
    /// <remarks>
    /// Light2 — the stage the notification card's header band already used. It is a SURFACE, not a
    /// mark: a band is a large area of colour, so it must recede towards the canvas, and Light2 is
    /// the deepest tint that still reads as a tint (measured 2.73:1 to 3.01:1 against the light
    /// canvas, 2.76:1 to 3.20:1 against the dark one — a surface, not text, so the 4.5:1 text floor
    /// does not apply and nothing is drawn on it but the card's own ink).
    /// </remarks>
    [AccentRole]
    internal virtual RampStage AccentTintDefault => RampStage.Light2;

    /// <summary>A SELECTED row's tint: the outlined column's selection fill, so a chosen row reads as picked without going dark.</summary>
    /// <remarks>
    /// Light6 — the stage the 22 selected-row sites already used, and the stage ThemeRules.md line 129
    /// prescribes ("Selected {color:AccentBrush Light6}, selected and hovered Light5, selected and
    /// pressed Light4"). The rule was written out but no token stood behind it, so 22 sites hand-copied
    /// the stage; this is the token that makes the rule enforceable.
    /// <para>
    /// It is the OUTLINED column, deliberately distinct from <see cref="SolidFillDefault"/> (Dark3):
    /// a selected list row is a light fill of the row's own hue with default-weight text, never a
    /// dark block.
    /// </para>
    /// </remarks>
    [AccentRole]
    internal virtual RampStage SelectedBackgroundDefault => RampStage.Light6;

    /// <summary>A selected row's tint under the pointer: one step deeper than the selected rest tint.</summary>
    [AccentRole]
    internal virtual RampStage SelectedBackgroundHover => RampStage.Light5;

    /// <summary>A selected row's tint while pressed: two steps deeper than the selected rest tint.</summary>
    [AccentRole]
    internal virtual RampStage SelectedBackgroundPressed => RampStage.Light4;

    /// <summary>The accent's HAIRLINE: a thin divider or tick that separates one region of a surface from another.</summary>
    /// <remarks>
    /// Light7 — one step shallower than the selected row's tint (Light6) and one step deeper than the
    /// outlined control's pressed fill. It is the accent's own hairline role, and it exists so the
    /// 8 hairline/tick sites (the pickers' column dividers, the expander's chevron, the slider's tick
    /// marks) stop borrowing a stage that means something else.
    /// <para>
    /// The collision it resolves: Light6 was doing FOUR jobs at once — the selected row's rest tint,
    /// the outlined control's pressed fill, a page/track surface, and the hairline divider. A pressed
    /// unselected row therefore painted exactly the colour a SELECTED row wears. Splitting the ladder
    /// — pressed fill Light7, hairline Light7, selected Light6 — means pressing an unselected row now
    /// paints a PALE tint that is visibly weaker than selection.
    /// </para>
    /// <para>
    /// It is a LINE, not text: measured 1.39:1 to 1.48:1 against the light canvas and 1.42:1 to
    /// 1.56:1 against the dark one, which is what a hairline is — a separation, not a mark that has to
    /// be read.
    /// </para>
    /// </remarks>
    [AccentRole]
    internal virtual RampStage AccentDividerDefault => RampStage.Light7;

    #endregion Default Accent Role Tokens (task 149)

    /// <summary>The stage a focused ACCENTED control draws its border at while <see cref="FocusColorOnAccents"/> is off.</summary>
    /// <remarks>
    /// Dark1 — the stage the accent's own border takes (<see cref="AccentBorderDefault"/>). The focus
    /// ring of an accented control is that control's own accent hairline, made heavier: at Dark3 (the
    /// stage before task 149) the focus ring and the accent TEXT were the same colour, which is what
    /// made the TextBox's warning focus border and the Label's warning text look like one another —
    /// Luke's exact complaint. Dark1 also fixes the two ends of it: in the light palette the focus
    /// ring is now the saturated #9C5F00 rather than the muddy #B05E1E, so it is no longer "very
    /// dark", and in the dark palette it is #C37800 (deep amber) rather than #DD902F (bright orange),
    /// so it is no longer "too close to yellow".
    /// <para>
    /// The objection recorded against the old Dark2 — that the light palette's hand-lifted ink band
    /// starts at Dark3, so light Dark2 sat DARKER than Dark3 and focused accented controls looked
    /// muddy — does not apply to Dark1, which in the light palette is LIGHTER and more saturated than
    /// Dark3.
    /// </para>
    /// <para>
    /// Measured, it clears the 3:1 non-text floor in every hue and both palettes: light 4.69:1
    /// (Success) to 5.23:1 (Error), dark 4.29:1 (Destructive) to 4.95:1 (Success). It is read off the
    /// RESOLVED theme instance at render time by {default:FocusBrush} (Theme/FocusBrush.cs) as
    /// Accent{Hue}{this stage}Brush — see ThemeRules.md, Rule 21.
    /// </para>
    /// </remarks>
    [AccentRole]
    internal virtual RampStage AccentFocusBorder => RampStage.Dark1;

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
