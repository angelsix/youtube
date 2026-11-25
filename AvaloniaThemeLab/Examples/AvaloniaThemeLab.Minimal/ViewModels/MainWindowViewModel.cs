using System;
using AngelSix.Mvvm;
using AvaloniaThemeLab.Minimal.Theme;

namespace AvaloniaThemeLab.Minimal.ViewModels;

public sealed partial class MainWindowViewModel(MinimalTheme theme) : ViewModelBase
{
    #region Properties

    public MinimalTheme Theme { get; } = theme;

    public string Greeting => "Welcome to a minimal theme";

    #endregion Properties

    #region Commands

    public IRelayCommand ToggleDarkCommand => Command(() => Theme.IsDark = !Theme.IsDark);

    public IRelayCommand GrowCommand => Command(() => Theme.BaseSize += 0.25);

    public IRelayCommand ShrinkCommand => Command(() => Theme.BaseSize = Math.Max(0.5, Theme.BaseSize - 0.25));

    #endregion Commands
}
