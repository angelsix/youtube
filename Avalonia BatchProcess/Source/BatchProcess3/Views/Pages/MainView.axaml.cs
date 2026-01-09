using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BatchProcess3.ViewModels;
using BatchProcess3.ViewModels.Pages;

namespace BatchProcess3.Views.Pages;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
    
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount != 2)
            return;
        
        (DataContext as MainViewModel)?.SideMenuResizeCommand?.Execute(null);
    }
}