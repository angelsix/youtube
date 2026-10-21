using System.Globalization;
using System.Reflection;
using AngelSix.ThemeEngine;
using AngelSix.ThemeEngine.Generated;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Avalonia.Themes.Default;

/// <summary>
/// The brush a SOLID-FILLED face takes in its rest state — the checked, toggled or starred face of a
/// control: <c>{default:SolidFill}</c>.
/// </summary>
/// <remarks>
/// <para>
/// This is the CENTRAL solid-fill policy, not a per-control colour and not a per-control special case.
/// The decision lives in one class (<see cref="SolidFillDecision"/>) and the per-hue choice is declared in
/// <see cref="DefaultTheme"/>: a hue whose SEED is light — its OKLCH lightness reaches
/// <see cref="DefaultTheme.SolidFillBrightSeedLightness"/> — cannot wear the deep ink column in the LIGHT
/// palette without turning muddy (yellow at the hue-aware <c>Dark3</c> lightness is olive), so it takes a
/// BRIGHT face near its own seed instead (<see cref="DefaultTheme.SolidFillBrightFace"/>), and the ink on
/// that face flips to the near-black text role, chosen by contrast.
/// </para>
/// <para>
/// Every other hue, and every hue in the DARK palette, keeps the standard column: the filled-column stage
/// <see cref="DefaultTheme.SolidFillDefault"/> (Dark3) with <see cref="DefaultTheme.OnSolidFillInkDefault"/>
/// (Light10) on it. The dark palette is left alone because its ink column already reads bright there
/// (Favourite's dark <c>Dark3</c> is a clear gold), so the deep stage is never muddy in that palette.
/// </para>
/// <para>
/// The hue and the palette are resolved at RENDER time from the element that receives the setter, exactly as
/// <see cref="FocusBrush"/> resolves its focus colour: the target's effective <c>theme:Accent.Kind</c>
/// decides which hue answers, and the STAGES are read off the RESOLVED THEME INSTANCE, so a downstream theme
/// overriding a token moves every solid face in the library at once.
/// </para>
/// </remarks>
public class SolidFill : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new Binding
        {
            RelativeSource = new(RelativeSourceMode.Self),
            Converter = SolidFillDecision.FaceFromTarget(SolidFillDecision.State.Rest),
        };
}

/// <summary>
/// The brush a solid-filled face takes under the pointer: <c>{default:SolidFillHover}</c>. One step deeper
/// than the rest face, as the filled column's hover stage always is.
/// </summary>
public class SolidFillHover : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new Binding
        {
            RelativeSource = new(RelativeSourceMode.Self),
            Converter = SolidFillDecision.FaceFromTarget(SolidFillDecision.State.Hover),
        };
}

/// <summary>
/// The brush a solid-filled face takes while pressed: <c>{default:SolidFillPressed}</c>. Two steps deeper
/// than the rest face, as the filled column's pressed stage always is.
/// </summary>
public class SolidFillPressed : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new Binding
        {
            RelativeSource = new(RelativeSourceMode.Self),
            Converter = SolidFillDecision.FaceFromTarget(SolidFillDecision.State.Pressed),
        };
}

/// <summary>
/// The ink drawn ON a solid face — a tick, a dot, an icon, on-fill text: <c>{default:OnSolidInk}</c>.
/// </summary>
/// <remarks>
/// Chosen BY CONTRAST against the face the ink sits on: the white role
/// (<see cref="DefaultTheme.OnSolidFillInkDefault"/>, Light10) and the near-black text role
/// (<see cref="DefaultTheme.TextDefault"/>, Dark9) are measured against the face, and the one that reaches
/// the higher contrast wins. On a bright face that is the near-black ink (12.36:1 on Favourite's light
/// bright face); on the deep ink column it is the white ink (5.21:1 on Favourite's light <c>Dark3</c>),
/// which is what the theme has always used. Whichever is chosen clears the 4.5:1 text floor on every face
/// it lands on.
/// </remarks>
public class OnSolidInk : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new Binding
        {
            RelativeSource = new(RelativeSourceMode.Self),
            Converter = SolidFillDecision.InkFromTarget,
        };
}

/// <summary>
/// The one place the solid-fill policy is decided. Every solid face in the theme resolves through here, so
/// the muddy-light problem cannot come back one control at a time.
/// </summary>
internal static class SolidFillDecision
{
    /// <summary>Which face of the solid-fill column is being asked for.</summary>
    internal enum State
    {
        Rest,
        Hover,
        Pressed,
    }

    private static readonly IValueConverter RestFace = new FaceConverter(SolidFillDecision.State.Rest);
    private static readonly IValueConverter HoverFace = new FaceConverter(SolidFillDecision.State.Hover);
    private static readonly IValueConverter PressedFace = new FaceConverter(SolidFillDecision.State.Pressed);
    private static readonly IValueConverter Ink = new InkConverter();

    internal static IValueConverter FaceFromTarget(SolidFillDecision.State state) => state switch
    {
        SolidFillDecision.State.Hover => HoverFace,
        SolidFillDecision.State.Pressed => PressedFace,
        _ => RestFace,
    };

    internal static IValueConverter InkFromTarget => Ink;

    /// <summary>Resolves the face brush for one target element.</summary>
    private sealed class FaceConverter(SolidFillDecision.State state) : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not StyledElement element)
                return AvaloniaProperty.UnsetValue;

            var resolved = SolidFillContext.Resolve(element);
            return resolved?.FaceBrush(state) ?? AvaloniaProperty.UnsetValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// Resolves the ink brush for one target element: whichever of the white role and the near-black text
    /// role reaches the higher contrast against the face the ink sits on.
    /// </summary>
    private sealed class InkConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not StyledElement element)
                return AvaloniaProperty.UnsetValue;

            var resolved = SolidFillContext.Resolve(element);
            return resolved?.InkBrush() ?? AvaloniaProperty.UnsetValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}

/// <summary>
/// Everything one target element needs to answer a solid-fill question: the resolved theme instance, the
/// hue that answers for it, and whether the dark palette is in force.
/// </summary>
internal sealed class SolidFillContext
{
    /// <summary>The OKLCH lightness a bright face must reach to count as bright in the resolved palette.</summary>
    private const double BrightFaceLightnessFloor = 0.75;
    private readonly object _theme;
    private readonly string _hue;

    private SolidFillContext(object theme, string hue)
    {
        _theme = theme;
        _hue = hue;
    }

    /// <summary>
    /// Resolves the theme, the hue and the palette for a target element, or null when they cannot be read.
    /// </summary>
    internal static SolidFillContext? Resolve(StyledElement element)
    {
        var hue = CollectHue(element);
        var forceDark = CollectIsDark(element);

        // Honour a scoped region and its dark twin exactly like the engine's own converters do.
        var theme = ThemeResolution.Resolve(null, forceDark, ThemeContext.Resolver?.Invoke()?.Active);
        if (theme is null)
            return null;

        // The hue that answers: the target's effective Accent.Kind, and the theme's fallback hue (Neutral)
        // when no hue is set anywhere above it — the same fallback the engine's own accent machinery uses.
        var hueName = hue.IsUnset || string.IsNullOrEmpty(hue.Name) ? "Neutral" : hue.Name;

        return new SolidFillContext(theme, hueName);
    }

    /// <summary>Whether this hue wears the BRIGHT face in the LIGHT palette.</summary>
    internal bool IsBright
    {
        get
        {
            // The dark palette never wears a bright face: its ink column already reads bright there
            // (Favourite's dark Dark3 is a clear gold), so the deep stage is not muddy in that palette and
            // today's colours are right there.
            if (IsDark)
                return false;

            // The PER-HUE choice, declared in the theme: a seed light enough to go olive at the deep ink
            // stage takes the bright column instead.
            var seed = ReadColor($"Accent{_hue}");
            if (seed is null)
                return false;

            var threshold = ReadDouble("SolidFillBrightSeedLightness") ?? 0.80;
            if (OklchLightness(seed.Value) < threshold)
                return false;

            // The palette check: the bright stage must actually BE bright in the resolved palette. This
            // keeps the policy honest even where the palette flag is not readable — a downstream theme whose
            // Light stages are dark simply keeps the standard column.
            var bright = ReadColor($"Accent{_hue}{BrightFaceStage()}");
            return bright is not null && OklchLightness(bright.Value) >= BrightFaceLightnessFloor;
        }
    }

    /// <summary>Whether the resolved theme instance is the dark palette.</summary>
    internal bool IsDark => ReadBool("IsDark") ?? false;

    /// <summary>The face brush for the given state.</summary>
    internal object? FaceBrush(SolidFillDecision.State state)
    {
        var stage = FaceStage(state);
        if (stage is null)
            return AvaloniaProperty.UnsetValue;

        var brush = ReadBrush($"Accent{_hue}{stage.Value}Brush");
        return brush ?? AvaloniaProperty.UnsetValue;
    }

    /// <summary>
    /// The ink brush: the white role and the near-black text role measured against the REST face, and the
    /// one that reaches the higher contrast wins.
    /// </summary>
    internal object? InkBrush()
    {
        var face = FaceColor(SolidFillDecision.State.Rest);
        if (face is null)
            return AvaloniaProperty.UnsetValue;

        object? bestBrush = null;
        var bestContrast = double.MinValue;

        foreach (var role in new[] { "OnSolidFillInkDefault", "TextDefault" })
        {
            var candidate = InkCandidate(role);
            if (candidate is null)
                continue;

            var contrast = ContrastRatio(candidate.Value.Color, face.Value);
            if (contrast > bestContrast)
            {
                bestContrast = contrast;
                bestBrush = candidate.Value.Brush;
            }
        }

        return bestBrush ?? AvaloniaProperty.UnsetValue;
    }

    private Color? FaceColor(SolidFillDecision.State state)
    {
        var stage = FaceStage(state);
        return stage is null ? null : ReadColor($"Accent{_hue}{stage.Value}");
    }

    private (Color Color, object? Brush)? InkCandidate(string roleProperty)
    {
        var stage = ReadStage(roleProperty);
        if (stage is null)
            return null;

        var color = ReadColor($"Accent{_hue}{stage.Value}");
        if (color is null)
            return null;

        return (color.Value, ReadBrush($"Accent{_hue}{stage.Value}Brush"));
    }

    /// <summary>
    /// The stage the face takes: the bright column for a bright hue in the light palette, the standard
    /// filled column otherwise.
    /// </summary>
    private RampStage? FaceStage(SolidFillDecision.State state)
    {
        if (IsBright)
        {
            var bright = state switch
            {
                SolidFillDecision.State.Hover => "SolidFillBrightHover",
                SolidFillDecision.State.Pressed => "SolidFillBrightPressed",
                _ => "SolidFillBrightFace",
            };
            var brightStage = ReadStage(bright);
            if (brightStage is not null)
                return brightStage;
        }

        var standard = state switch
        {
            SolidFillDecision.State.Hover => "SolidFillHover",
            SolidFillDecision.State.Pressed => "SolidFillPressed",
            _ => "SolidFillDefault",
        };
        return ReadStage(standard);
    }

    /// <summary>The bright column's rest stage, as a name, for the palette check above.</summary>
    private string BrightFaceStage()
    {
        var stage = ReadStage("SolidFillBrightFace");
        return stage is null ? "Light6" : stage.ToString();
    }

    // ---- reflection readers over the RESOLVED theme instance ----

    private RampStage? ReadStage(string propertyName)
    {
        var value = Read(propertyName);
        return value is RampStage stage ? stage : null;
    }

    private double? ReadDouble(string propertyName)
    {
        var value = Read(propertyName);
        return value is double d ? d : null;
    }

    private bool? ReadBool(string propertyName)
    {
        var value = Read(propertyName);
        return value is bool b ? b : null;
    }

    private Color? ReadColor(string propertyName)
    {
        var value = Read(propertyName);
        return value is Color c ? c : null;
    }

    private object? ReadBrush(string propertyName)
    {
        var value = Read(propertyName);
        return value is IBrush brush ? brush : null;
    }

    /// <summary>
    /// Reads a property off the RESOLVED theme instance.
    /// </summary>
    /// <remarks>
    /// The PUBLIC member is looked for first and the non-public one only if there is no public one:
    /// a name can exist twice on a type (the hand-written public member and the source generator's own
    /// non-public one), and asking for both visibility flags at once throws
    /// <see cref="AmbiguousMatchException"/> — which would fail the whole binding. A read that throws is
    /// reported as "no value", never propagated into a render.
    /// </remarks>
    private object? Read(string propertyName)
    {
        var type = _theme.GetType();

        try
        {
            var member = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (member is { CanRead: true })
                return member.GetValue(_theme);
        }
        catch
        {
            // Fall through to the non-public lookup.
        }

        try
        {
            var member = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (member is { CanRead: true })
                return member.GetValue(_theme);
        }
        catch
        {
            // A property that cannot be read is a property that is not there, as far as the policy goes.
        }

        return null;
    }

    /// <summary>
    /// Collects the effective hue of the target: the nearest SET <c>theme:Accent.Kind</c> at or above it.
    /// </summary>
    /// <remarks>
    /// The target itself is asked FIRST, because a control's own hue is what decides its face (a checked
    /// Favourite button inside a Neutral bar is still Favourite). A template part carries no hue of its own,
    /// so the walk continues up the visual ancestry until it finds one — which is the control that owns the
    /// part.
    /// </remarks>
    private static AccentHue CollectHue(StyledElement start)
    {
        for (var v = (Visual?)start; v is not null; v = v.GetVisualParent())
        {
            if (v is StyledElement se)
            {
                var h = ThemeAccent.GetHue(se);
                if (!h.IsUnset)
                    return h;
            }
        }
        return default;
    }

    /// <summary>Whether any ancestor forces the dark palette.</summary>
    private static bool CollectIsDark(StyledElement start)
    {
        for (var v = (Visual?)start; v is not null; v = v.GetVisualParent())
        {
            if (v is StyledElement se && ThemeRegion.GetIsDark(se))
                return true;
        }
        return false;
    }

    // ---- colour maths ----

    /// <summary>The OKLCH lightness of a colour — the CIE L* the engine's hue-aware ramps park stages at.</summary>
    internal static double OklchLightness(Color color)
    {
        var r = Linearize(color.R / 255.0);
        var g = Linearize(color.G / 255.0);
        var b = Linearize(color.B / 255.0);

        var l = Math.Cbrt(0.4122214708 * r + 0.5363325363 * g + 0.0514459929 * b);
        var m = Math.Cbrt(0.2119034982 * r + 0.6806995451 * g + 0.1073969566 * b);
        var s = Math.Cbrt(0.0883024619 * r + 0.2817188376 * g + 0.6299787005 * b);

        return 0.2104542553 * l + 0.7936177850 * m - 0.0040720468 * s;
    }

    /// <summary>The WCAG relative luminance of a colour.</summary>
    internal static double RelativeLuminance(Color color) =>
        0.2126 * Linearize(color.R / 255.0) +
        0.7152 * Linearize(color.G / 255.0) +
        0.0722 * Linearize(color.B / 255.0);

    /// <summary>The WCAG contrast ratio between two colours.</summary>
    internal static double ContrastRatio(Color a, Color b)
    {
        var la = RelativeLuminance(a);
        var lb = RelativeLuminance(b);
        var hi = Math.Max(la, lb);
        var lo = Math.Min(la, lb);
        return (hi + 0.05) / (lo + 0.05);
    }

    private static double Linearize(double c) =>
        c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
}
