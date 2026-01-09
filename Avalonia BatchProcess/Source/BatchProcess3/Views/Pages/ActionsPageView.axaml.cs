using Avalonia.Controls;
using BatchProcess3.MainApp;
using BatchProcess3.ViewModels;
using BatchProcess3.ViewModels.Pages;
using BatchProcess3.Views.Actions;

namespace BatchProcess3.Views.Pages;

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
            ActionPrintView => ActionsPageName.Print,
            ActionCustomPropertiesView => ActionsPageName.CustomProperties,
            ActionFileInfoView => ActionsPageName.FileInfo,
            ActionSaveModelView => ActionsPageName.SaveModelAs,
            ActionSaveDrawingView => ActionsPageName.SaveDrawingAs,
            ActionImportFileView => ActionsPageName.ImportFile,
            ActionDrawingTemplateView => ActionsPageName.DrawingTemplates,
            ActionMacrosView => ActionsPageName.Macros,
            // Unknown page return Print
            _ => ActionsPageName.Print
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