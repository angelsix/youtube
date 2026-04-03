using Avalonia.Controls;
using Avalonia.Threading;
using BatchProcess3.Core.SolidWorks;
using BatchProcess3Host.Services;
using BatchProcess3Host.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchProcess3Host.ViewModels;

public partial class HostJobItemViewModel : ViewModelBase
{
    public string JobId { get; set; } = "";

    [ObservableProperty] private string _jobName = "";
    [ObservableProperty] private string _status = "Running";
    [ObservableProperty] private int _totalFiles;
    [ObservableProperty] private int _completedFiles;
    [ObservableProperty] private string _currentAction = "";
    [ObservableProperty] private int _errorCount;
    [ObservableProperty] private bool _isComplete;

    public double ProgressFraction => TotalFiles > 0 ? (double)CompletedFiles / TotalFiles : 0;
    public string ProgressText => $"{CompletedFiles}/{TotalFiles}";

    partial void OnCompletedFilesChanged(int value)
    {
        OnPropertyChanged(nameof(ProgressFraction));
        OnPropertyChanged(nameof(ProgressText));
    }

    partial void OnTotalFilesChanged(int value)
    {
        OnPropertyChanged(nameof(ProgressFraction));
        OnPropertyChanged(nameof(ProgressText));
    }
}

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly JobExecutionService? _jobService;

    [ObservableProperty] private string _serverStatus =$"Listening on Port {BatchProcessHostUrls.DefaultPort} & Discovery {BatchProcessHostUrls.DiscoveryPort}";
    [ObservableProperty] private int _activeJobCount;
    [ObservableProperty] private int _completedJobCount;
    [ObservableProperty] private ObservableCollection<HostJobItemViewModel> _jobs = [];

    public MainWindowViewModel(JobExecutionService jobService)
    {
        _jobService = jobService;
        _jobService.JobsChanged += OnJobsChanged;
    }

    // Design-time constructor
    public MainWindowViewModel()
    {
    }

    [RelayCommand]
    private void OpenJobDetail(HostJobItemViewModel job)
    {
        if (_jobService is null) return;

        var state = _jobService.GetJobState(job.JobId);
        if (state is null) return;

        var vm = new JobDetailWindowViewModel();
        vm.LoadFromState(state, _jobService);

        var window = new JobDetailWindow
        {
            DataContext = vm,
        };

        window.Closed += (_, _) => vm.StopRefresh();
        window.Show();
    }

    private void OnJobsChanged()
    {
        Dispatcher.UIThread.Post(RefreshJobs);
    }

    private void RefreshJobs()
    {
        if (_jobService is null) return;

        var allJobs = _jobService.GetAllJobs();

        foreach (var state in allJobs)
        {
            var existing = Jobs.FirstOrDefault(j => j.JobId == state.JobId);
            if (existing is not null)
            {
                existing.Status = state.Status;
                existing.CompletedFiles = state.CompletedFiles;
                existing.CurrentAction = state.IsComplete ? "" : $"{state.CurrentFileName} — {state.CurrentActionName}";
                existing.ErrorCount = state.Errors.Count;
                existing.IsComplete = state.IsComplete;
            }
            else
            {
                Jobs.Insert(0, new HostJobItemViewModel
                {
                    JobId = state.JobId,
                    JobName = state.JobName,
                    Status = state.Status,
                    TotalFiles = state.TotalFiles,
                    CompletedFiles = state.CompletedFiles,
                    CurrentAction = $"{state.CurrentFileName} — {state.CurrentActionName}",
                    ErrorCount = state.Errors.Count,
                    IsComplete = state.IsComplete,
                });
            }
        }

        ActiveJobCount = allJobs.Count(j => !j.IsComplete);
        CompletedJobCount = allJobs.Count(j => j.IsComplete);
    }
}
