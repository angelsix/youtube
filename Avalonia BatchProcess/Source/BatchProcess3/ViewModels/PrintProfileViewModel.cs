using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BatchProcess3.ViewModels;

public partial class PrintProfileViewModel : ConfirmDialogViewModel
{
    [ObservableProperty]
    private string _id = "";
    
    [ObservableProperty]
    private string _name = "";
    
    [ObservableProperty]
    private string _description = "";
    
    [ObservableProperty]
    private ObservableCollection<ActionsPrinterSettingsViewModel> _printerSettings = [];

    [ObservableProperty]
    private int _copies;

    public PrintProfileViewModel() : base()
    {
        Title = "Print Settings";
        Message = "Specify the printer settings to use for each paper size, or leave as default.";
        ConfirmText = "Save";
        CancelText = "Cancel";
        
        // TODO: Remove once we pull from database
        DesignTimeData();
    }

    protected override void OnDesignTimeConstructor() => DesignTimeData();

    private void DesignTimeData()
    {
        // TODO: Pull from database 
        var printerSettingsItem = new ActionsPrinterSettingsViewModel
        {
            Id = "2",
            Height = 200,
            Width = 140,
            ScaleToFit = true
        };
        
        PrinterSettings =
        [
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
        ];

    }
}