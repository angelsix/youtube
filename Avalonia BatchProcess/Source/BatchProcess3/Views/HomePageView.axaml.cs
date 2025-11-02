using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Views;

public partial class HomePageView : UserControl
{
    public HomePageView()
    {
        InitializeComponent();
    }

    private void ActionsListBox_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control control && e.InitialPressMouseButton == MouseButton.Right)
            FlyoutBase.ShowAttachedFlyout(ActionsListBox);
    }

    private void ActionContextMenu_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (DataContext is HomePageViewModel viewModel && 
            e.InitialPressMouseButton == MouseButton.Left &&
            sender is Control { DataContext: AvailableActionItemViewModel itemViewModel })
        { 
            FlyoutBase.GetAttachedFlyout(ActionsListBox)?.Hide();
            viewModel.InsertAction(itemViewModel, ActionsListBox.SelectedIndex + 1);
        }
    }
}