using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace Avalonia.Themes.Default;

/// <summary>
/// Gives a flyout presenter the width of the control the flyout was opened from, so a
/// button's drop-down opens at the button's own width instead of at its content's width.
/// </summary>
/// <remarks>
/// <para>
/// A flyout presenter is created in code by the flyout, not by the control that owns it, and it
/// lives in the popup's own host, so the owning button cannot size it from its own template.
/// The presenter can, though: its logical parent is the <see cref="Primitives.Popup"/> that
/// hosts it, and the popup's <see cref="Primitives.Popup.PlacementTarget"/> is the control the
/// flyout was opened from. This converter reads that target and hands back its measured width,
/// which the presenter then uses as its minimum width.
/// </para>
/// <para>
/// Only a BUTTON's flyout is sized this way. A context flyout opened from a text field (or any
/// other non-button) keeps the width its own content asks for, so this converter hands back
/// <see cref="AvaloniaProperty.UnsetValue"/> for a target that is not a button, which leaves the
/// presenter's minimum width unset.
/// </para>
/// </remarks>
public class FlyoutPlacementTargetWidthConverter : IValueConverter
{
    /// <summary>A shared instance, so the theme does not construct one per use.</summary>
    public static FlyoutPlacementTargetWidthConverter Instance { get; } = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // The value is the flyout's placement target: the control the flyout was opened from.
        if (value is not Control target)
        {
            return AvaloniaProperty.UnsetValue;
        }

        // A button's drop-down opens at the button's width. Anything else (a text field's
        // context menu, a list's menu) keeps the width its content asks for.
        if (target is not Button && target is not SplitButton)
        {
            return AvaloniaProperty.UnsetValue;
        }

        // An unlaid-out target measures zero; leave the width alone rather than forcing the
        // flyout to zero.
        var width = target.Bounds.Width;
        return width > 0 ? width : AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException($"{nameof(FlyoutPlacementTargetWidthConverter)} is one-way.");
}
