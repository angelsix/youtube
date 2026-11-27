using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Controls;

namespace AngelSix.Themes.Default;

/// <summary>
/// Gives the default theme's ToggleSwitch knob a content slot, so a consumer can put a glyph inside the knob.
/// </summary>
public sealed partial class SwitchKnob
{
    /// <summary>The content shown inside the knob, centred on its dot.</summary>
    [Attached<ToggleSwitch>]
    public static readonly AttachedProperty<object?> ContentProperty;
}
