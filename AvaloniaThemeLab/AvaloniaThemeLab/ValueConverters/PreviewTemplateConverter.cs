using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace AvaloniaThemeLab.ValueConverters;

/// <summary>
/// Finds a control type's design preview: the <c>{TypeName}Preview</c> template its theme file keys
/// beside its ControlTheme. A type with no preview template yet converts to null.
/// </summary>
public sealed class PreviewTemplateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Type controlType && Application.Current is { } app &&
        app.TryFindResource($"{controlType.Name}Preview", app.ActualThemeVariant, out var template)
            ? template
            : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
