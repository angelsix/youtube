using Avalonia;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.ThemePlayground.Views;

public sealed class InspectionMainWindow : MainWindow
{
    public InspectionMainWindow() => DataContext = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
}
