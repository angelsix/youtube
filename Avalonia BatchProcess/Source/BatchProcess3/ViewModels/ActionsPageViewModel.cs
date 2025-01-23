using BatchProcess3.Data;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatchProcess3.ViewModels;

public partial class ActionsPageViewModel() : PageViewModel(ApplicationPageNames.Actions)
{
    private ObservableCollection<ActionsPrintViewModel> _printList;

    [RelayCommand]
    public void RefreshActionsPage(ActionsPageName actionsPageName)
    {
        switch (actionsPageName)
        {
            case ActionsPageName.Print: FetchPrintList(); break;
        }
    }
    
    [RelayCommand]
    public void FetchPrintList()
    {
        // TODO: Fetch from a database/service provider
        _printList =
        [
            new ActionsPrintViewModel { Id = "1", JobName = "Print Only Drawings" },
            new ActionsPrintViewModel { Id = "1", JobName = "Print All Drawings Scale To Fit" },
            new ActionsPrintViewModel { Id = "1", JobName = "Print 3D Models A3" },
        ];
    }
}