using Avalonia.Controls;
using AvaloniaThemeLab.ThemePlayground.Pages;
using AvaloniaThemeLab.ThemePlayground.Views;
using AngelSix.Mvvm;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>The Window page: real dialog windows stacked over the main window, and a modal page over a navigation page.</summary>
public sealed partial class WindowPageViewModel(DialogWindowService? dialogs = null)
    : PageEntryViewModel(typeof(Window), typeof(WindowPage))
{
    /// <summary>Opens a dialog over the topmost window: the main window, or the dialog already open.</summary>
    [Command]
    private void OpenDialog() => dialogs?.OpenDialog(this);

    [Command]
    private void CloseDialog() => dialogs?.CloseTopDialog();

    [Command]
    private void PushModal(NavigationPage? host)
    {
        if (host is not null)
            dialogs?.PushModal(host, this);
    }

    [Command]
    private void PopModal(NavigationPage? host)
    {
        if (host is not null)
            dialogs?.PopModal(host);
    }
}
