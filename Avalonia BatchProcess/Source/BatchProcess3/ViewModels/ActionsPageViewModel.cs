using BatchProcess3.Data;
using BatchProcess3.Services;
using BatchProcess3.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class ActionsPageViewModel(MainViewModel mainViewModel, DialogService dialogService) : PageViewModel(ApplicationPageNames.Actions)
{
    // TODO: Remove once we have database service
    private PrintProfileViewModel _defaultPrinterProfile = new PrintProfileViewModel
    {
        Id = "0", Name = "(Default)", Description = "Use all default settings", Copies = 1,
        // TODO: Populate PrinterSettings
    };

    // Design time only
    public ActionsPageViewModel() : this(new MainViewModel(), new DialogService()) { }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PrintListHasItems))]
    private ObservableCollection<ActionsPrintViewModel> _printList = [];
    
    public bool PrintListHasItems => PrintList.Any();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedPrintListItem))]
    private string _selectedPrintListItemId = "";

    public ActionsPrintViewModel? SelectedPrintListItem =>
        PrintList.FirstOrDefault(f => f.Id == SelectedPrintListItemId);
    
    [ObservableProperty]
    private ObservableCollection<PrintProfileViewModel> _printerProfiles = [];

    [RelayCommand]
    public void RefreshActionsPage(ActionsPageName actionsPageName)
    {
        switch (actionsPageName)
        {
            case ActionsPageName.Print: FetchPrintActionsData(); break;
        }
    }

    [RelayCommand]
    private void FetchPrintProfiles()
    {
        // TODO: Pull from database 
        
        PrinterProfiles =
        [
            _defaultPrinterProfile,
            new PrintProfileViewModel
            {
                Id = "1",
                Name = "Print Landscape",
                Description = "Print all files in landscape mode, 3 copies",
                Copies = 3,
                // TODO: Populate PrinterSettings
            },
            new PrintProfileViewModel
            {
                Id = "2",
                Name = "Print Portrait",
                Description = "Print all files in portait mode",
                Copies = 1,
                // TODO: Populate PrinterSettings
            },
            new PrintProfileViewModel
            {
                Id = "3",
                Name = "B&W A3",
                Description = "Make all A3 prints black and white",
                Copies = 5,
                // TODO: Populate PrinterSettings
            }
        ];
    }
    
    [RelayCommand]
    private void FetchPrintActionsData()
    {
        FetchPrintProfiles();
        
        // TODO: Fetch from a database/service provider
        PrintList =
        [
            new ActionsPrintViewModel { Id = "1", 
                JobName = "Print Only Drawings", 
                Description = "Prints only drawing files", 
                PrintDrawingRange = "0, 5, 7-8", 
                PrintDrawings = true, 
                DrawingExclusionList = $"Some item 1{System.Environment.NewLine}Some item 2{System.Environment.NewLine}Some item 3",
                PrinterProfileId = "1"
            },
            new ActionsPrintViewModel { Id = "2", JobName = "Print All Drawings Scale To Fit", Description = "Prints drawing scaled to fit the paper", PrintDrawings = true, PrinterProfileId = "2"},
            new ActionsPrintViewModel { Id = "3", JobName = "Print 3D Models A3", Description = "Prints models as 3D visuals", PrintModels = true, PrinterProfileId = "3" },
        ];

        // Update PrintListHasItems when collection changes
        PrintList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintListHasItems));

        if (PrintList.Count > 0)
        {
            // Select first item
            SelectedPrintListItemId = PrintList.First().Id;
            
            // Store last fetched database save states
            foreach (var printItem in PrintList)
                printItem.SetSavedState();
        }
    }

    protected override void OnDesignTimeConstructor() => FetchPrintActionsData();
    
    [RelayCommand]
    private async Task DeletePrintSettingsAsync(string id)
    {
        // TODO: Pass this logic to a service that handles the database/storage/fetching
        //       For now just do it direct in here

        if (PrinterProfiles.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;
        
        // TODO: Delete from database, then re-fetch to update UI
        //       1. Delete from database
        //       2. FetchPrintProfiles();

        await DeletePrintProfileFromUIAsync(id);
    }
    
    [RelayCommand]
    private async Task DeletePrintItemAsync(string id)
    {
        // TODO: Pass this logic to a service that handles the database/storage/fetching
        //       For now just do it direct in here

        if (PrintList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        await DeletePrintItemFromUIAsync(id);
    }

    [RelayCommand]
    private async Task EditPrintSettingsAsync(string id)
    {
        // TODO: Pass this logic to a service that handles database etc...

        var profileViewModel = PrinterProfiles.FirstOrDefault(f => f.Id == id);

        if (profileViewModel == null)
            // TODO: Throw/warn?
            return;

        // Copy view model
        var copiedProfileViewModel = new PrintProfileViewModel();
        copiedProfileViewModel.RestoreState(profileViewModel.GetState());
        
        await dialogService.ShowDialog(mainViewModel, copiedProfileViewModel);

        // Ignore if we clicked cancel
        if (!copiedProfileViewModel.Confirmed)
            return;
        
        // TODO: Database stuff
        
        // Commit copied view model back
        profileViewModel.RestoreState(copiedProfileViewModel.GetState());
    }

    [RelayCommand]
    private void AddNewPrintItem()
    {
        // TODO: Fetch new item defaults from a service provider
        // Create a new item
        var newItem = new ActionsPrintViewModel
        {
            Id = Guid.NewGuid().ToString("N"),
            IsNewItem = true,
            JobName = "New Print Item",
            PrinterProfileId = "0"
        };

        // Add to the print list
        PrintList.Add(newItem);
        
        // Select item
        SelectedPrintListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task AddNewPrintSettingsAsync()
    {
        var confirmViewModel = new PrintProfileViewModel()
        {
            Title = $"Printer settings",
            Message = "Are you sure you want to delete this print?",
            DialogWidth = 500,
            // OnConfirm = async (vm) =>
            // {
            //     await Task.Delay(2000);
            //
            //     vm.ProgressText = "This is taking a while...";
            //
            //     await Task.Delay(2000);
            //     
            //     vm.StatusText = "Oh no, something went wrong...";
            //
            //     return true;
            // }
        };

        await dialogService.ShowDialog(mainViewModel, confirmViewModel);

        // Ignore if we clicked cancel
        if (!confirmViewModel.Confirmed)
            return;
    }
    
    [RelayCommand]
    private async Task CancelPrintItem()
    {
        // Ignore if nothing is selected
        if (SelectedPrintListItem == null)
            return;
        
        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedPrintListItem.IsNewItem)
            await DeletePrintItemFromUIAsync(SelectedPrintListItem.Id, warn: false);
        else
            SelectedPrintListItem.RestoreState();
    }

    // ReSharper disable once InconsistentNaming
    private async Task DeletePrintProfileFromUIAsync(string id, bool warn = true)
    {
        var index = PrinterProfiles.IndexOf(PrinterProfiles.First(x => x.Id == id));
        if (index == -1)
            return;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = $"Delete Print Profile?",
                Message = $"Are you sure you want to delete '{PrinterProfiles[index].Name}'?",
                DialogWidth = 500,
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return;
        }

        // Remove item
        PrinterProfiles.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (PrinterProfiles.Count > 0)
            SelectedPrintListItem!.PrinterProfileId = PrinterProfiles[index].Id;
    }
    
    // ReSharper disable once InconsistentNaming
    private async Task DeletePrintItemFromUIAsync(string id, bool warn = true)
    {
        var index = PrintList.IndexOf(PrintList.First(x => x.Id == id));
        if (index == -1)
            return;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = $"Delete Print Item?",
                Message = $"Are you sure you want to delete ' {PrintList[index].JobName}'?",
                DialogWidth = 500,
                // OnConfirm = async (vm) =>
                // {
                //     await Task.Delay(2000);
                //
                //     vm.ProgressText = "This is taking a while...";
                //
                //     await Task.Delay(2000);
                //     
                //     vm.StatusText = "Oh no, something went wrong...";
                //
                //     return true;
                // }
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return;
        }

        // Remove item
        PrintList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (PrintList.Count > 0)
            SelectedPrintListItemId = PrintList[index].Id;
    }
}