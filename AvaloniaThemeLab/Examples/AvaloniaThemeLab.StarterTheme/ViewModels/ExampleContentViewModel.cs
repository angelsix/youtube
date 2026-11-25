using AngelSix.Mvvm;
using AvaloniaThemeLab.StarterTheme.Theme;

namespace AvaloniaThemeLab.StarterTheme.ViewModels;

public sealed partial class ExampleContentViewModel(AppTheme theme) : ViewModelBase
{
    #region Properties

    public AppTheme Theme { get; } = theme;

    #endregion Properties

    #region Commands

    public IRelayCommand ToggleModeCommand => Command(() => Theme.IsDark = !Theme.IsDark);

    public IRelayCommand BiggerCommand => Command(() => Theme.BaseSize = Math.Min(1.5, Math.Round(Theme.BaseSize + 0.1, 2)));

    public IRelayCommand SmallerCommand => Command(() => Theme.BaseSize = Math.Max(0.75, Math.Round(Theme.BaseSize - 0.1, 2)));

    #endregion Commands
}
