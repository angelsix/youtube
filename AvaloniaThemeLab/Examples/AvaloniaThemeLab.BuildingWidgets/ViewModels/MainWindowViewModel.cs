using System;
using System.Collections.Generic;
using AngelSix.Mvvm;
using AvaloniaThemeLab.BuildingWidgets.Theme;

namespace AvaloniaThemeLab.BuildingWidgets.ViewModels;

/// <summary>One gallery entry pairing a display label with its ViewModel.</summary>
public sealed record WidgetEntry(string Name, ViewModelBase ViewModel);

public sealed partial class MainWindowViewModel(
    [Property] PrototypeTheme theme,
    ExampleContentViewModel exampleContent,
    SideNavViewModel sideNav,
    TopMenuViewModel topMenu,
    AppShellViewModel appShell,
    MoonWidgetViewModel moonWidget,
    ModeToggleViewModel modeToggle,
    ThemeSettingsViewModel themeSettings,
    Action openSettings) : ViewModelBase
{
    #region Properties

    public IReadOnlyList<WidgetEntry> Widgets { get; } =
    [
        new("1. Example content", exampleContent),
        new("2. Side navigation", sideNav),
        new("3. Top menu", topMenu),
        new("4. App shell", appShell),
        new("5. Weather widget (Moon)", moonWidget),
        new("6. Mode toggle", modeToggle),
        new("7. Theme settings", themeSettings),
    ];

    #endregion Properties

    #region Observable Properties

    [Observable] public partial WidgetEntry SelectedWidget { get; set; } = new("1. Example content", exampleContent);

    #endregion Observable Properties

    #region Commands

    public IRelayCommand OpenSettingsCommand => Command(openSettings);

    #endregion Commands
}
