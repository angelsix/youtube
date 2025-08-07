using BatchProcess3.CustomProperties;
using BatchProcess3.DataStorage;
using BatchProcess3.Dialog;
using BatchProcess3.MainApp;
using BatchProcess3.Printer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class ActionsPageViewModel(
    MainViewModel mainViewModel,
    DialogService dialogService,
    PrinterService printerService,
    DatabaseService databaseService) : PageViewModel(ApplicationPageNames.Actions)
{
    #region Actions Page (Methods)

    [RelayCommand]
    public void RefreshActionsPage(ActionsPageName actionsPageName)
    {
        switch (actionsPageName)
        {
            case ActionsPageName.Print: FetchPrintList(); break;
            case ActionsPageName.CustomProperties: FetchCustomPropertiesList(); break;
            case ActionsPageName.DrawingTemplates: FetchDrawingTemplateList(); break;
            case ActionsPageName.FileInfo: FetchFileInfoList(); break;
            case ActionsPageName.ImportFile: FetchImportFileList(); break;
            case ActionsPageName.Macros: FetchMacrosList(); break;
            case ActionsPageName.SaveDrawingAs: FetchSaveDrawingList(); break;
            case ActionsPageName.SaveModelAs: FetchSaveModelList(); break;
        }
    }

    #endregion

    #region Members

    #region Print

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(PrintListHasItems))]
    private ObservableCollection<ActionsTabPrintViewModel> _printList = [];

    public bool PrintListHasItems => PrintList.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedPrintListItem))]
    private string _selectedPrintListItemId = "";

    public ActionsTabPrintViewModel? SelectedPrintListItem =>
        PrintList.FirstOrDefault(f => f.Id == SelectedPrintListItemId);

    [ObservableProperty] private ObservableCollection<PrintSettingsViewModel> _printerSettings = [];

    #endregion

    #region Custom Properties

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CustomPropertiesListHasItems))]
    private ObservableCollection<ActionsTabCustomPropertiesViewModel> _customPropertiesList = [];

    public bool CustomPropertiesListHasItems => CustomPropertiesList.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedCustomPropertiesListItem))]
    private string _selectedCustomPropertiesListItemId = "";

    public ActionsTabCustomPropertiesViewModel? SelectedCustomPropertiesListItem =>
        CustomPropertiesList.FirstOrDefault(f => f.Id == SelectedCustomPropertiesListItemId);

    public ObservableCollection<CustomPropertiesRuleType> CustomPropertiesRuleTypes =>
        new(Enum.GetValues<CustomPropertiesRuleType>());

    public ObservableCollection<CustomPropertiesFieldTypes> CustomPropertiesFieldTypes =>
        new(Enum.GetValues<CustomPropertiesFieldTypes>());

    #endregion

    #region File Info

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FileInfoListHasItems))]
    private ObservableCollection<ActionsTabFileInfoViewModel> _fileInfoList = [];

    public bool FileInfoListHasItems => FileInfoList.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedFileInfoListItem))]
    private string _selectedFileInfoListItemId = "";

    public ActionsTabFileInfoViewModel? SelectedFileInfoListItem =>
        FileInfoList.FirstOrDefault(f => f.Id == SelectedFileInfoListItemId);

    #endregion

    #region Save Model

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SaveModelListHasItems))]
    private ObservableCollection<ActionsTabSaveModelViewModel> _saveModelList = [];

    public bool SaveModelListHasItems => SaveModelList.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedSaveModelListItem))]
    private string _selectedSaveModelListItemId = "";

    public ActionsTabSaveModelViewModel? SelectedSaveModelListItem =>
        SaveModelList.FirstOrDefault(f => f.Id == SelectedSaveModelListItemId);

    public ObservableCollection<string> SaveModelFormats =>
    [
        "Lib Feat Part (*.sldfp)",
        "Assembly file to Part (*.sldprt)",
        "Part Templates (*.prtdot)",
        "Assembly Templates (*.asmdot)",
        "Form Tool (*.sldftp)",
        "Parasolid (*.x_t)",
        "Parasolid Binary (*.x_b)",
        "DXF (*.dxf)",
        "DWG (*.dwg)",
        "IGES (*.igs)",
        "STEP (*.step)",
        "ACIS (*.sat)",
        "VDAFS (*.vda)",
        "VRML (*.wrl)",
        "STL (*.stl)",
        "eDrawings Part (*.eprt)",
        "eDrawings Assembly (*.easm)",
        "Adobe PDF (*.pdf)",
        "Universal 3D (*.u3d)",
        "3D XML (*.3dxml)",
        "Adobe Photoshop (*.psd)",
        "Adobe Illustrator (*.ai)",
        "Microsoft XAML (*.xaml)",
        "Catia Graphics (*.cgr)",
        "ProE Part (*.prt)",
        "ProE Assembly (*.asm)",
        "JPEG (*.jpg)",
        "HCG (*.hcg)",
        "HOOPS HSF (*.hsf)",
        "Tif (*.tif)"
    ];

    #endregion

    #region Save Drawing

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SaveDrawingListHasItems))]
    private ObservableCollection<ActionsTabSaveDrawingViewModel> _saveDrawingList = [];

    public bool SaveDrawingListHasItems => SaveDrawingList.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedSaveDrawingListItem))]
    private string _selectedSaveDrawingListItemId = "";

    public ActionsTabSaveDrawingViewModel? SelectedSaveDrawingListItem =>
        SaveDrawingList.FirstOrDefault(f => f.Id == SelectedSaveDrawingListItemId);

    public ObservableCollection<string> SaveDrawingFormats =>
    [
        "Detached Drawing (*.slddrw)",
        "DXF (*.dxf)",
        "DWG (*.dwg)",
        "Photoshop File (*.psd)",
        "Illustrator File (*.ai)",
        "PDF (*.pdf)",
        "eDrawing (*.edrw)",
        "JPEG (*.jpg)",
        "Tif (*.tif)"
    ];

    #endregion

    #region Import File

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ImportFileListHasItems))]
    private ObservableCollection<ActionsTabImportFileViewModel> _importFileList = [];

    public bool ImportFileListHasItems => ImportFileList.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedImportFileListItem))]
    private string _selectedImportFileListItemId = "";

    public ActionsTabImportFileViewModel? SelectedImportFileListItem =>
        ImportFileList.FirstOrDefault(f => f.Id == SelectedImportFileListItemId);

    #endregion

    #region Drawing Templates

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(DrawingTemplateListHasItems))]
    private ObservableCollection<ActionsTabDrawingTemplateViewModel> _drawingTemplateList = [];

    public bool DrawingTemplateListHasItems => DrawingTemplateList.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedDrawingTemplateListItem))]
    private string _selectedDrawingTemplateListItemId = "";

    public ActionsTabDrawingTemplateViewModel? SelectedDrawingTemplateListItem =>
        DrawingTemplateList.FirstOrDefault(f => f.Id == SelectedDrawingTemplateListItemId);

    #endregion

    #region Macros

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(MacrosListHasItems))]
    private ObservableCollection<ActionsTabMacrosViewModel> _macrosList = [];

    public bool MacrosListHasItems => MacrosList.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedMacrosListItem))]
    private string _selectedMacrosListItemId = "";

    public ActionsTabMacrosViewModel? SelectedMacrosListItem =>
        MacrosList.FirstOrDefault(f => f.Id == SelectedMacrosListItemId);

    #endregion

    #endregion

    #region Constructor

    // Design time only
    public ActionsPageViewModel() : this(new MainViewModel(), new DialogService(() => null), new PrinterService(),
        new DatabaseService(new ApplicationDbContext()))
    {
    }

    protected override void OnDesignTimeConstructor()
    {
        FetchPrintList();
        FetchCustomPropertiesList();
        FetchFileInfoList();
        FetchSaveModelList();
        FetchSaveDrawingList();
        FetchImportFileList();
        FetchDrawingTemplateList();
        FetchMacrosList();
    }

    #endregion

    #region Print (Methods)

    [RelayCommand]
    private void FetchPrintList()
    {
        FetchPrintSettings();

        var list = databaseService.GetPrintList();

        PrintList = new ObservableCollection<ActionsTabPrintViewModel>(list
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel()));

        // Update PrintListHasItems when collection changes
        PrintList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintListHasItems));

        if (PrintList.Count <= 0) return;

        // Select first item
        SelectedPrintListItemId = PrintList.First().Id;

        // Store last fetched database save states
        foreach (var printItem in PrintList)
            printItem.SetSavedState();
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
    private async Task CancelPrintItem()
    {
        // Ignore if nothing is selected
        if (SelectedPrintListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedPrintListItem.IsNewItem)
            await DeletePrintItemFromUIAsync(SelectedPrintListItem.Id, false);
        else
            SelectedPrintListItem.RestoreState();
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
                Title = "Delete Print Item?",
                Message = $"Are you sure you want to delete ' {PrintList[index].JobName}'?",
                DialogWidth = 500
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
    private void FetchPrintSettings()
    {
        var settings = databaseService.GetPrintSettings();

        PrinterSettings = settings.ToViewModels();
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
    private async Task AddNewPrintSettingsAsync()
    {
        var confirmViewModel = new PrintSettingsViewModel
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
                Title = "Delete Print Profile?",
                Message = $"Are you sure you want to delete '{PrinterSettings[index].Name}'?",
                DialogWidth = 500
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

        if (SelectedPrintListItem != null && PrinterSettings.Count > 0)
        {
            SelectedPrintListItem.PrinterSettingsId = PrinterSettings[index].Id;
            await SavePrintItemAsync();
        }

        return true;
    }

    private void InjectPrinterDetails(PrintSettingsViewModel viewModel)
    {
        // Fetch live printers available on machine
        var availablePrinters = printerService.AvailablePrinters();

        var printerNameOptions = new ObservableCollection<string>(
            availablePrinters.Select(f => f.Name)
        );

        foreach (var printerSettingsItem in viewModel.PrinterSettingProfiles)
        {
            printerSettingsItem.PrinterNameOptions = printerNameOptions;

            printerSettingsItem.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(PrintSettingsProfileViewModel.PrinterName))
                    return;

                // Printer changed, update paper size and tray
                printerSettingsItem.PaperSizeOptions = new ObservableCollection<string>(
                    availablePrinters.FirstOrDefault(f => f.Name == printerSettingsItem.PrinterName)?.PaperSizes ?? []
                );

                printerSettingsItem.PaperSizeOptions.Insert(0, "(Default)");

                printerSettingsItem.SourceTrayOptions = new ObservableCollection<string>(
                    availablePrinters.FirstOrDefault(f => f.Name == printerSettingsItem.PrinterName)?.SourceTrays ?? []
                );

                printerSettingsItem.SourceTrayOptions.Insert(0, "(Default)");

                // Change paper size and source tray to first item
                if (!printerSettingsItem.PaperSizeOptions.Any(f => f == printerSettingsItem.PaperSize))
                    printerSettingsItem.PaperSize = printerSettingsItem.PaperSizeOptions.FirstOrDefault();

                if (!printerSettingsItem.SourceTrayOptions.Any(f => f == printerSettingsItem.SourceTray))
                    printerSettingsItem.SourceTray = printerSettingsItem.SourceTrayOptions.FirstOrDefault();
            };

            // Force a printer name change for initial list
            printerSettingsItem.RaiseOnPropertyChanged(nameof(printerSettingsItem.PrinterName));
        }
    }

    #endregion

    #region Custom Properties (Methods)

    [RelayCommand]
    private void FetchCustomPropertiesList()
    {
        var list = databaseService.GetCustomPropertiesList();

        CustomPropertiesList = new ObservableCollection<ActionsTabCustomPropertiesViewModel>(list
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel()));

        // Update CustomPropertiesListHasItems when collection changes
        CustomPropertiesList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CustomPropertiesListHasItems));

        if (CustomPropertiesList.Count <= 0) return;

        // Select first item
        SelectedCustomPropertiesListItemId = CustomPropertiesList.First().Id;

        // Store last fetched database save states
        foreach (var listItem in CustomPropertiesList)
            listItem.SetSavedState();
    }

    [RelayCommand]
    private void AddNewCustomPropertiesItem()
    {
        // Create a new item
        var newItem = new ActionsTabCustomPropertiesViewModel
        {
            Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New Custom Property Job"
        };

        // Add to the print list
        CustomPropertiesList.Add(newItem);

        // Select item
        SelectedCustomPropertiesListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task CancelCustomPropertiesItem()
    {
        // Ignore if nothing is selected
        if (SelectedCustomPropertiesListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedCustomPropertiesListItem.IsNewItem)
            await DeleteCustomPropertiesItemFromUIAsync(SelectedCustomPropertiesListItem.Id, false);
        else
            SelectedCustomPropertiesListItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteCustomPropertiesItemAsync(string id)
    {
        if (CustomPropertiesList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeleteCustomPropertiesItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteCustomPropertiesListItem(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteCustomPropertiesItemFromUIAsync(string id, bool warn = true)
    {
        var index = CustomPropertiesList.IndexOf(CustomPropertiesList.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = "Delete Custom Properties Item?",
                Message = $"Are you sure you want to delete ' {CustomPropertiesList[index].JobName}'?",
                DialogWidth = 500
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        CustomPropertiesList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (CustomPropertiesList.Count > 0)
            SelectedCustomPropertiesListItemId = CustomPropertiesList[index].Id;

        return true;
    }

    [RelayCommand]
    private async Task SaveCustomPropertiesItemAsync()
    {
        // Ignore if no selection
        if (SelectedCustomPropertiesListItem == null)
            return;

        // If the selected item is new...
        if (SelectedCustomPropertiesListItem.IsNewItem)
            databaseService.AddCustomPropertiesItem(SelectedCustomPropertiesListItem.ToDataModel());
        else
            databaseService.UpdateCustomPropertiesItem(SelectedCustomPropertiesListItem.ToDataModel());

        // Flag new item as not new
        SelectedCustomPropertiesListItem.IsNewItem = false;
        SelectedCustomPropertiesListItem.SetSavedState();
    }

    #endregion

    #region File Info (Methods)

    [RelayCommand]
    private void FetchFileInfoList()
    {
        var list = databaseService.GetFileInfoList();

        FileInfoList = new ObservableCollection<ActionsTabFileInfoViewModel>(list
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel()));

        // Update FileInfoListHasItems when collection changes
        FileInfoList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(FileInfoListHasItems));

        if (FileInfoList.Count <= 0) return;

        // Select first item
        SelectedFileInfoListItemId = FileInfoList.First().Id;

        // Store last fetched database save states
        foreach (var listItem in FileInfoList)
            listItem.SetSavedState();
    }

    [RelayCommand]
    private void AddNewFileInfoItem()
    {
        // Create a new item
        var newItem = new ActionsTabFileInfoViewModel
        {
            Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New File Info Job"
        };

        // Add to the print list
        FileInfoList.Add(newItem);

        // Select item
        SelectedFileInfoListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task CancelFileInfoItem()
    {
        // Ignore if nothing is selected
        if (SelectedFileInfoListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedFileInfoListItem.IsNewItem)
            await DeleteFileInfoItemFromUIAsync(SelectedFileInfoListItem.Id, false);
        else
            SelectedFileInfoListItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteFileInfoItemAsync(string id)
    {
        if (FileInfoList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeleteFileInfoItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteFileInfoListItem(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteFileInfoItemFromUIAsync(string id, bool warn = true)
    {
        var index = FileInfoList.IndexOf(FileInfoList.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = "Delete File Info Item?",
                Message = $"Are you sure you want to delete ' {FileInfoList[index].JobName}'?",
                DialogWidth = 500
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        FileInfoList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (FileInfoList.Count > 0)
            SelectedFileInfoListItemId = FileInfoList[index].Id;

        return true;
    }

    [RelayCommand]
    private async Task SaveFileInfoItemAsync()
    {
        // Ignore if no selection
        if (SelectedFileInfoListItem == null)
            return;

        // If the selected item is new...
        if (SelectedFileInfoListItem.IsNewItem)
            databaseService.AddFileInfoItem(SelectedFileInfoListItem.ToDataModel());
        else
            databaseService.UpdateFileInfoItem(SelectedFileInfoListItem.ToDataModel());

        // Flag new item as not new
        SelectedFileInfoListItem.IsNewItem = false;
        SelectedFileInfoListItem.SetSavedState();
    }

    #endregion

    #region Save Model (Methods)

    [RelayCommand]
    private void FetchSaveModelList()
    {
        var list = databaseService.GetSaveModelList();

        SaveModelList = new ObservableCollection<ActionsTabSaveModelViewModel>(list
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel()));

        // Update SaveModelListHasItems when collection changes
        SaveModelList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SaveModelListHasItems));

        if (SaveModelList.Count <= 0) return;

        // Select first item
        SelectedSaveModelListItemId = SaveModelList.First().Id;

        // Store last fetched database save states
        foreach (var listItem in SaveModelList)
            listItem.SetSavedState();
    }

    [RelayCommand]
    private void AddNewSaveModelItem()
    {
        // Create a new item
        var newItem = new ActionsTabSaveModelViewModel
        {
            Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New Save Model Job"
        };

        // Add to the print list
        SaveModelList.Add(newItem);

        // Select item
        SelectedSaveModelListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task CancelSaveModelItem()
    {
        // Ignore if nothing is selected
        if (SelectedSaveModelListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedSaveModelListItem.IsNewItem)
            await DeleteSaveModelItemFromUIAsync(SelectedSaveModelListItem.Id, false);
        else
            SelectedSaveModelListItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteSaveModelItemAsync(string id)
    {
        if (SaveModelList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeleteSaveModelItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteSaveModelListItem(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteSaveModelItemFromUIAsync(string id, bool warn = true)
    {
        var index = SaveModelList.IndexOf(SaveModelList.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = "Delete Save Model Item?",
                Message = $"Are you sure you want to delete ' {SaveModelList[index].JobName}'?",
                DialogWidth = 500
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        SaveModelList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (SaveModelList.Count > 0)
            SelectedSaveModelListItemId = SaveModelList[index].Id;

        return true;
    }

    [RelayCommand]
    private async Task SaveSaveModelItemAsync()
    {
        // Ignore if no selection
        if (SelectedSaveModelListItem == null)
            return;

        // If the selected item is new...
        if (SelectedSaveModelListItem.IsNewItem)
            databaseService.AddSaveModelItem(SelectedSaveModelListItem.ToDataModel());
        else
            databaseService.UpdateSaveModelItem(SelectedSaveModelListItem.ToDataModel());

        // Flag new item as not new
        SelectedSaveModelListItem.IsNewItem = false;
        SelectedSaveModelListItem.SetSavedState();
    }

    #endregion

    #region Save Drawing (Methods)

    [RelayCommand]
    private void FetchSaveDrawingList()
    {
        var list = databaseService.GetSaveDrawingList();

        SaveDrawingList = new ObservableCollection<ActionsTabSaveDrawingViewModel>(list
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel(SaveDrawingFormats)));

        // Update SaveDrawingListHasItems when collection changes
        SaveDrawingList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SaveDrawingListHasItems));

        if (SaveDrawingList.Count <= 0) return;

        // Select first item
        SelectedSaveDrawingListItemId = SaveDrawingList.First().Id;

        // Store last fetched database save states
        foreach (var listItem in SaveDrawingList)
            listItem.SetSavedState();
    }

    [RelayCommand]
    private void AddNewSaveDrawingItem()
    {
        // Create a new item
        var newItem = new ActionsTabSaveDrawingViewModel
        {
            Id = Guid.NewGuid().ToString("N"),
            IsNewItem = true,
            JobName = "New Save Drawing Job",
            ExportFormats =
                new ObservableCollection<KeyValueViewModel<string, bool>>(
                    SaveDrawingFormats.Select(f => new KeyValueViewModel<string, bool>(f, false)))
        };

        // Add to the print list
        SaveDrawingList.Add(newItem);

        // Select item
        SelectedSaveDrawingListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task CancelSaveDrawingItem()
    {
        // Ignore if nothing is selected
        if (SelectedSaveDrawingListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedSaveDrawingListItem.IsNewItem)
            await DeleteSaveDrawingItemFromUIAsync(SelectedSaveDrawingListItem.Id, false);
        else
            SelectedSaveDrawingListItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteSaveDrawingItemAsync(string id)
    {
        if (SaveDrawingList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeleteSaveDrawingItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteSaveDrawingListItem(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteSaveDrawingItemFromUIAsync(string id, bool warn = true)
    {
        var index = SaveDrawingList.IndexOf(SaveDrawingList.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = "Delete Save Drawing Item?",
                Message = $"Are you sure you want to delete ' {SaveDrawingList[index].JobName}'?",
                DialogWidth = 500
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        SaveDrawingList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (SaveDrawingList.Count > 0)
            SelectedSaveDrawingListItemId = SaveDrawingList[index].Id;

        return true;
    }

    [RelayCommand]
    private async Task SaveSaveDrawingItemAsync()
    {
        // Ignore if no selection
        if (SelectedSaveDrawingListItem == null)
            return;

        // If the selected item is new...
        if (SelectedSaveDrawingListItem.IsNewItem)
            databaseService.AddSaveDrawingItem(SelectedSaveDrawingListItem.ToDataModel());
        else
            databaseService.UpdateSaveDrawingItem(SelectedSaveDrawingListItem.ToDataModel());

        // Flag new item as not new
        SelectedSaveDrawingListItem.IsNewItem = false;
        SelectedSaveDrawingListItem.SetSavedState();
    }

    #endregion

    #region Import File (Methods)

    [RelayCommand]
    private void FetchImportFileList()
    {
        var list = databaseService.GetImportFileList();

        ImportFileList = new ObservableCollection<ActionsTabImportFileViewModel>(list
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel()));

        // Update ImportFileListHasItems when collection changes
        ImportFileList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ImportFileListHasItems));

        if (ImportFileList.Count <= 0) return;

        // Select first item
        SelectedImportFileListItemId = ImportFileList.First().Id;

        // Store last fetched database save states
        foreach (var listItem in ImportFileList)
            listItem.SetSavedState();
    }

    [RelayCommand]
    private void AddNewImportFileItem()
    {
        // Create a new item
        var newItem = new ActionsTabImportFileViewModel
        {
            Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New Import File Job"
        };

        // Add to the print list
        ImportFileList.Add(newItem);

        // Select item
        SelectedImportFileListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task CancelImportFileItem()
    {
        // Ignore if nothing is selected
        if (SelectedImportFileListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedImportFileListItem.IsNewItem)
            await DeleteImportFileItemFromUIAsync(SelectedImportFileListItem.Id, false);
        else
            SelectedImportFileListItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteImportFileItemAsync(string id)
    {
        if (ImportFileList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeleteImportFileItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteImportFileListItem(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteImportFileItemFromUIAsync(string id, bool warn = true)
    {
        var index = ImportFileList.IndexOf(ImportFileList.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = "Delete Import File Item?",
                Message = $"Are you sure you want to delete ' {ImportFileList[index].JobName}'?",
                DialogWidth = 500
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        ImportFileList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (ImportFileList.Count > 0)
            SelectedImportFileListItemId = ImportFileList[index].Id;

        return true;
    }

    [RelayCommand]
    private async Task SaveImportFileItemAsync()
    {
        // Ignore if no selection
        if (SelectedImportFileListItem == null)
            return;

        // If the selected item is new...
        if (SelectedImportFileListItem.IsNewItem)
            databaseService.AddImportFileItem(SelectedImportFileListItem.ToDataModel());
        else
            databaseService.UpdateImportFileItem(SelectedImportFileListItem.ToDataModel());

        // Flag new item as not new
        SelectedImportFileListItem.IsNewItem = false;
        SelectedImportFileListItem.SetSavedState();
    }

    #endregion

    #region Drawing Template (Methods)

    [RelayCommand]
    private void FetchDrawingTemplateList()
    {
        var list = databaseService.GetDrawingTemplateList();

        DrawingTemplateList = new ObservableCollection<ActionsTabDrawingTemplateViewModel>(list
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel()));

        // Update DrawingTemplateListHasItems when collection changes
        DrawingTemplateList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(DrawingTemplateListHasItems));

        if (DrawingTemplateList.Count <= 0) return;

        // Select first item
        SelectedDrawingTemplateListItemId = DrawingTemplateList.First().Id;

        // Store last fetched database save states
        foreach (var listItem in DrawingTemplateList)
            listItem.SetSavedState();
    }

    [RelayCommand]
    private void AddNewDrawingTemplateItem()
    {
        // Create a new item
        var newItem = new ActionsTabDrawingTemplateViewModel
        {
            Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New Drawing Template Job"
        };

        // Add to the print list
        DrawingTemplateList.Add(newItem);

        // Select item
        SelectedDrawingTemplateListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task CancelDrawingTemplateItem()
    {
        // Ignore if nothing is selected
        if (SelectedDrawingTemplateListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedDrawingTemplateListItem.IsNewItem)
            await DeleteDrawingTemplateItemFromUIAsync(SelectedDrawingTemplateListItem.Id, false);
        else
            SelectedDrawingTemplateListItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteDrawingTemplateItemAsync(string id)
    {
        if (DrawingTemplateList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeleteDrawingTemplateItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteDrawingTemplateListItem(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteDrawingTemplateItemFromUIAsync(string id, bool warn = true)
    {
        var index = DrawingTemplateList.IndexOf(DrawingTemplateList.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = "Delete Drawing Template Item?",
                Message = $"Are you sure you want to delete ' {DrawingTemplateList[index].JobName}'?",
                DialogWidth = 500
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        DrawingTemplateList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (DrawingTemplateList.Count > 0)
            SelectedDrawingTemplateListItemId = DrawingTemplateList[index].Id;

        return true;
    }

    [RelayCommand]
    private async Task SaveDrawingTemplateItemAsync()
    {
        // Ignore if no selection
        if (SelectedDrawingTemplateListItem == null)
            return;

        // If the selected item is new...
        if (SelectedDrawingTemplateListItem.IsNewItem)
            databaseService.AddDrawingTemplateItem(SelectedDrawingTemplateListItem.ToDataModel());
        else
            databaseService.UpdateDrawingTemplateItem(SelectedDrawingTemplateListItem.ToDataModel());

        // Flag new item as not new
        SelectedDrawingTemplateListItem.IsNewItem = false;
        SelectedDrawingTemplateListItem.SetSavedState();
    }

    #endregion

    #region Macros (Methods)

    [RelayCommand]
    private void FetchMacrosList()
    {
        var list = databaseService.GetMacrosList();

        MacrosList = new ObservableCollection<ActionsTabMacrosViewModel>(list
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel()));

        // Update MacrosListHasItems when collection changes
        MacrosList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(MacrosListHasItems));

        if (MacrosList.Count <= 0) return;

        // Select first item
        SelectedMacrosListItemId = MacrosList.First().Id;

        // Store last fetched database save states
        foreach (var listItem in MacrosList)
            listItem.SetSavedState();
    }

    [RelayCommand]
    private void AddNewMacrosItem()
    {
        // Create a new item
        var newItem = new ActionsTabMacrosViewModel
        {
            Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New Macro Job"
        };

        // Add to the print list
        MacrosList.Add(newItem);

        // Select item
        SelectedMacrosListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task CancelMacrosItem()
    {
        // Ignore if nothing is selected
        if (SelectedMacrosListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedMacrosListItem.IsNewItem)
            await DeleteMacrosItemFromUIAsync(SelectedMacrosListItem.Id, false);
        else
            SelectedMacrosListItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteMacrosItemAsync(string id)
    {
        if (MacrosList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeleteMacrosItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteMacrosListItem(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteMacrosItemFromUIAsync(string id, bool warn = true)
    {
        var index = MacrosList.IndexOf(MacrosList.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = "Delete Macro Item?",
                Message = $"Are you sure you want to delete ' {MacrosList[index].JobName}'?",
                DialogWidth = 500
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        MacrosList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (MacrosList.Count > 0)
            SelectedMacrosListItemId = MacrosList[index].Id;

        return true;
    }

    [RelayCommand]
    private async Task SaveMacrosItemAsync()
    {
        // Ignore if no selection
        if (SelectedMacrosListItem == null)
            return;

        // If the selected item is new...
        if (SelectedMacrosListItem.IsNewItem)
            databaseService.AddMacrosItem(SelectedMacrosListItem.ToDataModel());
        else
            databaseService.UpdateMacrosItem(SelectedMacrosListItem.ToDataModel());

        // Flag new item as not new
        SelectedMacrosListItem.IsNewItem = false;
        SelectedMacrosListItem.SetSavedState();
    }

    #endregion
}