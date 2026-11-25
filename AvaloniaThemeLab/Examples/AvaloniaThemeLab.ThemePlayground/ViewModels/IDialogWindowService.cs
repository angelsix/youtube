namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>Opens and closes the window demonstration's dialogs.</summary>
public interface IDialogWindowService
{
    void OpenDialog(object? dataContext);
    void CloseTopDialog();
}
