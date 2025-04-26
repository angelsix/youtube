using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class PrinterSettingsViewModel : ConfirmDialogViewModel
{
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private uint _copies = 2;
    
    public PrinterSettingsViewModel()
    {
        Title = "Print Settings";
        Message = "Specify the printer settings to use for each paper size, or leave as default.";
    }
}