using Avalonia.Media;

namespace AngelSix.ThemeEngine;

public sealed class DefaultTheme : Theme
{
    public override string ThemeName => "Default";

    public override Color ThemeColor1 => Color.Parse("#FFFFFFFF");
    public override Color ThemeColor2 => Color.Parse("#22000000");
    public override Color ThemeColor3 => Color.Parse("#44000000");
    public override Color ThemeColor4 => Color.Parse("#66000000");
    public override Color ThemeColor5 => Color.Parse("#AA000000");

    public override Color AccentColor => Color.Parse("#0F766E");
}
