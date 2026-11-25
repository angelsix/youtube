using System.Threading.Tasks;
using Avalonia.Controls;
using AngelSix.Mvvm;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>The Window page: real dialog windows stacked over the main window, and a modal page over a navigation page.</summary>
public sealed partial class WindowPageViewModel(IDialogWindowService? dialogs = null, IModalNavigationService? modalNavigation = null)
    : PageEntryViewModel(typeof(Window))
{
    public IModalNavigationService? ModalNavigation { get; } = modalNavigation;
    /// <summary>Opens a dialog over the topmost window: the main window, or the dialog already open.</summary>
    [Command]
    private void OpenDialog() => dialogs?.OpenDialog(this);

    [Command]
    private void CloseDialog() => dialogs?.CloseTopDialog();

    [Command]
    private Task PushModalAsync() => ModalNavigation?.PushAsync(this) ?? Task.CompletedTask;

    [Command]
    private Task PopModalAsync() => ModalNavigation?.PopAsync() ?? Task.CompletedTask;
}
