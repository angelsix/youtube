using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionMacrosViewModel : ActionViewModel, ISelectableItemListViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeAssemblies;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeDrawings;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeParts;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _macroPath = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _moduleName = "";

    public new ActionMacrosDataModel ToDataModel() => new()
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

public static class ActionMacrosViewModelExtensions
{
    public static ActionMacrosViewModel ToViewModel(this ActionMacrosDataModel dataModel) =>
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