using Avalonia;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.ThemePlayground.Views;

public sealed class InspectionWindowPageModal : Pages.WindowPageModal
{
    public InspectionWindowPageModal() => DataContext = ((App)Application.Current!).Services.GetRequiredService<WindowPageViewModel>();
}
