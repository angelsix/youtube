using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BatchProcess3.Dialog;

public static class TopLevelLocator
{
    private static WeakReference<TopLevel?> _topLevel;
    
    public static readonly AvaloniaProperty<bool> CaptureProperty = AvaloniaProperty.RegisterAttached<Control, bool>("Capture", typeof(TopLevelLocator));

    public static bool GetCapture(Control control) => control.GetValue<bool>(CaptureProperty);
    public static void SetCapture(Control control, bool value) => control.SetValue(CaptureProperty, value);

    static TopLevelLocator()
    {
        CaptureProperty.Changed.AddClassHandler<Control>((control, e) =>
        {
            var enabled = e.NewValue is bool and true;

            // Detach previous event
            control.AttachedToVisualTree -= ControlOnAttachedToVisualTree;

            if (!enabled) return;
            
            // Capture top-level if anything changes in the tree
            control.AttachedToVisualTree += ControlOnAttachedToVisualTree;
                
            // Try and get the top-level now
            TryCaptureTopLevel(control);
        });
    }

    private static void ControlOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Control control)
            TryCaptureTopLevel(control);
    }

    private static void TryCaptureTopLevel(Control control)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(control);
            if (topLevel != null)
                _topLevel = new WeakReference<TopLevel?>(topLevel);
        }
        catch (Exception _)
        {
            // ignored
        }
    }

    public static void AddTopLevelProvider(this IServiceCollection collection)
    {
        collection.AddSingleton<Func<TopLevel?>>(sp => () =>
        {
            // If we have a design-time set topLevel, use that
            if (Avalonia.Controls.Design.IsDesignMode)
            {
                TopLevel? topLevel = null;
                _topLevel?.TryGetTarget(out topLevel);
                return topLevel;
            }
            
            // Standard runtime get of top level
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime topWindow)
                return TopLevel.GetTopLevel(topWindow.MainWindow);
            else if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
                return TopLevel.GetTopLevel(singleViewPlatform.MainView);

            return null;
        });
    }
}