using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BatchProcess3.ViewModels;

public partial class PrintSettingsViewModel : ConfirmDialogViewModel
{
    [ObservableProperty]
    private string _id = "";
    
    [ObservableProperty]
    private string _name = "";
    
    [ObservableProperty]
    private string _description = "";
    
    [ObservableProperty]
    private ObservableCollection<PrintSettingsProfileViewModel> _printerSettingProfiles = [];

    [ObservableProperty] private bool _canEdit = true;
    
    [ObservableProperty] private bool _canDelete = true;
    
    [ObservableProperty]
    private int _copies;

    public PrintSettingsViewModel() : base()
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
        var printerSettingsItem = new PrintSettingsProfileViewModel
        {
            Id = "2",
            Height = 200,
            Width = 140,
            ScaleToFit = true
        };
        
        PrinterSettingProfiles =
        [
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
        ];

    }
}