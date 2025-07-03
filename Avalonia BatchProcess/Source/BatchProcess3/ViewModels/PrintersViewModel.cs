using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BatchProcess3.ViewModels;

public partial class PrintersViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _id = string.Empty;
    
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _paperSizes = []; 

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _sourceTrays = []; 
}