using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// Turns one entry of <c>DataValidationErrors.Errors</c> into the text to show the user.
/// </summary>
/// <remarks>
/// The collection is <c>IEnumerable</c> of object: binding validation puts <see cref="Exception"/>
/// instances in it, while <c>INotifyDataErrorInfo</c> sources usually put strings. Rendering an
/// entry directly gives "System.Exception: Value is not valid" for the first case, so unwrap the
/// message and fall back to the value's own text for everything else.
/// </remarks>
public class ValidationErrorMessageConverter : IValueConverter
{
    /// <summary>A shared instance, so the theme does not construct one per use.</summary>
    public static ValidationErrorMessageConverter Instance { get; } = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        Exception ex => ex.Message,
        null => null,
        _ => value.ToString()
    };

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException($"{nameof(ValidationErrorMessageConverter)} is one-way.");
}
