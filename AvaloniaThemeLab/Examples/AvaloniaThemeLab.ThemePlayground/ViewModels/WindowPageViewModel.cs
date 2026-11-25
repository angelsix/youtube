using System.Threading.Tasks;
using AngelSix.Mvvm;
using Avalonia.Controls;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>The Window page: real dialog windows stacked over the main window, and a modal page over a navigation page.</summary>
public sealed partial class WindowPageViewModel(IDialogWindowService? dialogs = null, IModalNavigationService? modalNavigation = null)
    : PageEntryViewModel(typeof(Window))
{
    #region Properties

    public IModalNavigationService? ModalNavigation { get; } = modalNavigation;

    #endregion Properties

    #region Commands

    /// <summary>Opens a dialog over the topmost window: the main window, or the dialog already open.</summary>
    public IRelayCommand OpenDialogCommand => Command(() => dialogs?.OpenDialog(this));

    public IRelayCommand CloseDialogCommand => Command(() => dialogs?.CloseTopDialog());

    public IAsyncRelayCommand PushModalCommand => AsyncCommand(() => ModalNavigation?.PushAsync(this) ?? Task.CompletedTask);

    public IAsyncRelayCommand PopModalCommand => AsyncCommand(() => ModalNavigation?.PopAsync() ?? Task.CompletedTask);

    #endregion Commands
}
