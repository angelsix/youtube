using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
// Orientation lives in Avalonia.Layout, not Avalonia.Controls
using Avalonia.Layout;

namespace AvaloniaThemeLab.ValueConverters;

/// <summary>
/// Looks up a control type's preview orientation: the <c>{TypeName}PreviewOrientation</c> resource
/// its theme file keys beside its <c>{TypeName}Preview</c> template (see ThemeRules.md Rule 20).
/// A type with no such key keeps the gallery default, <see cref="Orientation.Horizontal"/>.
/// </summary>
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
