using System;
using System.Runtime.CompilerServices;
using AngelSix.ThemeEngine.Colors;
using AngelSix.ThemeEngine.Generated;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AngelSix.Themes.Default;

/// <summary>
/// Paints a light/dark region's surface on the element the region is set on.
/// </summary>
/// <remarks>
/// <c>ThemeRegion.IsDark</c> switches the palette tokens read beneath it, but the palette alone paints nothing:
/// a TextBox's theme leaves it transparent so it sits on any surface, and a bare Border has no background at all.
/// Set dark on either in a light window and the dark-palette text lands on the light window behind it.
/// So the element the region is set on (not the elements that inherit it) takes the region's
/// <c>SurfaceDefaultBrush</c>, at Template priority: under a Background you set yourself, and under the
/// theme's own hover and pressed states, which are style triggers. Only an element its theme leaves without a
/// visible background is painted, so a control that already draws its own face keeps it.
/// </remarks>
internal static class RegionSurface
{
    private static readonly ConditionalWeakTable<Control, IDisposable> Painted = [];
    private static bool registered;

    public static void Register()
    {
        if (registered)
            return;

        registered = true;
        ThemeRegion.IsDarkProperty.Changed.AddClassHandler<Control>(OnRegionChanged);
    }

    private static void OnRegionChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        // A value arriving by inheritance is the region's contents, which sit on the region's surface
        if (e.Priority == BindingPriority.Inherited || !HasBackground(control))
            return;

        Unpaint(control);
        control.Loaded -= OnLoaded;

        if (ThemeRegion.GetIsDark(control) is null)
            return;

        // The theme's own Background is only known once styles have applied
        if (control.IsLoaded)
            Paint(control);
        else
            control.Loaded += OnLoaded;
    }

    private static void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var control = (Control)sender!;
        control.Loaded -= OnLoaded;
        if (ThemeRegion.GetIsDark(control) is not null)
            Paint(control);
    }

    private static void Paint(Control control)
    {
        if (Painted.TryGetValue(control, out _))
            return;

        // A Background set on the element itself is the author's choice, whatever it is
        if (control.GetDiagnostic(Border.BackgroundProperty).Priority == BindingPriority.LocalValue)
            return;

        var current = control.GetValue(Border.BackgroundProperty);
        if (current is not null && !(current is ISolidColorBrush { Color.A: 0 }))
            return;

        var surface = (MultiBinding)new SurfaceDefaultBrushExtension().ProvideValue(null!);
        surface.Priority = BindingPriority.Template;
        Painted.AddOrUpdate(control, control.Bind(Border.BackgroundProperty, surface));
    }

    private static void Unpaint(Control control)
    {
        if (Painted.TryGetValue(control, out var binding))
        {
            binding.Dispose();
            Painted.Remove(control);
        }
    }

    private static bool HasBackground(Control control) =>
        AvaloniaPropertyRegistry.Instance.IsRegistered(control, Border.BackgroundProperty);
}
