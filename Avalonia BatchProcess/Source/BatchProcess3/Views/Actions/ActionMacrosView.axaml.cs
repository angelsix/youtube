using Avalonia.Controls;
using BatchProcess3.ViewModels;
using BatchProcess3.ViewModels.Actions;

namespace BatchProcess3.Views.Actions;

public partial class ActionMacrosView : UserControl
{
    public ActionMacrosView()
    {
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems?.Count > 0 && e.AddedItems[0] is ActionMacrosViewModel viewModel)
        {
            // When it is a newly created item
            if (viewModel.IsNewItem)
            {
                JobNameTextBox.SelectAll();
                JobNameTextBox.Focus();
            }
        }
    }
}