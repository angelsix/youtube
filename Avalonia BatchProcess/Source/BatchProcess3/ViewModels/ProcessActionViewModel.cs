using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ProcessActionViewModel : ActionViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _processId = "";
    
    public new ProcessActionDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        SortOrder = SortOrder,
        ProcessId = ProcessId
    };
}

public static class ProcessActionViewModelExtensions
{
    public static ProcessActionViewModel ToViewModel(this ProcessActionDataModel dataModel) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            SortOrder = dataModel.SortOrder,
            ProcessId = dataModel.ProcessId
        };
    
    public static ProcessActionViewModel ToProcessActionViewModel(this ActionDataModel dataModel) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            SortOrder = dataModel.SortOrder
        };

}