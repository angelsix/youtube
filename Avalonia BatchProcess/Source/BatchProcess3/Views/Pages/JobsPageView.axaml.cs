using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BatchProcess3.ViewModels.Pages;

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

    private void JobRow_OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border { DataContext: JobItemViewModel job } &&
            DataContext is JobsPageViewModel vm)
        {
            vm.OpenJobDetailCommand.Execute(job);
        }
    }
}
