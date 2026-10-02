using Avalonia;
using AvaloniaThemeLab.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.Views;

/// <summary>
/// The main window with its view model taken from the app's container, for `guard xaml inspect`,
/// which constructs its view without arguments and never runs the desktop start-up that wires it.
/// </summary>
public sealed class InspectionMainWindow : MainWindow
{
    public InspectionMainWindow() => DataContext = ((App)Application.Current!).Services.GetRequiredService<MainWindowViewModel>();
}
