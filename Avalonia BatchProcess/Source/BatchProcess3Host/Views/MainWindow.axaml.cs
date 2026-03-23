using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BatchProcess3Host.ViewModels;

namespace BatchProcess3Host.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void JobRow_OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border { DataContext: HostJobItemViewModel job } &&
            DataContext is MainWindowViewModel vm)
        {
            vm.OpenJobDetailCommand.Execute(job);
        }
    }
}
