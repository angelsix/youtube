using Avalonia.Controls;
using Avalonia.Input;

namespace BatchProcess3.Views.Pages;

public partial class JobsPageView : UserControl
{
    public JobsPageView()
    {
        InitializeComponent();
        PointerPressed += OnPointerPressed;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Focus();
    }
}
