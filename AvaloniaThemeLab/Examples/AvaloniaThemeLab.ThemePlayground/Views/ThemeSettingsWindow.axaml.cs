using System;
using Avalonia.Controls;

namespace AvaloniaThemeLab.ThemePlayground.Views;

public partial class ThemeSettingsWindow : Window
{
    public ThemeSettingsWindow()
    {
        InitializeComponent();
    }

    // A closed window lets go of the theme; until it is collected, its sliders would otherwise clamp every later change to their own ranges
    protected override void OnClosed(EventArgs e)
    {
        DataContext = null;
        base.OnClosed(e);
    }
}
