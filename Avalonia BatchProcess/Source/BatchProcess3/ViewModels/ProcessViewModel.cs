
using BatchProcess3.DataStorage;
using BatchProcess3.DataStorage.DataModels;
using BatchProcess3.MainApp;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ProcessViewModel : ViewModelBase, ISelectableItemListViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";
    
    [JsonIgnore]
    public override bool HasChanged =>
        IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, GetType(), _jsonOptions));

    [ObservableProperty] private bool _isNewItem;

    private ObservableCollection<ProcessActionViewModel> _actions;
    
    public ObservableCollection<ProcessActionViewModel> Actions
    {
        get => _actions;
        set => this.SetAndObserveEverything(value, ref _actions, [nameof(HasChanged)]);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ProcessViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        Actions = [];
    }

    public ProcessDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        Actions = Actions.Select(f => f.ToDataModel()).ToList()
    };

    public override string ToString() => $"{JobName} ({Description})";
}

public static class ProcessViewModelExtensions
{
    public static ProcessViewModel ToViewModel(this ProcessDataModel dataModel) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            Actions = new (dataModel.Actions
                .Select(f => f.ToViewModel())
                .OrderBy(f => f.SortOrder))
        };
}