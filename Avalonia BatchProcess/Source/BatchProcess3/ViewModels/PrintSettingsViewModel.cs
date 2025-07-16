using BatchProcess3.DataModels;
using BatchProcess3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;

namespace BatchProcess3.ViewModels;

public partial class PrintSettingsViewModel : ConfirmDialogViewModel
{
    [ObservableProperty] private string _id = Guid.NewGuid().ToString("N");

    [ObservableProperty] private string _name = "";

    [ObservableProperty] private string _description = "";

    [ObservableProperty] private ObservableCollection<PrintSettingsProfileViewModel> _printerSettingProfiles = [];

    [ObservableProperty] private bool _canEdit = true;

    [ObservableProperty] private bool _canDelete = true;

    [ObservableProperty] private int _copies;

    public PrintSettingsViewModel() : base()
    {
        Title = "Print Settings";
        Message = "Specify the printer settings to use for each paper size, or leave as default.";
        ConfirmText = "Save";
        CancelText = "Cancel";

        // TODO: Think of a better place to do this
        //PrinterSettingProfiles = databaseService.GetPrintSettingsProfiles()
    }

    protected override void OnDesignTimeConstructor() => DesignTimeData();

    private void DesignTimeData()
    {
        // TODO: Pull from database 
        var printerSettingsItem = new PrintSettingsProfileViewModel
        {
            Id = "2", Height = 200, Width = 140, ScaleToFit = true
        };

        PrinterSettingProfiles =
        [
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
        ];
    }
}

public static class PrintSettingsViewModelExtensions
{
    public static PrintSettingsDataModel ToDataModel(this PrintSettingsViewModel viewModel)
    {
        return new PrintSettingsDataModel()
        {
            Id = viewModel.Id,
            CanDelete = viewModel.CanDelete,
            CanEdit = viewModel.CanEdit,
            Copies = viewModel.Copies,
            Description = viewModel.Description,
            Name = viewModel.Name,
            PrinterSettingProfiles = viewModel.PrinterSettingProfiles.ToDataModels()
        };
    }

    public static List<PrintSettingsDataModel> ToDataModels(
        this ObservableCollection<PrintSettingsViewModel> viewModels) =>
        viewModels.Select(ToDataModel).ToList();

    public static PrintSettingsViewModel ToViewModel(this PrintSettingsDataModel dataModel)
    {
        return new PrintSettingsViewModel()
        {
            Name = dataModel.Name,
            Description = dataModel.Description,
            Id = dataModel.Id,
            Copies = dataModel.Copies,
            CanEdit = dataModel.CanEdit,
            CanDelete = dataModel.CanDelete,
            PrinterSettingProfiles = new ObservableCollection<PrintSettingsProfileViewModel>(dataModel.PrinterSettingProfiles
                .OrderBy(profile => profile.Type)
                .Select(profile => new PrintSettingsProfileViewModel
                {
                    Id = profile.Id,
                    DrawingColor = profile.DrawingColor,
                    Height = profile.Height,
                    Orientation = profile.Orientation,
                    PaperSize = profile.PaperSize,
                    PrinterName = profile.PrinterName,
                    ScaleToFit = profile.ScaleToFit,
                    SourceTray = profile.SourceTray,
                    Type = profile.Type,
                    Width = profile.Width,
                }))
        };
    }
    
    public static ObservableCollection<PrintSettingsViewModel> ToViewModels(
        this List<PrintSettingsDataModel> dataModels)
    {
        return new ObservableCollection<PrintSettingsViewModel>(dataModels
            .OrderBy(f => f.Name)
            .Select(ToViewModel));
    }
}