using BatchProcess3.Core.Jobs;
using BatchProcess3.ViewModels.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace BatchProcess3.ViewModels;

public partial class JobStepItemViewModel : ViewModelBase
{
    [ObservableProperty] private string _fileName = "";
    [ObservableProperty] private string _actionName = "";
    [ObservableProperty] private string _status = "";
    [ObservableProperty] private string _durationText = "";
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _isFailed;
}

public partial class JobDetailDialogViewModel : DialogViewModel
{
    private JobItemViewModel? _sourceJob;

    [ObservableProperty] private string _title = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private string _statusText = "";
    [ObservableProperty] private JobStatus _status;
    [ObservableProperty] private int _totalFiles;
    [ObservableProperty] private int _completedFiles;
    [ObservableProperty] private string _progressText = "";
    [ObservableProperty] private string _elapsedText = "";
    [ObservableProperty] private string _dateText = "";
    [ObservableProperty] private int _successCount;
    [ObservableProperty] private int _failedCount;
    [ObservableProperty] private double _dialogWidth = 700;
    [ObservableProperty] private double _dialogHeight = 550;
    [ObservableProperty] private ObservableCollection<JobStepItemViewModel> _steps = [];
    [ObservableProperty] private ObservableCollection<string> _errors = [];

    public double ProgressFraction => TotalFiles > 0 ? (double)CompletedFiles / TotalFiles : 0;

    public void LoadFromJob(JobItemViewModel job)
    {
        _sourceJob = job;
        _sourceJob.PropertyChanged += OnSourceJobChanged;

        RefreshFromJob();
    }

    private void OnSourceJobChanged(object? sender, PropertyChangedEventArgs e)
    {
        RefreshFromJob();
    }

    private void RefreshFromJob()
    {
        if (_sourceJob is null) return;

        Title = _sourceJob.Name;
        Description = _sourceJob.Description;
        Status = _sourceJob.Status;
        StatusText = _sourceJob.StatusText;
        TotalFiles = _sourceJob.TotalFiles;
        CompletedFiles = _sourceJob.CompletedFiles;
        ProgressText = _sourceJob.ProgressText;
        ElapsedText = _sourceJob.ElapsedText;
        DateText = _sourceJob.DateText;
        Errors = new ObservableCollection<string>(_sourceJob.Errors);

        var stepVms = _sourceJob.Steps.Select(s => new JobStepItemViewModel
        {
            FileName = s.FileName,
            ActionName = s.ActionName,
            Status = s.Status,
            DurationText = FormatDuration(s.DurationMs),
            ErrorMessage = s.ErrorMessage,
            IsFailed = s.Status == "Failed",
        });

        Steps = new ObservableCollection<JobStepItemViewModel>(stepVms);
        SuccessCount = Steps.Count(s => !s.IsFailed);
        FailedCount = Steps.Count(s => s.IsFailed);
    }

    private static string FormatDuration(long ms)
    {
        return ms >= 1000 ? $"{ms / 1000.0:0.0}s" : $"{ms}ms";
    }

    [RelayCommand]
    private void CloseDialog()
    {
        if (_sourceJob is not null)
            _sourceJob.PropertyChanged -= OnSourceJobChanged;

        Close();
    }
}
