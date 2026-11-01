using System;
using Avalonia.Controls;
using AvaloniaThemeLab.ViewModels;

namespace AvaloniaThemeLab.Views;

/// <summary>Opens the theme settings window over the main window, one at a time.</summary>
/// <remarks>
/// The view model asks for the window through this service so it never constructs a view itself;
/// a second request brings the open window forward rather than opening another.
/// </remarks>
public sealed class ThemeSettingsWindowService(Func<TopLevel?> owner, Func<ThemeSettingsViewModel> viewModel)
{
    private ThemeSettingsWindow? _window;

    public void Show()
    {
        if (_window is not null)
        {
            _window.Activate();
            return;
        }

        _window = new ThemeSettingsWindow { DataContext = viewModel() };
        _window.Closed += (_, _) => _window = null;

        if (owner() is Window main)
            _window.Show(main);
        else
            _window.Show();
    }
}
