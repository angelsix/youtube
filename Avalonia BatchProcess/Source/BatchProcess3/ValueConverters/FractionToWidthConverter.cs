using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BatchProcess3.ValueConverters;

/// <summary>
/// Converts a fraction (0.0–1.0) and a parent width into a pixel width.
/// Usage: MultiBinding with [0] = fraction, [1] = parent Bounds.Width
/// </summary>
public class FractionToWidthConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 ||
            values[0] is not double fraction ||
            values[1] is not double parentWidth)
            return 0.0;

        return Math.Max(0, fraction * parentWidth);
    }
}
