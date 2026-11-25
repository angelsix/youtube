using Avalonia;
using Avalonia.Controls;

namespace AngelSix.Themes.Default;

/// <summary>
/// Gives the default theme's ToggleSwitch knob a content slot, so a consumer can put a glyph inside the knob.
/// </summary>
public sealed class SwitchKnob
{
    #region Fields

    /// <summary>The content shown inside the knob, centred on its dot.</summary>
    public static readonly AttachedProperty<object?> ContentProperty =
        AvaloniaProperty.RegisterAttached<SwitchKnob, ToggleSwitch, object?>("Content");

    #endregion Fields

    #region Constructors

    private SwitchKnob()
    {
    }

    #endregion Constructors

    #region Methods

    /// <summary>Reads the content shown inside a switch's knob.</summary>
    /// <param name="toggle">The switch whose knob content to read.</param>
    /// <returns>The knob content, or null when the consumer has set none.</returns>
    public static object? GetContent(ToggleSwitch toggle) => toggle.GetValue(ContentProperty);

    /// <summary>Sets the content shown inside a switch's knob.</summary>
    /// <param name="toggle">The switch whose knob content to set.</param>
    /// <param name="value">The content to show inside the knob, centred on its dot.</param>
    public static void SetContent(ToggleSwitch toggle, object? value) => toggle.SetValue(ContentProperty, value);

    #endregion Methods
}
