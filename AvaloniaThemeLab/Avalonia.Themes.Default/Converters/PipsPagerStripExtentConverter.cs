using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Avalonia.Themes.Default;

/// <summary>
/// The extent the pip strip must have for its pips to fit: the number of pips that fit, times the
/// size of one pip cell.
/// </summary>
/// <remarks>
/// <para>
/// Avalonia's <see cref="Avalonia.Controls.PipsPager"/> sizes the pip strip from the pip cells it
/// can already see. On the very first layout there are none — the strip's items are realised during
/// that same layout pass — so it falls back to a hard-coded 12px cell and sets the strip to
/// <c>pages × 12</c>. A theme whose pip cells are wider (this one: 24px) is then arranged at half
/// the width its pips need, and the arrows at either end of the strip land on top of the first and
/// last pips. The correction arrives a layout pass later, which is why the overlap appears on the
/// first load into a page and disappears as soon as anything else triggers a layout.
/// </para>
/// <para>
/// This computes the same extent from the page count, so the strip is right from its first
/// measurement and the arrows never overlap the pips.
/// </para>
/// <para>
/// Theme Exception (Rule 10): a control-specific converter, because the value is a page count
/// multiplied by a themed size — no theme token and no engine markup extension can multiply a
/// bound count by a bound token.
/// </para>
/// </remarks>
public class PipsPagerStripExtentConverter : IMultiValueConverter
{
    /// <summary>A shared instance, so the theme does not construct one per use.</summary>
    public static PipsPagerStripExtentConverter Instance { get; } = new();

    /// <inheritdoc />
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // The page count, the visible-page cap and the size of one pip cell, in that order.
        if (values.Count < 3)
            return AvaloniaProperty.UnsetValue;
        if (values[0] is not int pages || values[1] is not int maxVisible)
            return AvaloniaProperty.UnsetValue;
        if (values[2] is not double cell && values[2] is not int)
            return AvaloniaProperty.UnsetValue;

        var pipSize = System.Convert.ToDouble(values[2], CultureInfo.InvariantCulture);
        if (pipSize <= 0)
            return AvaloniaProperty.UnsetValue;

        // The same cap Avalonia's PipsPager applies: only MaxVisiblePips pips are ever shown.
        var visible = Math.Min(pages, Math.Max(1, maxVisible));
        return visible * pipSize;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException("The pip strip's extent is derived, not written back.");
}
