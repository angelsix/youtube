using Avalonia;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.ThemePlayground.Views;

public sealed class InspectionWindowPage : Pages.WindowPage
{
    public InspectionWindowPage() => DataContext = ((App)Application.Current!).Services.GetRequiredService<WindowPageViewModel>();
}
