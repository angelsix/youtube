using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaThemeLab.ThemePlayground.Pages;
using AvaloniaThemeLab.ThemePlayground.ViewModels;

namespace AvaloniaThemeLab.ThemePlayground.Views;

/// <summary>Connects modal navigation to the currently displayed view without retaining a closed view.</summary>
public sealed class ModalNavigationService : IModalNavigationService
{
    private WeakReference<NavigationPage>? _host;

    public void Attach(NavigationPage host) => _host = new(host);

    public void Detach(NavigationPage host)
    {
        if (_host?.TryGetTarget(out var current) == true && ReferenceEquals(current, host))
            _host = null;
    }

    public Task PushAsync(object? dataContext) => _host?.TryGetTarget(out var host) == true
        ? host.PushModalAsync(new WindowPageModal { DataContext = dataContext })
        : Task.CompletedTask;

    public Task PopAsync() => _host?.TryGetTarget(out var host) == true
        ? host.PopModalAsync() : Task.CompletedTask;
}
