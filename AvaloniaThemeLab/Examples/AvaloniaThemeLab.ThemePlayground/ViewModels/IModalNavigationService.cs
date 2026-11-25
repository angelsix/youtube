using System.Threading.Tasks;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>Navigates the modal page without exposing its host control.</summary>
public interface IModalNavigationService
{
    Task PushAsync(object? dataContext);
    Task PopAsync();
}
