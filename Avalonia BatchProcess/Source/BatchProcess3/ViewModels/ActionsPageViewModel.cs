using BatchProcess3.Data;
using BatchProcess3.Services;
using BatchProcess3.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class ActionsPageViewModel(MainViewModel mainViewModel, DialogService dialogService, PrinterService printerService, DatabaseService databaseService) : PageViewModel(ApplicationPageNames.Actions)
{
    // Design time only
    public ActionsPageViewModel() : this(new MainViewModel(), new DialogService(() => null), new  PrinterService(), new DatabaseService(new ApplicationDbContext())) { }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PrintListHasItems))]
    private ObservableCollection<ActionsTabPrintViewModel> _printList = [];
    
    public bool PrintListHasItems => PrintList.Any();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedPrintListItem))]
    private string _selectedPrintListItemId = "";

    public ActionsTabPrintViewModel? SelectedPrintListItem =>
        PrintList.FirstOrDefault(f => f.Id == SelectedPrintListItemId);
    
    [ObservableProperty]
    private ObservableCollection<PrintSettingsViewModel> _printerSettings = [];

    [RelayCommand]
    public void RefreshActionsPage(ActionsPageName actionsPageName)
    {
        switch (actionsPageName)
        {
            case ActionsPageName.Print: FetchPrintList(); break;
        }
    }

    [RelayCommand]
    private void FetchPrintSettings()
    {
        var settings = databaseService.GetPrintSettings();

        PrinterSettings = settings.ToViewModels();
    }
    
    [RelayCommand]
    private void FetchPrintList()
    {
        FetchPrintSettings();
        
        var printList = databaseService.GetPrintList();
        
        PrintList = new ObservableCollection<ActionsTabPrintViewModel>(printList
            .OrderBy(f => f.JobName)
            .Select(f => new ActionsTabPrintViewModel()
        {
            Id = f.Id,
            JobName = f.JobName,
            Description = f.Description,
            DrawingExclusionIsWhiteList = f.DrawingExclusionIsWhiteList,
            DrawingExclusionList = f.DrawingExclusionList,
            PrintDrawingRange = f.PrintDrawingRange,
            PrintDrawings = f.PrintDrawings,
            PrinterSettingsId = f.PrinterSettingsId,
            PrintModels = f.PrintModels
        }));

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

    protected override void OnDesignTimeConstructor() => FetchPrintList();
    
    [RelayCommand]
    private async Task DeletePrintSettingsAsync(string id)
    {
        if (PrinterSettings.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;
        
        if (await DeletePrintSettingsFromUIAsync(id))
            databaseService.DeletePrintSettings(id);
    }
    
    [RelayCommand]
    private async Task DeletePrintItemAsync(string id)
    {
        if (PrintList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeletePrintItemFromUIAsync(id))
            // Delete from database
            databaseService.DeletePrintListItem(id);
    }

    [RelayCommand]
    private async Task EditPrintSettingsAsync(string id)
    {
        var profileViewModel = PrinterSettings.FirstOrDefault(f => f.Id == id);

        if (profileViewModel == null)
            // TODO: Throw/warn?
            return;

        // Copy view model
        var copiedProfileViewModel = new PrintSettingsViewModel();
        copiedProfileViewModel.RestoreState(profileViewModel.GetState());
        
        InjectPrinterDetails(copiedProfileViewModel);
        
        await dialogService.ShowDialog(mainViewModel, copiedProfileViewModel);

        // Ignore if we clicked cancel
        if (!copiedProfileViewModel.Confirmed)
            return;
        
        // Commit copied view model back
        profileViewModel.RestoreState(copiedProfileViewModel.GetState());
        databaseService.UpdatePrintSettings(copiedProfileViewModel.ToDataModel());
    }

    private void InjectPrinterDetails(PrintSettingsViewModel viewModel)
    {
        // Fetch live printers available on machine
        var availablePrinters = printerService.AvailablePrinters();
        
        var printerNameOptions = new ObservableCollection<KeyValuePair<string, string>>(
            availablePrinters.Select((f) => new KeyValuePair<string, string>(f.Id.ToString(), f.Name))
        );

        foreach (var printerSettingsItem in viewModel.PrinterSettingProfiles)
        {
            printerSettingsItem.PrinterNameOptions = printerNameOptions;

            printerSettingsItem.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(PrintSettingsProfileViewModel.PrinterName))
                    return;
                
                // Printer changed, update paper size and tray
                printerSettingsItem.PaperSizeOptions = new ObservableCollection<KeyValuePair<string, string>>(
                    availablePrinters.FirstOrDefault(f => f.Name == printerSettingsItem.PrinterName.Value)?.PaperSizes ?? []
                );
                
                printerSettingsItem.PaperSizeOptions.Insert(0, new KeyValuePair<string, string>("(Default)", "(Default)"));
            
                printerSettingsItem.SourceTrayOptions = new ObservableCollection<KeyValuePair<string, string>>(
                    availablePrinters.FirstOrDefault(f => f.Name == printerSettingsItem.PrinterName.Value)?.SourceTrays ?? []
                );

                printerSettingsItem.SourceTrayOptions.Insert(0, new KeyValuePair<string, string>("(Default)", "(Default)"));

                // Change paper size and source tray to first item
                if (!printerSettingsItem.PaperSizeOptions.Any(f => f.Value == printerSettingsItem.PaperSize.Value))
                    printerSettingsItem.PaperSize =  printerSettingsItem.PaperSizeOptions.FirstOrDefault();

                if (!printerSettingsItem.SourceTrayOptions.Any(f => f.Value == printerSettingsItem.SourceTray.Value))
                    printerSettingsItem.SourceTray = printerSettingsItem.SourceTrayOptions.FirstOrDefault();
            };
            
            // Force a printer name change for initial list
            printerSettingsItem.RaiseOnPropertyChanged(nameof(printerSettingsItem.PrinterName));
        }
    }

    [RelayCommand]
    private void AddNewPrintItem()
    {
        // Fetch printer settings
        var printerSettings = databaseService.GetPrintSettings();
        
        // Create a new item
        var newItem = new ActionsTabPrintViewModel
        {
            Id = Guid.NewGuid().ToString("N"),
            IsNewItem = true,
            JobName = "New Print Item",
            PrinterSettingsId = printerSettings.FirstOrDefault()?.Id
        };

        // Add to the print list
        PrintList.Add(newItem);
        
        // Select item
        SelectedPrintListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task AddNewPrintSettingsAsync()
    {
        var confirmViewModel = new PrintSettingsViewModel()
        {
            Name = "New Print Settings",
            PrinterSettingProfiles = databaseService.GetPrintSettingsProfiles().ToViewModels()
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
        
        // TODO: Remove once new confirm view model dialog is pulled from database
        confirmViewModel.RestoreState(confirmViewModel.GetState());
        
        InjectPrinterDetails(confirmViewModel);

        await dialogService.ShowDialog(mainViewModel, confirmViewModel);

        // Ignore if we clicked cancel
        if (!confirmViewModel.Confirmed)
            return;
        
        PrinterSettings.Add(confirmViewModel);
        databaseService.AddPrintSettings(confirmViewModel.ToDataModel());
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

    [RelayCommand]
    private async Task SavePrintItemAsync()
    {
        // Ignore if no selection
        if (SelectedPrintListItem == null)
            return;
        
        // If the selected item is new...
        if (SelectedPrintListItem.IsNewItem)
            databaseService.AddPrintListItem(SelectedPrintListItem.ToDataModel());
        else
            databaseService.UpdatePrintListItem(SelectedPrintListItem.ToDataModel());
        
        // Flag new item as not new
        SelectedPrintListItem.IsNewItem = false;
        SelectedPrintListItem.SetSavedState();
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeletePrintSettingsFromUIAsync(string id, bool warn = true)
    {
        var index = PrinterSettings.IndexOf(PrinterSettings.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = $"Delete Print Profile?",
                Message = $"Are you sure you want to delete '{PrinterSettings[index].Name}'?",
                DialogWidth = 500,
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        PrinterSettings.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (PrinterSettings.Count > 0)
            SelectedPrintListItem!.PrinterSettingsId = PrinterSettings[index].Id;

        return true;
    }
    
    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeletePrintItemFromUIAsync(string id, bool warn = true)
    {
        var index = PrintList.IndexOf(PrintList.First(x => x.Id == id));
        if (index == -1)
            return false;

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
                return false;
        }

        // Remove item
        PrintList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (PrintList.Count > 0)
            SelectedPrintListItemId = PrintList[index].Id;

        return true;
    }
}