
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

    [ObservableProperty] private SelectableItemListViewModel<ProcessViewModel> _processList;
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
        
        _processList = new
        (
            title: "Process",
            mainViewModel: mainViewModel,
            dialogService: dialogService,
            getList: () =>
            {
                var list = databaseService.GetProcessList();

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
            updateItem: (item) => databaseService.UpdateProcessItem(item.ToDataModel())
        );
        
        ProcessList.FetchList();
    }

// Design-time only
    public ProcessPageViewModel() : this(new MainViewModel(), new DialogService(() => null), new DatabaseService(new ApplicationDbContext()))
    {
        if (!Avalonia.Controls.Design.IsDesignMode) throw new InvalidOperationException("Parameterless constructor is only for design time use");
    }

    protected override void OnDesignTimeConstructor() => ProcessList.FetchList();
    
    #endregion
}