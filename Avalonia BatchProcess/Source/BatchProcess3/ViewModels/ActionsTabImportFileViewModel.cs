using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionsTabImportFileViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _addToProject;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fileName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _importArguments = "";

    [ObservableProperty] private bool _isNewItem;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _saveLocation = "";

    [JsonIgnore]
    public new bool HasChanged =>
        IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, _jsonOptions));

    public ActionsTabImportFileDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        FileName = FileName,
        SaveLocation = SaveLocation,
        ImportArguments = ImportArguments,
        AddToProject = AddToProject
    };
}

public static class ActionsTabImportFileViewModelExtensions
{
    public static ActionsTabImportFileViewModel ToViewModel(this ActionsTabImportFileDataModel dataModel) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            SaveLocation = dataModel.SaveLocation,
            FileName = dataModel.FileName,
            AddToProject = dataModel.AddToProject,
            ImportArguments = dataModel.ImportArguments
        };
}