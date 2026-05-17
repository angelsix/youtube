using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AngelSix.ThemeEngine;

public abstract class Theme : ObservableObject
{
    public abstract string ThemeName { get; }

    public abstract Color ThemeColor1 { get; }
    public abstract Color ThemeColor2 { get; }
    public abstract Color ThemeColor3 { get; }
    public abstract Color ThemeColor4 { get; }
    public abstract Color ThemeColor5 { get; }

    public virtual SolidColorBrush ThemeColor1Brush => new(ThemeColor1);
    public virtual SolidColorBrush ThemeColor2Brush => new(ThemeColor2);
    public virtual SolidColorBrush ThemeColor3Brush => new(ThemeColor3);
    public virtual SolidColorBrush ThemeColor4Brush => new(ThemeColor4);
    public virtual SolidColorBrush ThemeColor5Brush => new(ThemeColor5);

    public abstract Color AccentColor { get; }
    public virtual SolidColorBrush AccentBrush => new(AccentColor);

    public virtual Thickness ButtonBorderThemeThickness => new(1);
    public virtual Thickness ButtonPadding => new(8, 5, 8, 6);
    public virtual CornerRadius ControlCornerRadius => new(3);
}
