using Avalonia.Platform.Storage;
using BatchProcess3.CustomProperties;
using BatchProcess3.DataStorage;
using BatchProcess3.Dialog;
using BatchProcess3.DrawingTemplates;
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
    #region Members

    [ObservableProperty] private ObservableCollection<ActionPrintSettingsViewModel> _printerSettings = [];
    
    public ObservableCollection<CustomPropertiesRuleType> CustomPropertiesRuleTypes =>
        new(Enum.GetValues<CustomPropertiesRuleType>());

    public ObservableCollection<CustomPropertiesFieldTypes> CustomPropertiesFieldTypes =>
        new(Enum.GetValues<CustomPropertiesFieldTypes>());

    public static ObservableCollection<string> SaveModelFormats =>
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
    
    public static ObservableCollection<string> SaveDrawingFormats =>
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
    
    public ObservableCollection<DrawingTemplateOperation> DrawingTemplateOperations => new(Enum.GetValues<DrawingTemplateOperation>());

    [ObservableProperty]
    private ObservableCollection<string> _drawingTemplateSelectedPaths = [];

    public ObservableCollection<string> DrawingTemplatePaths => new(databaseService.GetSettings().DrawingTemplatePaths);

    #region Selectable Lists

    [ObservableProperty] private SelectableItemListViewModel<ActionPrintViewModel> _printList = new
    (
        title: "Print",
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        getList: () =>
        {
            var list = databaseService.GetPrintList();

            return new ObservableCollection<ActionPrintViewModel>(list
                .OrderBy(f => f.JobName)
                .Select(f => f.ToViewModel()));
        },
        createItem: () =>
        {
            // Fetch printer settings
            var printerSettings = databaseService.GetPrintSettings();
            
            return new ActionPrintViewModel
            {
                Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New Print Job",
                PrinterSettingsId = printerSettings.FirstOrDefault()?.Id
            };
        },
        deleteItem: databaseService.DeletePrintListItem,
        addItem: (item) => databaseService.AddPrintListItem(item.ToDataModel()),
        updateItem: (item) => databaseService.UpdatePrintListItem(item.ToDataModel())
    );
    
    [ObservableProperty] private SelectableItemListViewModel<ActionCustomPropertiesViewModel> _customPropertiesList = new
    (
        title: "Custom Properties",
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        getList: () =>
        {
            var list = databaseService.GetCustomPropertiesList();

            return new ObservableCollection<ActionCustomPropertiesViewModel>(list
                .OrderBy(f => f.JobName)
                .Select(f => f.ToViewModel()));
        },
        createItem: () => new ActionCustomPropertiesViewModel
        {
            Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New Custom Properties Job"
        },
        deleteItem: databaseService.DeleteCustomPropertiesListItem,
        addItem: (item) => databaseService.AddCustomPropertiesItem(item.ToDataModel()),
        updateItem: (item) => databaseService.UpdateCustomPropertiesItem(item.ToDataModel())
    );

    [ObservableProperty]
    private SelectableItemListViewModel<ActionFileInfoViewModel> _fileInfoList = new
    (
        title: "File Info",
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        getList: () =>
        {
            var list = databaseService.GetFileInfoList();

            return new ObservableCollection<ActionFileInfoViewModel>(list
                .OrderBy(f => f.JobName)
                .Select(f => f.ToViewModel()));
        },
        createItem: () => new ActionFileInfoViewModel
        {
            Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New File Info Job"
        },
        deleteItem: databaseService.DeleteFileInfoListItem,
        addItem: (item) => databaseService.AddFileInfoItem(item.ToDataModel()),
        updateItem: (item) => databaseService.UpdateFileInfoItem(item.ToDataModel())
    );
    
    [ObservableProperty]
    private SelectableItemListViewModel<ActionSaveModelViewModel> _saveModelList = new
    (
        title: "Save Model",
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        getList: () =>
        {
            var list = databaseService.GetSaveModelList();

            return new ObservableCollection<ActionSaveModelViewModel>(list
                .OrderBy(f => f.JobName)
                .Select(f => f.ToViewModel(exportFormats: SaveModelFormats)));
        },
        createItem: () => new ActionSaveModelViewModel
        {
            Id = Guid.NewGuid().ToString("N"), 
            IsNewItem = true, 
            JobName = "New Save Model Job",
            ExportFormats =
                new ObservableCollection<KeyValueViewModel<string, bool>>(
                    SaveModelFormats.Select(f => new KeyValueViewModel<string, bool>(f, false)))
        },
        deleteItem: databaseService.DeleteSaveModelListItem,
        addItem: (item) => databaseService.AddSaveModelItem(item.ToDataModel()),
        updateItem: (item) => databaseService.UpdateSaveModelItem(item.ToDataModel())
    );
    
    [ObservableProperty]
    private SelectableItemListViewModel<ActionSaveDrawingViewModel> _saveDrawingList = new
    (
        title: "Save Drawing",
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        getList: () =>
        {
            var list = databaseService.GetSaveDrawingList();

            return new ObservableCollection<ActionSaveDrawingViewModel>(list
                .OrderBy(f => f.JobName)
                .Select(f => f.ToViewModel(exportFormats: SaveDrawingFormats)));
        },
        createItem: () => new ActionSaveDrawingViewModel
        {
            Id = Guid.NewGuid().ToString("N"), 
            IsNewItem = true, 
            JobName = "New Save Drawing Job",
            ExportFormats =
                new ObservableCollection<KeyValueViewModel<string, bool>>(
                    SaveDrawingFormats.Select(f => new KeyValueViewModel<string, bool>(f, false)))
        },
        deleteItem: databaseService.DeleteSaveDrawingListItem,
        addItem: (item) => databaseService.AddSaveDrawingItem(item.ToDataModel()),
        updateItem: (item) => databaseService.UpdateSaveDrawingItem(item.ToDataModel())
    );
    
    [ObservableProperty]
    private SelectableItemListViewModel<ActionImportFileViewModel> _importFileList = new
    (
        title: "Import File",
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        getList: () =>
        {
            var list = databaseService.GetImportFileList();

            return new ObservableCollection<ActionImportFileViewModel>(list
                .OrderBy(f => f.JobName)
                .Select(f => f.ToViewModel()));
        },
        createItem: () => new ActionImportFileViewModel
        {
            Id = Guid.NewGuid().ToString("N"), 
            IsNewItem = true, 
            JobName = "New Import File Job"
        },
        deleteItem: databaseService.DeleteImportFileListItem,
        addItem: (item) => databaseService.AddImportFileItem(item.ToDataModel()),
        updateItem: (item) => databaseService.UpdateImportFileItem(item.ToDataModel())
    );

    [ObservableProperty] private SelectableItemListViewModel<ActionDrawingTemplateViewModel> _drawingTemplateList = new
    (
        title: "Drawing Template",
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        getList: () =>
        {
            var list = databaseService.GetDrawingTemplateList();

            return new ObservableCollection<ActionDrawingTemplateViewModel>(list
                .OrderBy(f => f.JobName)
                .Select(f => f.ToViewModel()));
        },
        createItem: () => new ActionDrawingTemplateViewModel
        {
            Id = Guid.NewGuid().ToString("N"), 
            IsNewItem = true, 
            JobName = "New Drawing Template Job"
        },
        deleteItem: databaseService.DeleteDrawingTemplateListItem,
        addItem: (item) => databaseService.AddDrawingTemplateItem(item.ToDataModel()),
        updateItem: (item) => databaseService.UpdateDrawingTemplateItem(item.ToDataModel())
    );
    
    [ObservableProperty] private SelectableItemListViewModel<ActionMacrosViewModel> _macrosList = new
    (
        title: "Macros",
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        getList: () =>
        {
            var list = databaseService.GetMacrosList();

            return new ObservableCollection<ActionMacrosViewModel>(list
                .OrderBy(f => f.JobName)
                .Select(f => f.ToViewModel()));
        },
        createItem: () => new ActionMacrosViewModel
        {
            Id = Guid.NewGuid().ToString("N"), 
            IsNewItem = true, 
            JobName = "New Macros Job"
        },
        deleteItem: databaseService.DeleteMacrosListItem,
        addItem: (item) => databaseService.AddMacrosItem(item.ToDataModel()),
        updateItem: (item) => databaseService.UpdateMacrosItem(item.ToDataModel())
    );
    #endregion

    #endregion

    #region Constructor

    protected override void OnDesignTimeConstructor()
    {
        PrintList.FetchList();
        CustomPropertiesList.FetchList();
        FileInfoList.FetchList();
        SaveModelList.FetchList();
        SaveDrawingList.FetchList();
        ImportFileList.FetchList();
        DrawingTemplateList.FetchList();
        MacrosList.FetchList();
    }
    
    #endregion
    
    #region Actions Page (Methods)

    [RelayCommand]
    public void RefreshActionsPage(ActionsPageName actionsPageName)
    {
        switch (actionsPageName)
        {
            case ActionsPageName.Print: PrintList.FetchList(); break;
            case ActionsPageName.CustomProperties: CustomPropertiesList.FetchList(); break;
            case ActionsPageName.DrawingTemplates: DrawingTemplateList.FetchList(); break;
            case ActionsPageName.FileInfo: FileInfoList.FetchList(); break;
            case ActionsPageName.ImportFile: ImportFileList.FetchList(); break;
            case ActionsPageName.Macros: MacrosList.FetchList(); break;
            case ActionsPageName.SaveDrawingAs: SaveDrawingList.FetchList(); break;
            case ActionsPageName.SaveModelAs: SaveModelList.FetchList(); break;
        }
    }

    #endregion

    #region Printer Settings (Methods)
    
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
        var copiedProfileViewModel = new ActionPrintSettingsViewModel();
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
        var confirmViewModel = new ActionPrintSettingsViewModel
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

        if (PrintList.SelectedItem != null && PrinterSettings.Count > 0)
        {
            PrintList.SelectedItem.PrinterSettingsId = PrinterSettings[index].Id;
            await PrintList.SaveItemAsync();
        }

        return true;
    }

    private void InjectPrinterDetails(ActionPrintSettingsViewModel viewModel)
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
                if (args.PropertyName != nameof(ActionPrintSettingsProfileViewModel.PrinterName))
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
                if (printerSettingsItem.PaperSizeOptions.All(f => f != printerSettingsItem.PaperSize))
                    printerSettingsItem.PaperSize = printerSettingsItem.PaperSizeOptions.FirstOrDefault() ?? "";

                if (printerSettingsItem.SourceTrayOptions.All(f => f != printerSettingsItem.SourceTray))
                    printerSettingsItem.SourceTray = printerSettingsItem.SourceTrayOptions.FirstOrDefault() ?? "";
            };

            // Force a printer name change for initial list
            printerSettingsItem.RaiseOnPropertyChanged(nameof(printerSettingsItem.PrinterName));
        }
    }

    #endregion
    
    #region Drawing Template (Methods)

    [RelayCommand]
    private async Task AddDrawingTemplatePaths()
    {
        var paths = await dialogService.FilePicker(
            title: "Select a drawing template", 
            allowMultiple: true,
            fileTypes: [
                new FilePickerFileType("Drawing Template") { Patterns = ["*.slddrt"] }
            ]);

        // Add to database
        databaseService.AddDrawingTemplatePaths(paths);
        
        // Let the UI know the paths have changed
        RaiseOnPropertyChanged(nameof(DrawingTemplatePaths));
    }

    [RelayCommand]
    private void DeleteDrawingTemplatePaths()
    {
        // Ignore empty list
        if (DrawingTemplateSelectedPaths.Count == 0)
            return;

        // Delete from database
        databaseService.DeleteDrawingTemplatePaths(DrawingTemplateSelectedPaths.ToArray());
        
        // Let the UI know the paths have changed
        RaiseOnPropertyChanged(nameof(DrawingTemplatePaths));
    }

    #endregion
}