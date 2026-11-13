using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
// Orientation lives in Avalonia.Layout, not Avalonia.Controls
using Avalonia.Layout;

namespace AvaloniaThemeLab.ThemePlayground.ValueConverters;

/// <summary>Looks up a control type's preview orientation.</summary>
public sealed class PreviewOrientationConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Type controlType && Application.Current is { } app &&
        app.TryFindResource($"{controlType.Name}PreviewOrientation", app.ActualThemeVariant, out var orientation) &&
        orientation is Orientation stacked
            ? stacked
            : Orientation.Horizontal;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
