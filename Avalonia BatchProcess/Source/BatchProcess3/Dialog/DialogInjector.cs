using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace BatchProcess3.Dialog;

public static class DialogInjector
{
    public static readonly AttachedProperty<bool> AddDialogProperty = AvaloniaProperty.RegisterAttached<Control, bool>("AddDialog", typeof(DialogInjector));
    
    public static bool GetAddDialog(Control control) => control.GetValue<bool>(AddDialogProperty);
    public static void SetAddDialog(Control control, bool value) => control.SetValue(AddDialogProperty, value);

    private static WeakReference<Grid?> _overlayGrid;
    private static WeakReference<ContentPresenter> _dialogHost;

    static DialogInjector()
    {
        AddDialogProperty.Changed.AddClassHandler<Control>((control, e) =>
        {
            var enabled = e.NewValue is bool and true;
            
            // Detach previous event
            control.AttachedToVisualTree -= ControlOnAttachedToVisualTree;

            if (!enabled) return;
            
            // Capture top-level if anything changes in the tree
            control.AttachedToVisualTree += ControlOnAttachedToVisualTree;
                
            // Try and get the top-level now
            if (control.GetVisualRoot() != null)
                TryInjectOverlay(control);
        });
    }
    
    private static void ControlOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Control control)
            TryInjectOverlay(control);
    }
    
    private static void TryInjectOverlay(Control control)
    {
        if (!Avalonia.Controls.Design.IsDesignMode)
            return;

        try
        {
            // Create grid that will house the overlay and original content
            var originalContent = control switch
            {
                UserControl uc => uc.Content as Control,
                Window window => window.Content as Control,
                _ => throw new InvalidCastException(control.ToString())
            };

            if (originalContent == null) return;
            
            var gridId = "DesignerDialogOverlay";
            
            // If this was the second call and already has a dialog... return
            if (originalContent is Grid grid && grid.Children.OfType<Grid>().Any(f => f.Name == gridId))
                return;
            
            // Wrap original content in grid
            var wrapper = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Update the original control to the wrapper
            if (control is UserControl uc2)
                uc2.Content = wrapper;
            else if (control is Window window2)
                window2.Content = wrapper;
            
            // Create dialog overlay
            var dialogOverlayGrid = new Grid
            {
                Name = gridId,
                IsHitTestVisible = false,
                IsVisible = false,
                Opacity = 0,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                [Panel.ZIndexProperty] = 100000
            };
            
            var dialogHostControl = new ContentPresenter
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            
            dialogOverlayGrid.Children.Add(new Border { Background = Brushes.Black, Opacity = 0.5});
            dialogOverlayGrid.Children.Add(dialogHostControl);
            
            // Add original content to new grid
            wrapper.Children.Add(originalContent);
            wrapper.Children.Add(dialogOverlayGrid);
            
            // Store weak references
            _overlayGrid = new WeakReference<Grid?>(dialogOverlayGrid);
            _dialogHost = new  WeakReference<ContentPresenter>(dialogHostControl);
        }
        catch (Exception _)
        {
            throw;
            // ignored
        }
    }

    public static bool TryShow(Control dialogView)
    {
        if (!Avalonia.Controls.Design.IsDesignMode)
            return false;

        if (_overlayGrid?.TryGetTarget(out var overlay) != true || overlay == null)
            return false;
        
        if (_dialogHost?.TryGetTarget(out var host) != true || host == null)
            return false;
        
        host.Content = dialogView;
        overlay.IsVisible = true;
        overlay.IsHitTestVisible = true;
        overlay.Opacity = 1;
        return true;
    }

    public static void Close()
    {
        if (_overlayGrid?.TryGetTarget(out var overlay) == true)
        {
            overlay.IsVisible = false;
            overlay.IsHitTestVisible = false;
            overlay.Opacity = 0;
        }
        
        if (_dialogHost?.TryGetTarget(out var host) == true)
            host.Content = null;
    }
}