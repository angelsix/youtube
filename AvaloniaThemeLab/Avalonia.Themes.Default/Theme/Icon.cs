using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Avalonia.Themes.Default;

/// <summary>
/// An icon a control draws beside its content: <c>&lt;Button Icon.Data="{icons:Save}" Content="Save" /&gt;</c>.
/// </summary>
/// <remarks>
/// Attached rather than a Button subclass, so any button-like control opts in by reading these in its
/// template with <c>TemplateBinding</c>; a control whose template does not read them ignores them.
/// No prefix is needed because the assembly maps this namespace into Avalonia's XAML namespace.
/// </remarks>
public static class Icon
{
    /// <summary>The icon's geometry; null draws no icon and leaves no gap.</summary>
    public static readonly AttachedProperty<Geometry?> DataProperty =
        AvaloniaProperty.RegisterAttached<Control, Geometry?>("Data", typeof(Icon));

    /// <summary>The side of the content the icon sits on.</summary>
    public static readonly AttachedProperty<Dock> PlacementProperty =
        AvaloniaProperty.RegisterAttached<Control, Dock>("Placement", typeof(Icon), Dock.Left);

    public static Geometry? GetData(Control control) => control.GetValue(DataProperty);

    public static void SetData(Control control, Geometry? value) => control.SetValue(DataProperty, value);

    public static Dock GetPlacement(Control control) => control.GetValue(PlacementProperty);

    public static void SetPlacement(Control control, Dock value) => control.SetValue(PlacementProperty, value);
}
