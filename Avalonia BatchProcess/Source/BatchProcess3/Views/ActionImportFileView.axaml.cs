using Avalonia.Controls;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Views;

public partial class ActionImportFileView : UserControl
{
    public ActionImportFileView()
    {
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems?.Count > 0 && e.AddedItems[0] is ActionImportFileViewModel viewModel)
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