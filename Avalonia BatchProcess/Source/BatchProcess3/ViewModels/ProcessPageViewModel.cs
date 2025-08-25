
using BatchProcess3.DataStorage;
using BatchProcess3.MainApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchProcess3.ViewModels;

public partial class ProcessPageViewModel : PageViewModel
{
    #region Members

    private DatabaseService _databaseService;
        
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

    public ProcessPageViewModel(DatabaseService databaseService) : base(ApplicationPageNames.Process)
    {
        _databaseService = databaseService;
        
        FetchProcesses();
    }

// Design-time only
    public ProcessPageViewModel() : this(new DatabaseService(new ApplicationDbContext()))
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

}