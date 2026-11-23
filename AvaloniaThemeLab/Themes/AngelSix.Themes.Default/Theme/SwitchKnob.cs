using Avalonia;
using Avalonia.Controls;

namespace AngelSix.Themes.Default;

/// <summary>
/// Gives the default theme's ToggleSwitch knob a content slot, so a consumer can put a glyph inside the knob.
/// </summary>
/// <remarks>
/// The knob is two bare ellipses with no content hook of its own, so an icon could only reach it by retemplating
/// the whole control. This attached property is that hook: set on a ToggleSwitch, the control theme's template
/// renders the content centred on the knob's dot. Set it in a <c>ToggleSwitch:checked</c> style and another for
/// the unchecked state, both from a page, and the knob's glyph swaps with the switch — a sun in one state and a
/// moon in the other. Leaving it unset draws nothing extra, so the knob's default appearance is exactly what it
/// was before the hook existed.
/// </remarks>
public sealed class SwitchKnob
{
    private SwitchKnob()
    {
    }

    /// <summary>The content shown inside the knob, centred on its dot.</summary>
    public static readonly AttachedProperty<object?> ContentProperty =
        AvaloniaProperty.RegisterAttached<SwitchKnob, ToggleSwitch, object?>("Content");

    /// <summary>Reads the content shown inside a switch's knob.</summary>
    /// <param name="toggle">The switch whose knob content to read.</param>
    /// <returns>The knob content, or null when the consumer has set none.</returns>
    public static object? GetContent(ToggleSwitch toggle) => toggle.GetValue(ContentProperty);

    /// <summary>Sets the content shown inside a switch's knob.</summary>
    /// <param name="toggle">The switch whose knob content to set.</param>
    /// <param name="value">The content to show inside the knob, centred on its dot.</param>
    public static void SetContent(ToggleSwitch toggle, object? value) => toggle.SetValue(ContentProperty, value);
}
