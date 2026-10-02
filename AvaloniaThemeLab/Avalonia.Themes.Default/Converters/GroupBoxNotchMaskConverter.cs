using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls.Converters;
using Avalonia.Data.Converters;

namespace Avalonia.Themes.Default;

/// <summary>
/// The opacity mask that cuts a GroupBox's header notch out of its frame, starting wherever the header actually sits.
/// </summary>
/// <remarks>
/// Avalonia's <see cref="BorderGapMaskConverter"/> takes the notch's left edge as a fixed ConverterParameter, which
/// cannot be bound, so the gap was a literal. A literal gap does not move with the corner radius: once the radius
/// reached it, the notch started inside the frame's curve and cut the corner. This reads the gap from the header's
/// laid-out X instead, so the theme places the header (from the radius token) and the notch follows.
/// Values: header X, header width, frame width, frame height.
/// </remarks>
public class GroupBoxNotchMaskConverter : IMultiValueConverter
{
    private static readonly BorderGapMaskConverter mask = new();

    /// <summary>A shared instance, so the theme does not construct one per use.</summary>
    public static GroupBoxNotchMaskConverter Instance { get; } = new();

    /// <inheritdoc />
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) =>
        values.Count == 4 && values[0] is double gap
            ? mask.Convert([values[1], values[2], values[3]], targetType, gap, culture)
            : AvaloniaProperty.UnsetValue;
}
