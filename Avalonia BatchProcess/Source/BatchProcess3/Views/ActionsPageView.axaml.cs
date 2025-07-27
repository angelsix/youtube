using Avalonia.Controls;
using BatchProcess3.MainApp;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Views;

public partial class ActionsPageView : UserControl
{
    public ActionsPageView()
    {
        InitializeComponent();
    }

    private void ActionsTab_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (Equals(e.Source, ActionsTabControl)) OnTabChanged();
    }

    private void OnTabChanged()
    {
        // Get active tab control (Pages inside of each tab)
        var selectedPage = (ActionsTabControl?.SelectedItem as TabItem)?.Content as Control;

        if (selectedPage == null)
            return;

        // Convert to ActionsPageName
        var actionsPage = selectedPage switch
        {
            ActionsTabPrintView => ActionsPageName.Print,
            _ => ActionsPageName.Unknown,
        };

        // Get view model
        var viewModel = selectedPage.DataContext as ActionsPageViewModel;

        // Type check
        viewModel?.RefreshActionsPage(actionsPage);
    }

    protected override void OnInitialized()
    {
        // Fire off initial refresh
        OnTabChanged();
        
        base.OnInitialized();   
    }
}