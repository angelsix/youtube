using System;
using AngelSix.Mvvm;
using AngelSix.Themes.Default;

namespace AvaloniaThemeLab.GettingStarted.ViewModels;

public partial class MainWindowViewModel(DefaultTheme theme) : ViewModelBase
{
    public DefaultTheme Theme { get; } = theme;

    public string Greeting => "Welcome to the default theme";

    [Command]
    private void ToggleDark() => Theme.IsDark = !Theme.IsDark;

    [Command]
    private void Grow() => Theme.BaseSize += 0.25;

    [Command]
    private void Shrink() => Theme.BaseSize = Math.Max(0.5, Theme.BaseSize - 0.25);
}
