using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionsTabSaveModelViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    private ObservableCollection<KeyValueViewModel<string, bool>> _exportFormats = [];

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fileName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty] private bool _isNewItem;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _saveAllConfigurations;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _saveLocation = "";

    public ObservableCollection<KeyValueViewModel<string, bool>> ExportFormats
    {
        get => _exportFormats;
        set => this.SetAndObserveEverything(value, ref _exportFormats, [nameof(HasChanged)]);
    }

    [JsonIgnore]
    public new bool HasChanged =>
        IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, _jsonOptions));

    public ActionsTabSaveModelDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        SaveLocation = SaveLocation,
        ExportFormats = ExportFormats.Where(f => f.Value).Select(f => f.Key).ToList(),
        FileName = FileName,
        SaveAllConfigurations = SaveAllConfigurations
    };
}

public static class ActionsTabSaveModelViewModelExtensions
{
    public static ActionsTabSaveModelViewModel ToViewModel(this ActionsTabSaveModelDataModel dataModel,
        ObservableCollection<string> exportFormats) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            SaveLocation = dataModel.SaveLocation,
            ExportFormats =
                new ObservableCollection<KeyValueViewModel<string, bool>>(exportFormats.Select(f =>
                    new KeyValueViewModel<string, bool>(f, dataModel.ExportFormats.Any(e => e == f)))),
            SaveAllConfigurations = dataModel.SaveAllConfigurations,
            FileName = dataModel.FileName
        };
}