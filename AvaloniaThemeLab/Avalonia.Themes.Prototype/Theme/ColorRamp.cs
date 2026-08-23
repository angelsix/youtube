using Avalonia.Media;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// One colour and the evenly-spaced shades either side of it — ten lighter, ten darker.
/// </summary>
/// <remarks>
/// <para>
/// A ramp exists so a theme can spend one colour and get a coherent set out of it, rather than
/// hand-picking a dozen near-identical greys and hoping they stay in step. Only lightness moves;
/// hue and colourfulness are held, which is what makes the whole family read as shades of one
/// colour rather than drifting towards a different hue at the dark end.
/// </para>
/// <para>
/// The outermost stage stops at <see cref="ExtremeAmount"/> of the way to white or black rather
/// than reaching it. Going the whole way would collapse every colour's extremes onto the same two
/// values, so a ramp's ends would say nothing about which colour it came from.
/// </para>
/// <para>
/// It is deliberately <i>not</i> the theme's public API — <see cref="DefaultTheme"/> exposes each
/// stage as its own flat <c>virtual</c> property fed from a ramp, so overriding a single one stays
/// a one-line change and the source generator still sees a real property to build a markup
/// extension from.
/// </para>
/// </remarks>
public class ColorRamp
{
    #region Constructor

    /// <param name="seed">The centre point. Every stage is measured from it.</param>
    public ColorRamp(Color seed) => Base = seed;

    #endregion Constructor

    #region Properties

    /// <summary>The centre point.</summary>
    public Color Base { get; }

    #endregion Properties

    #region Derivation Amounts

    /// <summary>How many stages sit between the centre and each extreme.</summary>
    protected virtual int Stages => 10;

    /// <summary>
    /// How far the outermost stage travels towards white or black, as a fraction of the distance
    /// available. Short of 1 so no colour's ramp ends on the same white or black as every other's.
    /// </summary>
    protected virtual double ExtremeAmount => 0.97;

    #endregion Derivation Amounts

    #region Stages

    public virtual Color Light1 => Lighter(1);
    public virtual Color Light2 => Lighter(2);
    public virtual Color Light3 => Lighter(3);
    public virtual Color Light4 => Lighter(4);
    public virtual Color Light5 => Lighter(5);
    public virtual Color Light6 => Lighter(6);
    public virtual Color Light7 => Lighter(7);
    public virtual Color Light8 => Lighter(8);
    public virtual Color Light9 => Lighter(9);
    public virtual Color Light10 => Lighter(10);

    public virtual Color Dark1 => Darker(1);
    public virtual Color Dark2 => Darker(2);
    public virtual Color Dark3 => Darker(3);
    public virtual Color Dark4 => Darker(4);
    public virtual Color Dark5 => Darker(5);
    public virtual Color Dark6 => Darker(6);
    public virtual Color Dark7 => Darker(7);
    public virtual Color Dark8 => Darker(8);
    public virtual Color Dark9 => Darker(9);
    public virtual Color Dark10 => Darker(10);

    #endregion Stages

    #region Methods

    /// <summary>The stage <paramref name="step"/> steps towards white.</summary>
    protected virtual Color Lighter(int step)
    {
        var l = BaseLightness;
        return AtLightness(l + ((1 - l) * Fraction(step)));
    }

    /// <summary>The stage <paramref name="step"/> steps towards black.</summary>
    protected virtual Color Darker(int step)
    {
        var l = BaseLightness;
        return AtLightness(l - (l * Fraction(step)));
    }

    /// <summary>
    /// How far a given step travels, as an even split of <see cref="ExtremeAmount"/>. Ten stages at 97% gives 9.7 / 19.4 / 29.1 … / 97.
    /// </summary>
    /// <remarks>
    /// A fraction of the <i>remaining</i> distance rather than a fixed lightness step, so the ramp
    /// can never clamp and the same step means the same proportional move whatever the seed's
    /// lightness.
    /// </remarks>
    protected double Fraction(int step) => ExtremeAmount * step / Stages;

    /// <summary>The lightness of <see cref="Base"/> — where every stage is measured from.</summary>
    protected double BaseLightness => Base.ToHsl().L;

    /// <summary>
    /// Builds the ramp's colour at a given lightness, wearing the seed's hue and colourfulness.
    /// </summary>
    protected Color AtLightness(double lightness)
    {
        var seed = Base.ToHsl();
        lightness = Clamp(lightness);

        // Carrying S across unchanged would make the result dramatically more colourful, because
        // the same S means a far wider RGB spread at mid lightness than at the extremes. A near
        // white #EBE6EF derived that way gives a violet #7F6198 where the palette wants a muted
        // #6D6275. Converting to chroma, moving, then converting back holds the colourfulness the
        // seed actually had.
        var chroma = seed.S * LightnessSpan(seed.L);
        var span = LightnessSpan(lightness);
        var saturation = span == 0 ? 0 : Clamp(chroma / span);

        return new HslColor(seed.A, seed.H, saturation, lightness).ToRgb();
    }

    /// <summary>
    /// How much room a given lightness leaves for colour — full at mid grey, none at black or
    /// white. This is the factor between HSL's saturation and actual chroma.
    /// </summary>
    private static double LightnessSpan(double lightness) => 1 - Math.Abs((2 * lightness) - 1);

    private static double Clamp(double value) => Math.Clamp(value, 0, 1);

    #endregion Methods
}

/// <summary>
/// A ramp with its stages mirrored: <c>Light</c> travels towards black and <c>Dark</c> towards
/// white.
/// </summary>
/// <remarks>
/// This is what lets one set of control themes serve both palettes. The stage names are absolute —
/// <c>Dark3</c> means darker — so a control theme reaching for <c>Dark3</c> as its text colour
/// would produce dark-on-dark in a dark palette. Mirroring the ramp once here keeps that inversion
/// in a single place rather than putting a light/dark branch in every control.
/// </remarks>
public class MirroredColorRamp : ColorRamp
{
    public MirroredColorRamp(Color seed)
        : base(seed)
    {
    }

    protected override Color Lighter(int step) => base.Darker(step);

    protected override Color Darker(int step) => base.Lighter(step);
}
