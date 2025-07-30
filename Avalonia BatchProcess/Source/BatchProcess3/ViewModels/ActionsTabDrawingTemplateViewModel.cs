using BatchProcess3.DataStorage.DataModels;
using BatchProcess3.DrawingTemplates;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionsTabDrawingTemplateViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _currentTemplatePath = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty] private bool _isNewItem;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _newTemplatePath = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private DrawingTemplateOperation _operation;

    [JsonIgnore]
    public new bool HasChanged =>
        IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, _jsonOptions));

    public ActionsTabDrawingTemplateDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        Operation = Operation,
        CurrentTemplatePath = CurrentTemplatePath,
        NewTemplatePath = NewTemplatePath
    };
}

public static class ActionsTabDrawingTemplateViewModelExtensions
{
    public static ActionsTabDrawingTemplateViewModel ToViewModel(this ActionsTabDrawingTemplateDataModel dataModel) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            CurrentTemplatePath = dataModel.CurrentTemplatePath,
            NewTemplatePath = dataModel.NewTemplatePath,
            Operation = dataModel.Operation
        };
}