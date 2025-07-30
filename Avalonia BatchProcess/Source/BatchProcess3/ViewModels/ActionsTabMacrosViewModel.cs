using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionsTabMacrosViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeAssemblies;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeDrawings;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeParts;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty] private bool _isNewItem;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _macroPath = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _moduleName = "";

    [JsonIgnore]
    public new bool HasChanged =>
        IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, _jsonOptions));

    public ActionsTabMacrosDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        MacroPath = MacroPath,
        ModuleName = ModuleName,
        ExcludeParts = ExcludeParts,
        ExcludeDrawings = ExcludeDrawings,
        ExcludeAssemblies = ExcludeAssemblies
    };
}

public static class ActionsTabMacrosViewModelExtensions
{
    public static ActionsTabMacrosViewModel ToViewModel(this ActionsTabMacrosDataModel dataModel) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            ExcludeAssemblies = dataModel.ExcludeAssemblies,
            ExcludeParts = dataModel.ExcludeParts,
            ExcludeDrawings = dataModel.ExcludeDrawings,
            MacroPath = dataModel.MacroPath,
            ModuleName = dataModel.ModuleName
        };
}