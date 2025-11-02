
using BatchProcess3.Actions;
using BatchProcess3.DataStorage;
using BatchProcess3.DataStorage.DataModels;
using BatchProcess3.Dialog;
using BatchProcess3.MainApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class ProcessPageViewModel(
    MainViewModel mainViewModel,
    DialogService dialogService, 
    DatabaseService databaseService,
    ActionService actionService) : PageViewModel(ApplicationPageNames.Process)
{
    #region Properties

    [ObservableProperty] private SelectableItemListViewModel<ProcessViewModel>? _processList;
    
    [ObservableProperty] private ObservableCollection<AvailableActionItemViewModel>? _availableActionsList;
    
    #endregion

    #region Constructor

    // Design-time only
    public ProcessPageViewModel() : this(new MainViewModel(), new DialogService(() => null), new DatabaseService(new ApplicationDbContext()), new ActionService(new DatabaseService(new ApplicationDbContext())))
    {
        if (!Avalonia.Controls.Design.IsDesignMode) throw new InvalidOperationException("Parameterless constructor is only for design time use");
    }
    
    #endregion
    
    #region Commands
    
    [RelayCommand]
    private void Initialize()
    {
        ProcessList = new SelectableItemListViewModel<ProcessViewModel>(
            title: "Process",
            mainViewModel: mainViewModel,
            dialogService: dialogService,
            getList: () =>
            {
                var list = databaseService.GetProcessList();
                
                // TODO: Update job name and description for each action as they will be out of date

                return new ObservableCollection<ProcessViewModel>(list
                    .OrderBy(f => f.JobName)
                    .Select(f => f.ToViewModel()));
            },
            createItem: () => new ProcessViewModel
            {
                Id = Guid.NewGuid().ToString("N"), IsNewItem = true, JobName = "New Process"
            },
            deleteItem: databaseService.DeleteProcessItem,
            addItem: (item) => databaseService.AddProcessItem(item.ToDataModel()),
            updateItem: (item) =>
            {
                UpdateActionSortOrder();
                databaseService.UpdateProcessItem(item.ToDataModel());
            });

        AvailableActionsList = actionService.GetAvailableActionsList();
        
        ProcessList.FetchList();
    }

    [RelayCommand]
    public void AddActionToProcess(AvailableActionItemViewModel item) => InsertActionToProcess(item, -1);
    
    public void InsertActionToProcess(AvailableActionItemViewModel item, int index)
    {
        if (ProcessList.SelectedItem == null) return;

        if (item.ActionViewModel == null) return;
        
        var copy = new AvailableActionItemViewModel();
        copy.RestoreState(item.GetState());

        // Give the copy a new unique ID
        copy.ActionViewModel!.Id = Guid.NewGuid().ToString("N");
        
        if (index <= -1 || index > ProcessList.SelectedItem.Actions.Count || ProcessList.SelectedItem.Actions.Count == 0)
            ProcessList.SelectedItem.Actions.Add(copy.ActionViewModel!);
        else
            ProcessList.SelectedItem.Actions.Insert(index, copy.ActionViewModel!);
        
        // Update sort order
        UpdateActionSortOrder();
    }

    [RelayCommand]
    private void UpdateActionSortOrder()
    {
        if (ProcessList.SelectedItem == null) return;
        
        foreach (var (action, index) in ProcessList.SelectedItem.Actions.Select((f, i) => (f, i)))
            // Sort order should match position in list
            action.SortOrder = index;
    }

    [RelayCommand]
    private void DeleteActionFromProcess(ProcessActionViewModel item) => ProcessList.SelectedItem?.Actions.Remove(item);

    #endregion
}