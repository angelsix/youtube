using System;
using AngelSix.Mvvm;
using AngelSix.Themes.Default;

namespace AvaloniaThemeLab.GettingStarted.ViewModels;

public sealed partial class MainWindowViewModel([Property] DefaultTheme theme) : ViewModelBase
{
    #region Properties

    public string Greeting => "Welcome to the default theme";

    #endregion Properties

    #region Commands

    public IRelayCommand ToggleDarkCommand => Command(() => Theme.IsDark = !Theme.IsDark);

    public IRelayCommand GrowCommand => Command(() => Theme.BaseSize += 0.25);

    public IRelayCommand ShrinkCommand => Command(() => Theme.BaseSize = Math.Max(0.5, Theme.BaseSize - 0.25));

    #endregion Commands
}
