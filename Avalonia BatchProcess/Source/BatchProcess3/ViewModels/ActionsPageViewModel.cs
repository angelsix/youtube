using BatchProcess3.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchProcess3.ViewModels;

public partial class ActionsPageViewModel() : PageViewModel(ApplicationPageNames.Actions)
{
    [ObservableProperty]
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
    private void FetchPrintList()
    {
        // TODO: Fetch from a database/service provider
        PrintList =
        [
            new ActionsPrintViewModel { Id = "1", JobName = "Print Only Drawings" },
            new ActionsPrintViewModel { Id = "2", JobName = "Print All Drawings Scale To Fit" },
            new ActionsPrintViewModel { Id = "3", JobName = "Print 3D Models A3" },
        ];
    }

    protected override void OnDesignTimeConstructor() => FetchPrintList();

    [RelayCommand]
    public void DeletePrintItem(string id)
    {
        // TODO: Pass this logic to a service that handles the database/storage/fetching
        //       For now just do it direct in here

        if (PrintList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;
        
        // Remove item
        PrintList.Remove(PrintList.First(x => x.Id == id));
    }
}