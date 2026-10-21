using System.Globalization;
using System.Reflection;
using AngelSix.ThemeEngine;
using AngelSix.ThemeEngine.Generated;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace Avalonia.Themes.Default;

/// <summary>
/// The border brush a focused control takes, honouring the theme's <see cref="DefaultTheme.FocusColorOnAccents"/>
/// switch: <c>{default:FocusBrush}</c>.
/// </summary>
/// <remarks>
/// <para>
/// Resolved against the ELEMENT THAT RECEIVES THE SETTER (the focus style's target). The element's OWNERING
/// CONTROL decides the outcome: a template part answers with the hue of the outermost templated control it
/// belongs to (walking <c>TemplatedParent</c> up to the control the focus style attaches to); a standalone
/// control answers with its own. From that owner, the nearest hue is collected by climbing the visual
/// ancestry, stopping at the FIRST element whose effective <c>theme:Accent.Kind</c> is SET — <c>Neutral</c>
/// included; only an UNSET value continues upward. Caveat: the engine erases an explicit
/// <c>Accent.Kind="Neutral"</c> inside a tinted region (see <see cref="FocusDecision.TargetConverter.Collect"/>),
/// so such a control resolves to the region's hue until the engine preserves the assignment.
/// </para>
/// <list type="bullet">
/// <item><term>Neutral target:</term><description>takes <c>{color:AccentFocusBrush}</c>, the dedicated focus colour.</description></item>
/// <item><term>Non-neutral target, switch ON:</term><description>takes <c>{color:AccentFocusBrush}</c> too — today's behaviour.</description></item>
/// <item><term>Non-neutral target, switch OFF (the default):</term><description>takes the control's OWN hue at the
/// focus-border stage (<see cref="DefaultTheme.AccentFocusBorder"/>) — its own colour, darker than its resting
/// border, so focus is announced without borrowing the focus role's colour. The STAGE is read off the RESOLVED
/// THEME INSTANCE (reflection, virtual dispatch honours a downstream override), never hard-coded.</description></item>
/// </list>
/// </remarks>
public class FocusBrush : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new Binding
        {
            // Empty path + Self relative source: the bound VALUE is the target element itself, which the
            // converter climbs upward to find the effective hue. A plain Binding (not a MultiBinding /
            // ReflectionBinding-with-converter) because in this host those forms silently invalidate before the
            // converter runs — a struct such as AccentHue cannot coerce toward a paint slot, and a ReflectionBinding
            // bypasses its own Converter.
            RelativeSource = new(RelativeSourceMode.Self),
            Converter = FocusDecision.BrushFromTarget,
        };
}

/// <summary>
/// The border thickness a focused control takes, honouring the same decision as <see cref="FocusBrush"/>:
/// <c>{default:FocusThickness}</c>. Always <c>{size:ThicknessMd}</c>: the focus WEIGHT is constant whatever the
/// switch says, so a focused accented control reads heavier than its resting border.
/// </summary>
public class FocusThickness : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) =>
        new Binding
        {
            RelativeSource = new(RelativeSourceMode.Self),
            Converter = FocusDecision.ThicknessFromTarget,
        };
}

internal static class FocusDecision
{
    private static readonly IValueConverter BrushFromTargetImpl = new TargetConverter(wantsBrush: true);
    private static readonly IValueConverter ThicknessFromTargetImpl = new TargetConverter(wantsBrush: false);

    internal static IValueConverter BrushFromTarget => BrushFromTargetImpl;
    internal static IValueConverter ThicknessFromTarget => ThicknessFromTargetImpl;

    /// <summary>
    /// Converts the TARGET ELEMENT into the focus border value. Reads the token off the RESOLVED THEME INSTANCE
    /// (honouring a scoped region and its dark twin, exactly like the engine's own converters), so a downstream
    /// theme flipping <c>FocusColorOnAccents</c> flips every focused accented control at once.
    /// </summary>
    private sealed class TargetConverter(bool wantsBrush) : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var element = value as StyledElement;
            if (element is null)
                return AvaloniaProperty.UnsetValue;

            var (owner, hue, forceDark) = Collect(element);

            // Honour a scoped region and its dark twin exactly like the engine's own converters do.
            var theme = ThemeResolution.Resolve(null, forceDark, ThemeContext.Resolver?.Invoke()?.Active);
            if (theme is null)
                return AvaloniaProperty.UnsetValue;

            var prop = theme.GetType().GetProperty(nameof(DefaultTheme.FocusColorOnAccents),
                BindingFlags.Instance | BindingFlags.Public);
            var onFocus = prop is { CanRead: true } && prop.GetValue(theme) is true;

            if (!wantsBrush)
            {
                // The focus ring must NOT change a control's size (task 67). Thickening the border to
                // ThicknessMd grew an auto-sized control by 2px on focus, shifting layout and breaking the
                // state-pass restore (AVLG030). Instead the focus WEIGHT equals the OWNING CONTROL'S RESTING
                // BORDER THICKNESS: the box keeps exactly its rest outer size, and the visible 2px ring is
                // carried by the default focus ADORNER (AdornerLayer.axaml), which draws concentrically OUTSIDE
                // the control's bounds and therefore consumes no layout (Rule 19). A control that nulls its
                // FocusAdorner and leans on the border bump for its ring keeps its own resting weight here.
                // Hand the OWNING CONTROL'S RESTING BORDER THICKNESS straight through (usually {size:ThicknessSm}):
                // the focus weight then EQUALS the rest weight, so the border box never grows. Read off the OWNER,
                // not the part the setter lands on — the part is what the focus style inflates, whereas the owning
                // control's own BorderThickness stays at its rest value throughout. Wrapped in try/catch because the
                // owner is a general StyledElement whose concrete type may not register the property; that must
                // degrade to the fallback below, never throw into the shared build.
                try
                {
                    // GetValue on a Thickness-typed property yields the owning control's resting thickness
                    // (usually {size:ThicknessSm}); returning it makes the focus weight equal the rest weight.
                    return owner.GetValue(Border.BorderThicknessProperty);
                }
                catch
                {
                    // Property unregistered on this owner's type: fall through to the theme's focus weight.
                }
                // Fall back to the theme's focus weight if the owner carries no usable thickness.
                var mdMember = theme.GetType().GetProperty("ThicknessMd", BindingFlags.Instance | BindingFlags.Public);
                var mdResult = mdMember is { CanRead: true } ? mdMember.GetValue(theme) : null;
                return mdResult is null || Equals(mdResult, AvaloniaProperty.UnsetValue) ? AvaloniaProperty.UnsetValue : mdResult;
            }

            string propertyName;
            if (hue.IsUnset || hue.Name == "Neutral" || onFocus)
            {
                // Neutral target, or the switch on: the dedicated focus colour.
                propertyName = "AccentFocusBrush";
            }
            else
            {
                // Accented target with the switch off: the control's own hue at the focus-border STAGE.
                // The stage is the [AccentRole] AccentFocusBorder, read off the RESOLVED THEME INSTANCE so a
                // downstream override of the role moves every focused accented control at once (virtual
                // dispatch honours the override). Built as Accent{Hue}{Stage}Brush — the same expansion the
                // source generator bakes into the engine's native {color:AccentBrush <Stage>} tokens.
                var stageProp = theme.GetType().GetProperty(nameof(DefaultTheme.AccentFocusBorder),
                    BindingFlags.Instance | BindingFlags.NonPublic);
                var stage = stageProp is { CanRead: true } ? stageProp.GetValue(theme) : null;
                if (stage is null)
                    return AvaloniaProperty.UnsetValue;
                propertyName = $"Accent{hue.Name}{stage}Brush";
            }

            var member = theme.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            var result = member is { CanRead: true } ? member.GetValue(theme) : null;
            return result is null || Equals(result, AvaloniaProperty.UnsetValue) ? AvaloniaProperty.UnsetValue : result;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();

        /// <summary>
        /// Resolves the OWNERING CONTROL of the target — the outermost templated control it belongs to, reached by
        /// walking <see cref="StyledElement.TemplatedParent"/> up until a control that is itself not a template
        /// part — and collects the nearest hue FROM THAT OWNER up the visual tree, together with whether any
        /// ancestor forces the dark palette (<c>ThemeRegion.IsDark</c>).
        /// </summary>
        /// <remarks>
        /// Stopping at the owner matters because a template part's own <c>theme:Accent.Kind</c> is usually UNSET
        /// (the part inherits nothing): answering from the part would miss the accent the OWNING control carries
        /// one level up. Walking <c>TemplatedParent</c> lands on the control the focus style attaches to, whose
        /// hue is the decision-maker. A standalone control (no <c>TemplatedParent</c>) owns itself.
        /// </remarks>
        /// <remarks>
        /// From the owner the walk STOPS at the FIRST element whose effective <c>theme:Accent.Kind</c> is SET —
        /// <c>Neutral</c> included; only an UNSET value continues upward. A top-level control pinned
        /// <c>Accent.Kind="Neutral"</c> therefore resolves to <c>Neutral</c> here and takes the dedicated focus
        /// colour.
        /// </remarks>
        /// <remarks>
        /// Known engine limit (documented in ThemeRules.md, Rule 21): the engine ERASES an explicit
        /// <c>Accent.Kind="Neutral"</c> assigned on an element inside a tinted region — assigning the default
        /// value stores no local value, so at runtime <c>GetHue</c> reports the REGION'S hue and
        /// <c>IsSet(HueProperty)</c> is false (measured: a focused box pinned <c>Neutral</c> inside a
        /// <c>Warning</c> StackPanel reads <c>Name=="Warning"</c>, <c>IsSet==false</c>; the identical box at
        /// top level reads <c>Name=="Neutral"</c>). The author's intent is therefore unrecoverable at render
        /// time, and a deliberately-neutral control inside a tinted region takes the region's hue for its focus
        /// border. Until the engine preserves such assignments, the documented escape hatch is to wrap the
        /// control in its own untinted container.
        /// </remarks>
        private static (StyledElement Owner, AccentHue Hue, bool ForceDark) Collect(StyledElement start)
        {
            // Resolve the owning control: climb TemplatedParent to the outermost templated control.
            var owner = start;
            for (var tp = start.TemplatedParent; tp is StyledElement tpe; tp = tpe.TemplatedParent)
                owner = tpe;

            var forceDark = false;
            for (var v = (Visual?)owner; v is not null; v = v.GetVisualParent())
            {
                if (v is StyledElement se)
                {
                    if (!forceDark && ThemeRegion.GetIsDark(se))
                        forceDark = true;
                    var h = ThemeAccent.GetHue(se);
                    // Stop at the first SET hue — Neutral included. Only UNSET continues upward.
                    if (!h.IsUnset)
                        return (owner, h, forceDark);
                }
            }
            return (owner, default, forceDark);
        }
    }
}
