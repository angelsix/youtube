using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ThemePlayground.ViewModels;

namespace AvaloniaThemeLab.ThemePlayground.Views;

public partial class MainWindow : Window
{
    // True while a left-button press that began on the sidebar list is still held down.
    private bool _sidebarPressInFlight;

    public MainWindow()
    {
        InitializeComponent();

        // The sidebar selects on pointer press, so a clicked row flips SelectedEntry mid-click.
        AddHandler(InputElement.PointerPressedEvent, OnSidebarPressTunnel, RoutingStrategies.Tunnel);
        AddHandler(InputElement.PointerReleasedEvent, OnSidebarReleaseBubble, RoutingStrategies.Bubble);
    }

    /// <summary>Tunnels ahead of the sidebar item's own selection flip to mark a navigating press.</summary>
    private void OnSidebarPressTunnel(object? sender, PointerPressedEventArgs e)
    {
        // Only a left press counts as a navigation gesture (right/middle drive context menus etc.), and only one that begins somewhere inside the sidebar list.
        var list = SidebarList();
        if (list is null || !e.Properties.IsLeftButtonPressed || e.Source is not Visual source)
            return;
        // Walk the press origin up its visual chain.
        foreach (var ancestor in source.GetSelfAndVisualAncestors())
        {
            if (ReferenceEquals(ancestor, list))
            {
                ArmSidebarNavigate();
                return;
            }
        }
    }

    /// <summary>Fires once the navigating press's release has routed; reveals the page past it.</summary>
    private void OnSidebarReleaseBubble(object? sender, PointerReleasedEventArgs e)
    {
        if (!_sidebarPressInFlight)
            return;
        FinishSidebarNavigate();
    }

    private void ArmSidebarNavigate()
    {
        _sidebarPressInFlight = true;
        if (DataContext is MainWindowViewModel vm)
            vm.ArmDeferredReveal();
    }

    private void FinishSidebarNavigate()
    {
        _sidebarPressInFlight = false;
        var vm = DataContext as MainWindowViewModel;
        if (vm is null)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            if (_sidebarPressInFlight)
                return; // Superseded by a newer press; that one owns the reveal
            vm.CompleteDeferredReveal();
        }, DispatcherPriority.Normal);
    }

    /// <summary>The sidebar entries list (row 1, column 0 of the root grid).</summary>
    private ListBox? SidebarList()
    {
        if (Content is Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is ListBox listBox && Grid.GetRow(child) == 1 && Grid.GetColumn(child) == 0)
                    return listBox;
            }
        }
        return null;
    }
}
