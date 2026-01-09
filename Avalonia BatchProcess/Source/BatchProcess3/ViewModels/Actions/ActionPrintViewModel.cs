using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionPrintViewModel : ActionViewModel, ISelectableItemListViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DrawingExclusionListTitle))]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _drawingExclusionIsWhiteList;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _drawingExclusionList = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _printDrawingRange = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _printDrawings;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string? _printerSettingsId;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _printModels;

    public string DrawingExclusionListTitle => DrawingExclusionIsWhiteList ? "White List" : "Black List";

    public new ActionPrintDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        DrawingExclusionIsWhiteList = DrawingExclusionIsWhiteList,
        DrawingExclusionList = DrawingExclusionList,
        JobName = JobName,
        PrintDrawingRange = PrintDrawingRange,
        PrintDrawings = PrintDrawings,
        PrinterSettingsId = PrinterSettingsId,
        PrintModels = PrintModels
    };
}

public static class ActionPrintViewModelExtensions
{
    public static ActionPrintViewModel ToViewModel(this ActionPrintDataModel dataModel) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            DrawingExclusionIsWhiteList = dataModel.DrawingExclusionIsWhiteList,
            DrawingExclusionList = dataModel.DrawingExclusionList,
            PrintDrawingRange = dataModel.PrintDrawingRange,
            PrintDrawings = dataModel.PrintDrawings,
            PrinterSettingsId = dataModel.PrinterSettingsId,
            PrintModels = dataModel.PrintModels
        };
}