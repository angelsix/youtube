using Avalonia.Controls;

namespace AvaloniaThemeLab.BuildingWidgets.Views;

/// <summary>Popout theme settings dialog, pinned to <c>ThemeRegion.BaseSize="1"</c> in XAML so scale sliders never resize the dialog.</summary>
public partial class ThemeSettingsWindow : Window
{
    public ThemeSettingsWindow()
    {
        InitializeComponent();
    }
}
