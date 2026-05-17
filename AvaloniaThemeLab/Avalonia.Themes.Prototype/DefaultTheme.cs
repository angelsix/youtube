using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Media;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// Default theme values for the prototype theme. Marked with <see cref="ThemeAttribute"/>
/// so the AngelSix.ThemeEngine source generator emits one <c>{theme:PropertyName}</c>
/// markup extension per public property below.
/// </summary>
/// <remarks>
/// Properties are immutable get-only — themes are swapped wholesale via
/// <see cref="ThemeContext.Active"/>, so this class doesn't need to implement
/// <see cref="System.ComponentModel.INotifyPropertyChanged"/>. Subclass and override
/// (or define a sibling [Theme] class) to ship alternative palettes.
/// </remarks>
[Theme]
public class DefaultTheme
{
    public string ThemeName => "Default";

    public Color ThemeColor1 => Color.Parse("#FFFFFFFF");
    public Color ThemeColor2 => Color.Parse("#22000000");
    public Color ThemeColor3 => Color.Parse("#44000000");
    public Color ThemeColor4 => Color.Parse("#66000000");
    public Color ThemeColor5 => Color.Parse("#AA000000");

    public SolidColorBrush ThemeColor1Brush => new(ThemeColor1);
    public SolidColorBrush ThemeColor2Brush => new(ThemeColor2);
    public SolidColorBrush ThemeColor3Brush => new(ThemeColor3);
    public SolidColorBrush ThemeColor4Brush => new(ThemeColor4);
    public SolidColorBrush ThemeColor5Brush => new(ThemeColor5);

    public Color AccentColor => Color.Parse("#0F766E");
    public SolidColorBrush AccentBrush => new(AccentColor);

    public Thickness ButtonBorderThemeThickness => new(1);
    public Thickness ButtonPadding => new(8, 5, 8, 6);
    public CornerRadius ControlCornerRadius => new(3);
}
