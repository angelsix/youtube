using Avalonia.Controls;
using AvaloniaThemeLab.Pages;
using AvaloniaThemeLab.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>The Window page: real dialog windows stacked over the main window, and a modal page over a navigation page.</summary>
/// <remarks>The service is absent in design-time and headless tools, which have no window to open over.</remarks>
public sealed partial class WindowPageViewModel(DialogWindowService? dialogs = null)
    : PageEntryViewModel(typeof(Window), typeof(WindowPage))
{
    /// <summary>Opens a dialog over the topmost window: the main window, or the dialog already open.</summary>
    [RelayCommand]
    private void OpenDialog() => dialogs?.OpenDialog(this);

    [RelayCommand]
    private void CloseDialog() => dialogs?.CloseTopDialog();

    [RelayCommand]
    private void PushModal(NavigationPage? host)
    {
        if (host is not null)
            dialogs?.PushModal(host, this);
    }

    [RelayCommand]
    private void PopModal(NavigationPage? host)
    {
        if (host is not null)
            dialogs?.PopModal(host);
    }
}
