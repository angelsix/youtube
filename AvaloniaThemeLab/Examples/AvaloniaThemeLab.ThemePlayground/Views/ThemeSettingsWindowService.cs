using System;
using Avalonia.Controls;
using AvaloniaThemeLab.ThemePlayground.ViewModels;

namespace AvaloniaThemeLab.ThemePlayground.Views;

/// <summary>Opens the theme settings window over the main window, one at a time.</summary>
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
