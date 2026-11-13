using System;
using AvaloniaThemeLab.Minimal.Theme;
using AngelSix.Mvvm;

namespace AvaloniaThemeLab.Minimal.ViewModels;

public partial class MainWindowViewModel(MinimalTheme theme) : ViewModelBase
{
    public MinimalTheme Theme { get; } = theme;

    public string Greeting => "Welcome to a minimal theme";

    [Command]
    private void ToggleDark() => Theme.IsDark = !Theme.IsDark;

    [Command]
    private void Grow() => Theme.BaseSize += 0.25;

    [Command]
    private void Shrink() => Theme.BaseSize = Math.Max(0.5, Theme.BaseSize - 0.25);
}
