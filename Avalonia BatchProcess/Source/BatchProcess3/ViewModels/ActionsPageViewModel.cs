using BatchProcess3.Data;
using BatchProcess3.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchProcess3.ViewModels;

public partial class ActionsPageViewModel() : PageViewModel(ApplicationPageNames.Actions)
{
    // TODO: Remove once we have database service
    private ActionsPrinterProfileViewModel _defaultPrinterProfile = new ActionsPrinterProfileViewModel
    {
        Id = "0", Name = "(Default)", Description = "Use all default settings", Copies = 1,
        // TODO: Populate PrinterSettings
    };
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PrintListHasItems))]
    private ObservableCollection<ActionsPrintViewModel> _printList = [];
    
    public bool PrintListHasItems => PrintList.Any();
    
    [ObservableProperty]
    private ActionsPrintViewModel? _selectedPrintListItem;

    private ActionsPrinterProfileViewModel? _selectedPrinterProfileItem;
    public ActionsPrinterProfileViewModel? SelectedPrinterProfileItem
    {
        get => _selectedPrinterProfileItem;
        set
        {
            if (_selectedPrinterProfileItem != null && value == _selectedPrinterProfileItem)
                return;
            
            _selectedPrinterProfileItem = value;
        }
    }

    [ObservableProperty]
    private ObservableCollection<ActionsPrinterProfileViewModel> _printerProfiles = [];

    [RelayCommand]
    public void RefreshActionsPage(ActionsPageName actionsPageName)
    {
        switch (actionsPageName)
        {
            case ActionsPageName.Print: FetchPrintActionsData(); break;
        }
    }
    
    [RelayCommand]
    private void FetchPrintActionsData()
    {
        // TODO: Fetch from a database/service provider
        PrintList =
        [
            new ActionsPrintViewModel { Id = "1", 
                JobName = "Print Only Drawings", 
                Description = "Prints only drawing files", 
                PrintDrawingRange = "0, 5, 7-8", 
                PrintDrawings = true, 
                DrawingExclusionList = $"Some item 1{System.Environment.NewLine}Some item 2{System.Environment.NewLine}Some item 3",
                PrinterProfile = _defaultPrinterProfile
            },
            new ActionsPrintViewModel { Id = "2", JobName = "Print All Drawings Scale To Fit", Description = "Prints drawing scaled to fit the paper", PrintDrawings = true, PrinterProfile = _defaultPrinterProfile},
            new ActionsPrintViewModel { Id = "3", JobName = "Print 3D Models A3", Description = "Prints models as 3D visuals", PrintModels = true, PrinterProfile = _defaultPrinterProfile },
        ];

        // Update PrintListHasItems when collection changes
        PrintList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintListHasItems));

        if (PrintList.Count > 0)
        {
            // Select first item
            PrintList.First().IsSelected = true;
            
            // Store last fetched database save states
            foreach (var printItem in PrintList)
                printItem.SetSavedState();
        }

        PrinterProfiles =
        [
            _defaultPrinterProfile,
            new ActionsPrinterProfileViewModel
            {
                Id = "1",
                Name = "Print Landscape",
                Description = "Print all files in landscape mode, 3 copies",
                Copies = 3,
                // TODO: Populate PrinterSettings
            },
            new ActionsPrinterProfileViewModel
            {
                Id = "2",
                Name = "Print Portrait",
                Description = "Print all files in portait mode",
                Copies = 1,
                // TODO: Populate PrinterSettings
            },
            new ActionsPrinterProfileViewModel
            {
                Id = "3",
                Name = "B&W A3",
                Description = "Make all A3 prints black and white",
                Copies = 5,
                // TODO: Populate PrinterSettings
            }
        ];
    }

    protected override void OnDesignTimeConstructor() => FetchPrintActionsData();

    [RelayCommand]
    public void DeletePrintItem(string id)
    {
        // TODO: Pass this logic to a service that handles the database/storage/fetching
        //       For now just do it direct in here

        if (PrintList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        DeletePrintItemFromUI(id);
    }

    [RelayCommand]
    public void AddNewPrintItem()
    {
        // TODO: Fetch new item defaults from a service provider
        // Create a new item
        var newItem = new ActionsPrintViewModel
        {
            Id = Guid.NewGuid().ToString("N"),
            IsSelected = true, 
            IsNewItem = true,
            JobName = "New Print Item",
            PrinterProfile = _defaultPrinterProfile
        };

        // Add to the print list
        PrintList.Add(newItem);
    }

    [RelayCommand]
    public void CancelPrintItem()
    {
        // Ignore if nothing is selected
        if (SelectedPrintListItem == null)
            return;
        
        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedPrintListItem.IsNewItem)
            DeletePrintItemFromUI(SelectedPrintListItem.Id);
    }

    private void DeletePrintItemFromUI(string id)
    {
        // Remove item
        var index = PrintList.IndexOf(PrintList.First(x => x.Id == id));
        PrintList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (PrintList.Count > 0)
            PrintList[index].IsSelected = true;
    }
}