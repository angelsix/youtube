using Avalonia;
using Avalonia.Controls;

namespace AvaloniaThemeLab.ThemePlayground.Views;

public sealed class InspectionExpanderPage : Avalonia.Controls.UserControl
{
    public InspectionExpanderPage() => Content = new AngelSix.ThemeEngine.Controls.LightDarkPreview
    {
        Title = "Expander",
        ContentTemplate = (Avalonia.Controls.Templates.IDataTemplate)Application.Current!.FindResource("ExpanderPreview")!,
    };
}
