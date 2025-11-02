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
using System.Linq;

namespace BatchProcess3.ViewModels;

public partial class HomePageViewModel : PageViewModel
{
    #region Members
    
    private DatabaseService _databaseService;
    private MainViewModel _mainViewModel;
    private DialogService _dialogService;
    private ActionService _actionService;
    
    #endregion
    
    #region Properties

    [ObservableProperty] private ObservableCollection<AvailableActionItemViewModel> _availableActionsList;

    private ObservableCollection<ProcessActionViewModel> _actions;
    
    public ObservableCollection<ProcessActionViewModel> Actions
    {
        get => _actions;
        set => this.SetAndObserveEverything(value, ref _actions, [nameof(HasChanged)]);
    }
    
    #endregion Properties

    #region Constructor
    
    public HomePageViewModel(
        MainViewModel mainViewModel,
        DialogService dialogService, 
        DatabaseService databaseService,
        ActionService actionService) : base(ApplicationPageNames.Home)
    {
        Initialize(mainViewModel, dialogService, databaseService, actionService);
    }

    private void Initialize(
        MainViewModel mainViewModel, 
        DialogService dialogService, 
        DatabaseService databaseService, 
        ActionService actionService)
    {
        _mainViewModel = mainViewModel;
        _dialogService = dialogService;
        _databaseService = databaseService;
        _actionService = actionService;

        Actions = [];
        AvailableActionsList = _actionService.GetAvailableActionsList();
    }

    // Design-time only
    public HomePageViewModel() : this(new MainViewModel(), new DialogService(() => null), new DatabaseService(new ApplicationDbContext()), new ActionService(new DatabaseService(new ApplicationDbContext())))
    {
        if (!Avalonia.Controls.Design.IsDesignMode) throw new InvalidOperationException("Parameterless constructor is only for design time use");
    }

    protected override void OnDesignTimeConstructor() => Initialize(new  MainViewModel(), new DialogService(() => null), new DatabaseService(new ApplicationDbContext()), new ActionService(new DatabaseService(new ApplicationDbContext())));
    
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
    
    #endregion
}