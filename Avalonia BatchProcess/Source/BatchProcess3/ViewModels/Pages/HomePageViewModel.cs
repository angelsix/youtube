using BatchProcess3.Actions;
using BatchProcess3.Core.SolidWorks;
using BatchProcess3.DataStorage;
using BatchProcess3.Dialog;
using BatchProcess3.MainApp;
using BatchProcess3.SolidWorks;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels.Pages;

public partial class HomePageViewModel(
    MainViewModel mainViewModel,
    DialogService dialogService,
    DatabaseFactory databaseFactory,
    BatchProcessClient batchProcessClient,
    ActionService actionService) : PageViewModel(ApplicationPageNames.Home)
{
    #region Properties

    [ObservableProperty] private ObservableCollection<ProcessViewModel> _processList = [];
    
    [ObservableProperty] private ObservableCollection<AvailableActionItemViewModel> _availableActionsList = [];
    
    [ObservableProperty] private ObservableCollection<SolidWorksFileDetails> _solidWorksFileList = [];

    [ObservableProperty] private SolidWorksFileDetails? _selectedSolidWorksFile;

    private ObservableCollection<ProcessActionViewModel> _actions = [];
    
    public ObservableCollection<ProcessActionViewModel> Actions
    {
        get => _actions;
        set => this.SetAndObserveEverything(value, ref _actions, [nameof(HasChanged)]);
    }

    [ObservableProperty]
    private bool _quickView = false;

    [ObservableProperty]
    private bool _saveOnClose = true;
    
    #endregion Properties

    #region Constructor

    [RelayCommand]
    private async Task InitializeAsync()
    {
        AvailableActionsList = actionService.GetAvailableActionsList();

        using var dbContext = databaseFactory.GetDatabaseService();
        ProcessList = new ObservableCollection<ProcessViewModel>(dbContext.GetProcessList()
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel()));
        
        // Get SolidWorks file list from remote host
        // batchProcessClient.Connect(dbContext.GetSettings().SolidWorksHost);
        batchProcessClient.Connect("http://localhost:5000");
        await SetSolidWorksFileList();
    }

    private async Task SetSolidWorksFileList()
    {
        SolidWorksFileList = new ObservableCollection<SolidWorksFileDetails>(
            (await batchProcessClient.GetActiveFileReferencesAsync())
            .OrderByDescending(f => f.IsActiveInSolidWorks)
            .ThenBy(f => f.FileName)
            );
    }

    #endregion
    
    #region Commands
    
    public void InsertAction(AvailableActionItemViewModel item, int index)
    {
        if (item.ActionViewModel == null) return;
        
        var copy = new AvailableActionItemViewModel();
        copy.RestoreState(item.GetState());

        // Give the copy a new unique ID
        copy.ActionViewModel!.Id = Guid.NewGuid().ToString("N");
        
        if (index <= -1 || index > Actions.Count || Actions.Count == 0)
            Actions.Add(copy.ActionViewModel!);
        else
            Actions.Insert(index, copy.ActionViewModel!);
        
        // Update sort order
        UpdateActionSortOrder();
    }

    [RelayCommand]
    private void UpdateActionSortOrder()
    {
        foreach (var (action, index) in Actions.Select((f, i) => (f, i)))
            // Sort order should match position in list
            action.SortOrder = index;
    }

    [RelayCommand]
    private void DeleteAction(ProcessActionViewModel item) => Actions.Remove(item);

    [RelayCommand]
    private void OpenFile()
    {
        if (SelectedSolidWorksFile is null) return;

        Process.Start(new ProcessStartInfo
        {
            FileName = SelectedSolidWorksFile.FilePath,
            UseShellExecute = true,
        });
    }

    [RelayCommand]
    private void OpenFolder()
    {
        if (SelectedSolidWorksFile is null) return;

        var filePath = SelectedSolidWorksFile.FilePath;

        if (OperatingSystem.IsWindows())
            Process.Start("explorer", $"/select,\"{filePath}\"");
        else if (OperatingSystem.IsMacOS())
            Process.Start("open", $"-R \"{filePath}\"");
    }

    [RelayCommand]
    private void FindDrawing() 
    { 
        if (SelectedSolidWorksFile is null) return;
        
        var drawingPath = Path.ChangeExtension(SelectedSolidWorksFile.FilePath, ".slddrw");

        if (!File.Exists(drawingPath)) return;

        // Fix case
        drawingPath = Directory.GetFiles(Path.GetDirectoryName(drawingPath)!, Path.GetFileName(drawingPath)).First();

        // Don't add if already in the list
        if (SolidWorksFileList.Any(f => string.Equals(f.FilePath, drawingPath, StringComparison.OrdinalIgnoreCase)))
            return;
        
        SolidWorksFileList.Add(new SolidWorksFileDetails(drawingPath));
    }

    [RelayCommand]
    private async Task DeleteFileAsync()
    {
        if (SelectedSolidWorksFile is null) return;

        var confirmViewModel = new ConfirmDialogViewModel
        {
            Title = "Delete File",
            Message = $"Are you sure you want to remove '{SelectedSolidWorksFile.FileName}' from the list?",
            DialogWidth = 400
        };

        await dialogService.ShowDialog(mainViewModel, confirmViewModel);

        if (!confirmViewModel.Confirmed)
            return;

        SolidWorksFileList.Remove(SelectedSolidWorksFile);
    }

    [RelayCommand]
    private void RunJob()
    {

    }
    
    #endregion

    public async void ReplaceAvailableActionsList(ObservableCollection<ProcessActionViewModel> actions)
    {
        if (Actions.Any())
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = $"Override Actions",
                Message = $"Are you sure you want to override the existing actions?",
                DialogWidth = 400
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return;
        }

        Actions = new ObservableCollection<ProcessActionViewModel>(actions);
    }
}