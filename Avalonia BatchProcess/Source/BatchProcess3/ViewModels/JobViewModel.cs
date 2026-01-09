using BatchProcess3.Core.SolidWorks;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BatchProcess3.ViewModels;

public partial class JobViewModel : ViewModelBase
{
    #region Properties

    [ObservableProperty] 
    private ObservableCollection<SolidWorksFileDetails> _solidWorksFileList = [];

    [ObservableProperty] 
    private ObservableCollection<ProcessActionViewModel> _actions = [];
    
    [ObservableProperty]
    private bool _quickView = false;

    [ObservableProperty]
    private bool _saveOnClose = true;

    #endregion Properties

}