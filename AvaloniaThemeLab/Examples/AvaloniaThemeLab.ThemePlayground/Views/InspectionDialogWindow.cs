using Avalonia;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.ThemePlayground.Views;

public sealed class InspectionDialogWindow : DialogWindow
{
    public InspectionDialogWindow() => DataContext = ((App)Application.Current!).Services.GetRequiredService<WindowPageViewModel>();
}
