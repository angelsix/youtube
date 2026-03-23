using Avalonia.Threading;
using BatchProcess3.Core.Jobs;
using BatchProcess3Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace BatchProcess3Host.ViewModels;

public partial class HostStepItemViewModel : ViewModelBase
{
    [ObservableProperty] private string _fileName = "";
    [ObservableProperty] private string _actionName = "";
    [ObservableProperty] private string _status = "";
    [ObservableProperty] private string _durationText = "";
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _isFailed;
}

public partial class JobDetailWindowViewModel : ViewModelBase
{
    private JobExecutionService? _jobService;
    private string _jobId = "";
    private Timer? _refreshTimer;

    [ObservableProperty] private string _title = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private string _statusText = "";
    [ObservableProperty] private int _totalFiles;
    [ObservableProperty] private int _completedFiles;
    [ObservableProperty] private string _progressText = "";
    [ObservableProperty] private string _elapsedText = "";
    [ObservableProperty] private int _successCount;
    [ObservableProperty] private int _failedCount;
    [ObservableProperty] private ObservableCollection<HostStepItemViewModel> _steps = [];
    [ObservableProperty] private ObservableCollection<string> _errors = [];

    public void LoadFromState(JobExecutionState state, JobExecutionService jobService)
    {
        _jobService = jobService;
        _jobId = state.JobId;

        RefreshFromState(state);

        // Start a timer to refresh while job is running
        if (!state.IsComplete)
        {
            _refreshTimer = new Timer(_ => Dispatcher.UIThread.Post(RefreshFromService),
                null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
    }

    private void RefreshFromService()
    {
        var state = _jobService?.GetJobState(_jobId);
        if (state is null) return;

        RefreshFromState(state);

        if (state.IsComplete)
        {
            _refreshTimer?.Dispose();
            _refreshTimer = null;
        }
    }

    private void RefreshFromState(JobExecutionState state)
    {
        Title = state.JobName;
        Description = state.Description;
        StatusText = state.Status;
        TotalFiles = state.TotalFiles;
        CompletedFiles = state.CompletedFiles;
        ProgressText = $"{state.CompletedFiles} / {state.TotalFiles} files";

        var elapsed = state.Stopwatch.Elapsed;
        ElapsedText = elapsed.TotalMinutes >= 1 ? $"{elapsed.Minutes}m {elapsed.Seconds}s" : $"{elapsed.TotalSeconds:0.0}s";

        Errors = new ObservableCollection<string>(state.Errors);

        var stepVms = state.Steps.Select(s => new HostStepItemViewModel
        {
            FileName = s.FileName,
            ActionName = s.ActionName,
            Status = s.Status,
            DurationText = s.DurationMs >= 1000 ? $"{s.DurationMs / 1000.0:0.0}s" : $"{s.DurationMs}ms",
            ErrorMessage = s.ErrorMessage,
            IsFailed = s.Status == "Failed",
        });

        Steps = new ObservableCollection<HostStepItemViewModel>(stepVms);
        SuccessCount = Steps.Count(s => !s.IsFailed);
        FailedCount = Steps.Count(s => s.IsFailed);
    }

    public void StopRefresh()
    {
        _refreshTimer?.Dispose();
        _refreshTimer = null;
    }
}
