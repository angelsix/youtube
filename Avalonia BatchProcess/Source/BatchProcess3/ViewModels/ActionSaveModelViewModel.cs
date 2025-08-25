using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionSaveModelViewModel : ActionViewModel
{
    private ObservableCollection<KeyValueViewModel<string, bool>> _exportFormats = [];

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fileName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _saveAllConfigurations;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _saveLocation = "";

    public ObservableCollection<KeyValueViewModel<string, bool>> ExportFormats
    {
        get => _exportFormats;
        set => this.SetAndObserveEverything(value, ref _exportFormats, [nameof(HasChanged)]);
    }

    public new ActionSaveModelDataModel ToDataModel() => new()
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

public static class ActionSaveModelViewModelExtensions
{
    public static ActionSaveModelViewModel ToViewModel(this ActionSaveModelDataModel dataModel,
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