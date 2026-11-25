using System;
using System.Collections.Generic;
using System.Linq;
using AngelSix.Mvvm;
using AngelSix.Themes.Default;
using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>The settings window: the app's live theme instance, edited in place.</summary>
public sealed partial class ThemeSettingsViewModel(DefaultTheme theme) : ObservableObject
{
    #region Properties

    /// <summary>The theme every window is drawing with, edited in place.</summary>
    public DefaultTheme Theme { get; } = theme;

    // The theme's own family first, so the current value is an item the list can select
    public IReadOnlyList<FontFamily> Fonts { get; } =
        [theme.FontFamily, .. FontManager.Current.SystemFonts.OrderBy(font => font.Name, StringComparer.OrdinalIgnoreCase)];

    public IReadOnlyList<HorizontalAlignment> HorizontalAlignments { get; } = Enum.GetValues<HorizontalAlignment>();

    public IReadOnlyList<VerticalAlignment> VerticalAlignments { get; } = Enum.GetValues<VerticalAlignment>();

    #endregion Properties

    #region Commands

    /// <summary>Puts every editable value back to what the theme ships with.</summary>
    public IRelayCommand ResetCommand => Command(Theme.ResetToDefaults);

    #endregion Commands
}
