using Avalonia;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.ThemePlayground.Views;

public sealed class InspectionThemeSettingsWindow : ThemeSettingsWindow
{
    public InspectionThemeSettingsWindow() => DataContext = ((App)Application.Current!).Services.GetRequiredService<ThemeSettingsViewModel>();
}
