using Avalonia.Controls;
using AvaloniaThemeLab.ViewModels;

namespace AvaloniaThemeLab.Views;

public partial class ThemeSettingsWindow : Window
{
    public ThemeSettingsWindow()
    {
        InitializeComponent();

        // Tools that build the window directly (the previewer, guard xaml inspect) never pass a view
        // model; the app does, before the window is shown
        DataContext ??= new ThemeSettingsViewModel();
    }
}
