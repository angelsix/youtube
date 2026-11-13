using System;
using System.Collections.Generic;
using System.Linq;
using AngelSix.Mvvm;
using AngelSix.Themes.Default;
using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>The settings window: the app's live theme instance, edited in place.</summary>
public sealed partial class ThemeSettingsViewModel : ObservableObject
{
    /// <summary>The theme every window is drawing with, edited in place.</summary>
    public DefaultTheme Theme { get; }

    public IReadOnlyList<FontFamily> Fonts { get; }

    public IReadOnlyList<HorizontalAlignment> HorizontalAlignments { get; } = Enum.GetValues<HorizontalAlignment>();

    public IReadOnlyList<VerticalAlignment> VerticalAlignments { get; } = Enum.GetValues<VerticalAlignment>();

    public ThemeSettingsViewModel(DefaultTheme theme)
    {
        Theme = theme;

        // The theme's own family first, so the current value is an item the list can select
        Fonts = [Theme.FontFamily, .. FontManager.Current.SystemFonts.OrderBy(font => font.Name)];
    }

    /// <summary>Puts every editable value back to what the theme ships with.</summary>
    [Command]
    private void Reset() => Theme.ResetToDefaults();
}
