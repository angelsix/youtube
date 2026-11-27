using System;
using AngelSix.Mvvm;
using AvaloniaThemeLab.Minimal.Theme;

namespace AvaloniaThemeLab.Minimal.ViewModels;

public sealed partial class MainWindowViewModel(MinimalTheme theme) : ObservableObject
{
    #region Properties

    public MinimalTheme Theme { get; } = theme;

    #endregion Properties

    #region Commands

    public IRelayCommand ToggleDarkCommand => Command(() => Theme.IsDark = !Theme.IsDark);

    public IRelayCommand GrowCommand => Command(() => Theme.BaseSize = Math.Min(1.5, Math.Round(Theme.BaseSize + 0.1, 2)));

    public IRelayCommand ShrinkCommand => Command(() => Theme.BaseSize = Math.Max(0.75, Math.Round(Theme.BaseSize - 0.1, 2)));

    public IRelayCommand ResetCommand => Command(() =>
    {
        Theme.IsDark = false;
        Theme.BaseSize = 1.0;
    });

    #endregion Commands
}
