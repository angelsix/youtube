using AngelSix.ThemeEngine;
using AngelSix.ThemeEngine.Generated;

namespace Avalonia.Themes.Default;

/// <summary>
/// The <see cref="Media.Color"/> twin of <c>{color:AccentBrush}</c>: <c>{default:AccentColor}</c> for the hue of
/// the control it is set on, <c>{default:AccentColor Dark3}</c> for a stage. For a <c>Color</c> slot, such as a
/// colour control's <c>Color</c>, which a brush cannot fill.
/// </summary>
/// <remarks>
/// It reads <c>Accent{Hue}{Stage}</c> where the brush reads <c>Accent{Hue}{Stage}Brush</c>, through the engine's
/// own accent binding, so the hue follows <c>Accent.Kind</c> and a dark region gets its mirrored ramp. Theme-local
/// for now, like <see cref="ConcentricCorners"/>; it is a candidate for the engine's generator.
/// </remarks>
public sealed class AccentColor : AccentValueExtension
{
    public AccentColor()
    {
    }

    /// <summary>Supports the positional form <c>{default:AccentColor Dark1}</c>.</summary>
    public AccentColor(AccentVariant variant) => Variant = variant;

    /// <summary>Which stage of the control's hue to resolve.</summary>
    public AccentVariant Variant { get; set; } = AccentVariant.Base;

    protected override string VariantSuffix => Variant == AccentVariant.Base ? string.Empty : Variant.ToString();

    protected override string FallbackHue => "Neutral";

    protected override string PropertySuffix => string.Empty;
}
