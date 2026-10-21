using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ViewModels;

namespace AvaloniaThemeLab.Views;

public partial class MainWindow : Window
{
    // True while a left-button press that began ON the sidebar list is still held down. Armed in the
    // TUNNEL phase (before the deep item flips the selection) and cleared when that press's release
    // has routed. See ArmSidebarNavigate / FinishSidebarNavigate.
    private bool _sidebarPressInFlight;

    public MainWindow()
    {
        InitializeComponent();

        // Design-time default data context: tools that construct this window directly (the XAML
        // previewer, guard xaml inspect) never run the desktop start-up that wires the gallery, so
        // fall back to the view model's design-time constructor. The running app replaces it with
        // the container's instance before the window is shown, so this is design-only in effect.
        DataContext ??= new MainWindowViewModel();

        // The sidebar selects on POINTER PRESS, so a clicked row flips SelectedEntry mid-click. Left
        // unrestrained, the page host would recreate the incoming page inside that same press, and the
        // click's RELEASE would then propagate into a freshly born overlay drawer, which the stock
        // SplitView treats as an outside click and light-dismisses (task 108 bullet 2). We therefore
        // hold the page back until the release has fully routed: arm on the press (tunnel phase, so we
        // beat the item's own selection flip), and reveal only once posted past the release. Keyboard
        // and programmatic selections never arm us, so they switch pages instantly.
        // Instance AddHandler (not the AddDisposableHandler extension): the window lives for the app
        // lifetime, so nothing needs disposing. Tunnel/Bubble picked explicitly — see the comments on
        // each handler for why the phases matter.
        AddHandler(InputElement.PointerPressedEvent, OnSidebarPressTunnel, RoutingStrategies.Tunnel);
        AddHandler(InputElement.PointerReleasedEvent, OnSidebarReleaseBubble, RoutingStrategies.Bubble);
    }

    /// <summary>Tunnels ahead of the sidebar item's own selection flip to mark a navigating press.</summary>
    private void OnSidebarPressTunnel(object? sender, PointerPressedEventArgs e)
    {
        // Only a LEFT press counts as a navigation gesture (right/middle drive context menus etc.),
        // and only one that begins somewhere inside the sidebar list. Attaching in the TUNNEL phase
        // means this runs before the deep ListBoxItem bubbles its press and flips SelectedEntry, so
        // the arming precedes the very selection change we intend to delay.
        var list = SidebarList();
        if (list is null || !e.Properties.IsLeftButtonPressed || e.Source is not Visual source)
            return;
        // Walk the press origin up its visual chain: a hit inside the sidebar list means the deepest
        // element under it, so the list itself appears among the source's ancestors.
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
        // No IsLeftButtonPressed test here: at RELEASE the button has already gone up, so the flag is
        // false by definition. _sidebarPressInFlight (set only for a LEFT press in the tunnel phase)
        // is the reliable signal that this release belongs to a navigating click.
        if (!_sidebarPressInFlight)
            return;
        FinishSidebarNavigate();
    }

    private void ArmSidebarNavigate()
    {
        _sidebarPressInFlight = true;
        // Arm the CURRENT view model: the running app wires one instance before the window is shown,
        // and the proof host assigns its own before any input arrives, so the press and its release
        // always observe the same instance.
        if (DataContext is MainWindowViewModel vm)
            vm.ArmDeferredReveal();
    }

    private void FinishSidebarNavigate()
    {
        _sidebarPressInFlight = false;
        var vm = DataContext as MainWindowViewModel;
        if (vm is null)
            return;

        // Post the reveal so it runs AFTER this release has finished propagating everywhere (including
        // the TopLevel.PointerReleased subscription the stock SplitView uses for light-dismiss). By the
        // time the page host materialises the incoming page, nothing new exists to catch the release.
        Dispatcher.UIThread.Post(() =>
        {
            if (_sidebarPressInFlight)
                return; // superseded by a newer press; that one owns the reveal
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
