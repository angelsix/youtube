using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BatchProcess3.ViewModels;

public partial class ActionsPrinterProfileViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _id;
    
    [ObservableProperty]
    private string _name;
    
    [ObservableProperty]
    private string _description;
    
    [ObservableProperty]
    private ObservableCollection<ActionsPrinterSettingsViewModel> _printerSettings;

    [ObservableProperty]
    private int _copies;
}