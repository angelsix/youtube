using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionPrintSettingsViewModel : ConfirmDialogViewModel
{
    [ObservableProperty] private string _id = Guid.NewGuid().ToString("N");

    [ObservableProperty] private string _name = "";

    [ObservableProperty] private string _description = "";

    [ObservableProperty] private ObservableCollection<ActionPrintSettingsProfileViewModel> _printerSettingProfiles = [];

    [ObservableProperty] private bool _canEdit = true;

    [ObservableProperty] private bool _canDelete = true;

    [ObservableProperty] private int _copies;

    public ActionPrintSettingsViewModel() : base()
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
        var printerSettingsItem = new ActionPrintSettingsProfileViewModel
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

public static class ActionPrintSettingsViewModelExtensions
{
    public static ActionPrintSettingsDataModel ToDataModel(this ActionPrintSettingsViewModel viewModel)
    {
        return new ActionPrintSettingsDataModel()
        {
            Id = viewModel.Id,
            CanDelete = viewModel.CanDelete,
            CanEdit = viewModel.CanEdit,
            Copies = viewModel.Copies,
            Description = viewModel.Description,
            JobName = viewModel.Name,
            PrinterSettingProfiles = viewModel.PrinterSettingProfiles.ToDataModels()
        };
    }

    public static List<ActionPrintSettingsDataModel> ToDataModels(
        this ObservableCollection<ActionPrintSettingsViewModel> viewModels) =>
        viewModels.Select(ToDataModel).ToList();

    public static ActionPrintSettingsViewModel ToViewModel(this ActionPrintSettingsDataModel dataModel)
    {
        return new ActionPrintSettingsViewModel()
        {
            Name = dataModel.JobName,
            Description = dataModel.Description,
            Id = dataModel.Id,
            Copies = dataModel.Copies,
            CanEdit = dataModel.CanEdit,
            CanDelete = dataModel.CanDelete,
            PrinterSettingProfiles = new ObservableCollection<ActionPrintSettingsProfileViewModel>(dataModel.PrinterSettingProfiles
                .OrderBy(profile => profile.Type)
                .Select(profile => new ActionPrintSettingsProfileViewModel
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
    
    public static ObservableCollection<ActionPrintSettingsViewModel> ToViewModels(
        this List<ActionPrintSettingsDataModel> dataModels)
    {
        return new ObservableCollection<ActionPrintSettingsViewModel>(dataModels
            .OrderBy(f => f.JobName)
            .Select(ToViewModel));
    }
}