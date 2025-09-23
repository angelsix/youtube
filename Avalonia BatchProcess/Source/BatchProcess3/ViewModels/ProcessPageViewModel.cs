
using BatchProcess3.DataStorage;
using BatchProcess3.Dialog;
using BatchProcess3.MainApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class ProcessPageViewModel : PageViewModel
{
    #region Members

    private DatabaseService _databaseService;
    private MainViewModel _mainViewModel;
    private DialogService _dialogService;
    
    #endregion
    
    #region Properties

    [ObservableProperty] private ObservableCollection<ProcessViewModel> _processList = [];

    public bool ProcessListHasItems => ProcessList.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedProcessListItem))]
    private string _selectedProcessListItemId = "";

    public ProcessViewModel? SelectedProcessListItem =>
        ProcessList.FirstOrDefault(f => f.Id == SelectedProcessListItemId);

    #endregion

    #region Constructor

    public ProcessPageViewModel(
        MainViewModel mainViewModel,
        DialogService dialogService, 
        DatabaseService databaseService) : base(ApplicationPageNames.Process)
    {
        _mainViewModel = mainViewModel;
        _dialogService = dialogService;
        _databaseService = databaseService;
        
        FetchProcesses();
    }

// Design-time only
    public ProcessPageViewModel() : this(new MainViewModel(), new DialogService(() => null), new DatabaseService(new ApplicationDbContext()))
    {
        if (!Avalonia.Controls.Design.IsDesignMode) throw new InvalidOperationException("Parameterless constructor is only for design time use");
    }

    protected override void OnDesignTimeConstructor() => FetchProcesses();
    
    #endregion
    
    [RelayCommand]
    private void FetchProcesses()
    {
        var list = _databaseService.GetProcessList();

        ProcessList = new ObservableCollection<ProcessViewModel>(list
            .OrderBy(f => f.JobName)
            .Select(f => f.ToViewModel()));

        // Update ProcessListHasItems when collection changes
        ProcessList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ProcessListHasItems));

        if (ProcessList.Count <= 0) return;

        // Select first item
        SelectedProcessListItemId = ProcessList.First().Id;

        // Store last fetched database save states
        foreach (var item in ProcessList)
            item.SetSavedState();
    }
    
    [RelayCommand]
    private void AddNewProcess()
    {
        // Create a new item
        var newItem = new ProcessViewModel
        {
            Id = Guid.NewGuid().ToString("N"),
            IsNewItem = true,
            JobName = "New Process",
        };

        // Add to the print list
        ProcessList.Add(newItem);

        // Select item
        SelectedProcessListItemId = newItem.Id;
    }   
    
    [RelayCommand]
    private async Task DeleteProcessItemAsync(string id)
    {
        if (ProcessList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeleteProcessItemFromUIAsync(id))
            // Delete from database
            _databaseService.DeleteProcessItem(id);
    }
    
    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteProcessItemFromUIAsync(string id, bool warn = true)
    {
        var index = ProcessList.IndexOf(ProcessList.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = "Delete Process Item?",
                Message = $"Are you sure you want to delete ' {ProcessList[index].JobName}'?",
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

            await _dialogService.ShowDialog(_mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        ProcessList.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (ProcessList.Count > 0)
            SelectedProcessListItemId = ProcessList[index].Id;

        return true;
    }

    [RelayCommand]
    private void DeleteSelectedProcessAction(int sortOrder)
    {
        if (SelectedProcessListItem == null)
            // TODO: Throw/Warn?
            return;

        var item = SelectedProcessListItem.Actions.FirstOrDefault(f => f.SortOrder ==  sortOrder);
        
        if (item != null)
            SelectedProcessListItem.Actions.Remove(item);
    }
    
    [RelayCommand]
    private async Task CancelProcessItem()
    {
        // Ignore if nothing is selected
        if (SelectedProcessListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedProcessListItem.IsNewItem)
            await DeleteProcessItemFromUIAsync(SelectedProcessListItem.Id, false);
        else
            SelectedProcessListItem.RestoreState();
    }
    
    [RelayCommand]
    private Task SaveProcessItemAsync()
    {
        // Ignore if no selection
        if (SelectedProcessListItem == null)
            return Task.CompletedTask;

        // If the selected item is new...
        if (SelectedProcessListItem.IsNewItem)
            _databaseService.AddProcessItem(SelectedProcessListItem.ToDataModel());
        else
            _databaseService.UpdateProcessItem(SelectedProcessListItem.ToDataModel());

        // Flag new item as not new
        SelectedProcessListItem.IsNewItem = false;
        SelectedProcessListItem.SetSavedState();

        return Task.CompletedTask;
    }
}