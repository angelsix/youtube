using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using BatchProcess3.ViewModels;
using System.Linq;

namespace BatchProcess3.Views;

public partial class ProcessPageView : UserControl
{
    public ProcessPageView()
    {
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems?.Count > 0 && e.AddedItems[0] is ProcessViewModel viewModel)
        {
            // When it is a newly created item
            if (viewModel.IsNewItem)
            {
                JobNameTextBox.SelectAll();
                JobNameTextBox.Focus();
            }
        }
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control control && e.InitialPressMouseButton == MouseButton.Right)
            FlyoutBase.ShowAttachedFlyout(ActionsListBox);
    }

    private void ActionContextMenu_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (DataContext is ProcessPageViewModel viewModel && 
            e.InitialPressMouseButton == MouseButton.Left &&
            sender is Control { DataContext: AvailableActionItemViewModel itemViewModel })
        { 
            FlyoutBase.GetAttachedFlyout(ActionsListBox)?.Hide();
            viewModel.InsertActionToProcess(itemViewModel, ActionsListBox.SelectedIndex + 1);
        }
    }
}