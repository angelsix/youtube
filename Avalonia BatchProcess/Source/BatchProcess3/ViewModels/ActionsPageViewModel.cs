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
        
        PrinterSettings = new ObservableCollection<PrintSettingsViewModel>(settings.Select(f => new PrintSettingsViewModel
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            CanEdit = f.CanEdit,
            CanDelete = f.CanDelete,
            Copies = f.Copies,
            PrinterSettingProfiles = new ObservableCollection<PrintSettingsProfileViewModel>(f.PrinterSettingProfiles.Select(profile => new PrintSettingsProfileViewModel
            {
                Id = profile.Id,
                DrawingColor = new KeyValuePair<string, string>(profile.DrawingColor, profile.DrawingColor),
                Height = profile.Height,
                Orientation = new  KeyValuePair<string, string>(profile.Orientation, profile.Orientation),
                PaperSize = new KeyValuePair<string, string>(profile.PaperSize, profile.PaperSize),
                PrinterName = new KeyValuePair<string, string>(profile.PrinterName, profile.PrinterName),
                ScaleToFit = profile.ScaleToFit,
                SourceTray = new KeyValuePair<string, string>(profile.SourceTray, profile.SourceTray),
                Type = profile.Type,
                Width = profile.Width,
            }))
        }));
    }
    
    [RelayCommand]
    private void FetchPrintList()
    {
        FetchPrintSettings();
        
        var printList = databaseService.GetPrintList();
        
        PrintList = new ObservableCollection<ActionsTabPrintViewModel>(printList.Select(f => new ActionsTabPrintViewModel()
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
        // TODO: Pass this logic to a service that handles the database/storage/fetching
        //       For now just do it direct in here

        if (PrinterSettings.Count(x => x.Id == id) != 1)
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
        
        // TODO: Database stuff
        
        // Commit copied view model back
        profileViewModel.RestoreState(copiedProfileViewModel.GetState());
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
            
                printerSettingsItem.SourceTrayOptions = new ObservableCollection<KeyValuePair<string, string>>(
                    availablePrinters.FirstOrDefault(f => f.Name == printerSettingsItem.PrinterName.Value)?.SourceTrays ?? []
                );
                
                // Change paper size and source tray to first item
                printerSettingsItem.PaperSize = printerSettingsItem.PaperSizeOptions.FirstOrDefault();
                printerSettingsItem.SourceTray = printerSettingsItem.SourceTrayOptions.FirstOrDefault();
            };
        }
    }

    [RelayCommand]
    private void AddNewPrintItem()
    {
        // TODO: Fetch new item defaults from a service provider
        // Create a new item
        var newItem = new ActionsTabPrintViewModel
        {
            Id = Guid.NewGuid().ToString("N"),
            IsNewItem = true,
            JobName = "New Print Item",
            PrinterSettingsId = "0"
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
        var index = PrinterSettings.IndexOf(PrinterSettings.First(x => x.Id == id));
        if (index == -1)
            return;

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
                return;
        }

        // Remove item
        PrinterSettings.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (PrinterSettings.Count > 0)
            SelectedPrintListItem!.PrinterSettingsId = PrinterSettings[index].Id;
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