using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionsTabFileInfoViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _author = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _comments = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty] private bool _isNewItem;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _keywords = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _subject = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _title = "";

    [JsonIgnore]
    public new bool HasChanged =>
        IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, _jsonOptions));

    public ActionsTabFileInfoDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        Author = Author,
        Comments = Comments,
        Keywords = Keywords,
        Subject = Subject,
        Title = Title
    };
}

public static class ActionsTabFileInfoViewModelExtensions
{
    public static ActionsTabFileInfoViewModel ToViewModel(this ActionsTabFileInfoDataModel dataModel) => new()
    {
        Id = dataModel.Id,
        Description = dataModel.Description,
        JobName = dataModel.JobName,
        Author = dataModel.Author,
        Comments = dataModel.Comments,
        Keywords = dataModel.Keywords,
        Subject = dataModel.Subject,
        Title = dataModel.Title
    };
}