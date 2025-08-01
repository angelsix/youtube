using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionsTabSaveDrawingViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private ObservableCollection<string> _exportFormats = [];

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fileName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty] private bool _isNewItem;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _saveLocation = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _sheetsFilter = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _singleDwgDxf;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _singleEDrawing;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _singlePdf;

    [JsonIgnore]
    public new bool HasChanged =>
        IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, _jsonOptions));

    public ActionsTabSaveDrawingDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        FileName = FileName,
        SaveLocation = SaveLocation,
        ExportFormats = ExportFormats.ToList(),
        SheetsFilter = SheetsFilter.ToList(),
        SingleDwgDxf = SingleDwgDxf,
        SingleEDrawing = SingleEDrawing,
        SinglePdf = SinglePdf
    };
}

public static class ActionsTabSaveDrawingViewModelExtensions
{
    public static ActionsTabSaveDrawingViewModel ToViewModel(this ActionsTabSaveDrawingDataModel dataModel) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            SaveLocation = dataModel.SaveLocation,
            ExportFormats = new ObservableCollection<string>(dataModel.ExportFormats.ToList()),
            FileName = dataModel.FileName,
            SheetsFilter = new ObservableCollection<string>(dataModel.SheetsFilter.ToList()),
            SingleDwgDxf = dataModel.SingleDwgDxf,
            SinglePdf = dataModel.SinglePdf,
            SingleEDrawing = dataModel.SingleEDrawing
        };
}