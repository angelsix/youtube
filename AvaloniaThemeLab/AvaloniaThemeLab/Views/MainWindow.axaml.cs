using Avalonia.Controls;
using AvaloniaThemeLab.ViewModels;

namespace AvaloniaThemeLab.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Design-time default data context: tools that construct this window directly (the XAML
        // previewer, guard xaml inspect) never run the desktop start-up that wires the gallery, so
        // fall back to the view model's design-time constructor. The running app replaces it with
        // the container's instance before the window is shown, so this is design-only in effect.
        DataContext ??= new MainWindowViewModel();
    }
}
