using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels;

public partial class PrinterDetailsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _id = string.Empty;
    
    [ObservableProperty]
    private string _name = string.Empty;
}