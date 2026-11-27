using System;
using System.Collections.Generic;
using System.Linq;
using AngelSix.Mvvm;
using Avalonia.Media;
using AvaloniaThemeLab.BuildingWidgets.Theme;

namespace AvaloniaThemeLab.BuildingWidgets.ViewModels;

public sealed partial class ThemeSettingsViewModel(PrototypeTheme theme) : ViewModelBase
{
    #region Properties

    public PrototypeTheme Theme { get; } = theme;

    public IReadOnlyList<FontFamily> Fonts { get; } =
        [theme.FontFamily, .. FontManager.Current.SystemFonts.OrderBy(font => font.Name, StringComparer.OrdinalIgnoreCase)];

    #endregion Properties

    #region Commands

    public IRelayCommand ResetCommand => Command(Theme.ResetToDefaults);

    #endregion Commands
}
