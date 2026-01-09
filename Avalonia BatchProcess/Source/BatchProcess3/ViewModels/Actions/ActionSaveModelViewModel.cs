using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionSaveModelViewModel : ActionViewModel, ISelectableItemListViewModel
{
    private ObservableCollection<KeyValueViewModel<string, bool>> _exportFormats;

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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ActionSaveModelViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        ExportFormats = [];
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