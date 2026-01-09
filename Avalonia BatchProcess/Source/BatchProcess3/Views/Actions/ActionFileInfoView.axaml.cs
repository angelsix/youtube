using Avalonia.Controls;
using BatchProcess3.ViewModels;
using BatchProcess3.ViewModels.Actions;

namespace BatchProcess3.Views.Actions;

public partial class ActionFileInfoView : UserControl
{
    public ActionFileInfoView()
    {
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems?.Count > 0 && e.AddedItems[0] is ActionFileInfoViewModel viewModel)
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