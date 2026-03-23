using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using BatchProcess3.Core.Jobs;
using BatchProcess3.Dialog;
using BatchProcess3.MainApp;
using BatchProcess3.SolidWorks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels.Pages;

public enum JobStatus
{
    Running,
    Success,
    Failed,
    Paused
}

public enum SortColumn
{
    Name,
    Description,
    Progress,
    Status,
    Date
}

public partial class JobItemViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private int _totalFiles;
    [ObservableProperty] private int _completedFiles;
    [ObservableProperty] private JobStatus _status;
    [ObservableProperty] private string _dateText = "";
    [ObservableProperty] private int _sortIndex;
    [ObservableProperty] private long _elapsedMs;
    [ObservableProperty] private ObservableCollection<JobStepDetail> _steps = [];
    [ObservableProperty] private ObservableCollection<string> _errors = [];

    /// <summary>
    /// The job ID used to poll for progress from the host
    /// </summary>
    public string JobId { get; set; } = "";

    public string ElapsedText
    {
        get
        {
            var ts = TimeSpan.FromMilliseconds(ElapsedMs);
            return ts.TotalMinutes >= 1 ? $"{ts.Minutes}m {ts.Seconds}s" : $"{ts.TotalSeconds:0.0}s";
        }
    }

    partial void OnElapsedMsChanged(long value) => OnPropertyChanged(nameof(ElapsedText));

    public double ProgressPercentage => TotalFiles > 0 ? (double)CompletedFiles / TotalFiles * 100 : 0;
    public double ProgressFraction => TotalFiles > 0 ? (double)CompletedFiles / TotalFiles : 0;
    public string ProgressText => $"{CompletedFiles} / {TotalFiles} files  ·  {ProgressPercentage:0}%";

    public string StatusText => Status switch
    {
        JobStatus.Running => "Running",
        JobStatus.Success => "Success",
        JobStatus.Failed => "Failed",
        JobStatus.Paused => "Paused",
        _ => "Unknown"
    };

    partial void OnCompletedFilesChanged(int value)
    {
        OnPropertyChanged(nameof(ProgressPercentage));
        OnPropertyChanged(nameof(ProgressFraction));
        OnPropertyChanged(nameof(ProgressText));
    }

    partial void OnTotalFilesChanged(int value)
    {
        OnPropertyChanged(nameof(ProgressPercentage));
        OnPropertyChanged(nameof(ProgressFraction));
        OnPropertyChanged(nameof(ProgressText));
    }

    partial void OnStatusChanged(JobStatus value)
    {
        OnPropertyChanged(nameof(StatusText));
    }
}

public partial class JobsPageViewModel : PageViewModel
{
    private readonly BatchProcessClient _batchProcessClient;
    private readonly MainViewModel _mainViewModel;
    private readonly DialogService _dialogService;
    private readonly Dictionary<string, Timer> _pollingTimers = [];

    [ObservableProperty] private int _totalJobs;
    [ObservableProperty] private string _totalRuntime = "0h 0m";
    [ObservableProperty] private int _currentlyRunning;
    [ObservableProperty] private int _successfulJobs;
    [ObservableProperty] private string _successRate = "0%";
    [ObservableProperty] private int _failedJobs;
    [ObservableProperty] private string _failedSummary = "";

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private string _activeFilter = "All";

    [ObservableProperty] private bool _isFilterAll = true;
    [ObservableProperty] private bool _isFilterRunning;
    [ObservableProperty] private bool _isFilterSuccessful;
    [ObservableProperty] private bool _isFilterFailed;
    [ObservableProperty] private bool _isFilterPaused;

    [ObservableProperty] private SortColumn _currentSortColumn = SortColumn.Date;
    [ObservableProperty] private bool _isSortAscending;

    // Sort arrow visibility per column
    [ObservableProperty] private bool _isSortByName;
    [ObservableProperty] private bool _isSortByDescription;
    [ObservableProperty] private bool _isSortByProgress;
    [ObservableProperty] private bool _isSortByStatus;
    [ObservableProperty] private bool _isSortByDate = true;

    [ObservableProperty] private string _sortArrowName = "";
    [ObservableProperty] private string _sortArrowDescription = "";
    [ObservableProperty] private string _sortArrowProgress = "";
    [ObservableProperty] private string _sortArrowStatus = "";
    [ObservableProperty] private string _sortArrowDate = "\u25BC";

    [ObservableProperty] private ObservableCollection<JobItemViewModel> _allJobs = [];
    [ObservableProperty] private ObservableCollection<JobItemViewModel> _filteredJobs = [];

    public JobsPageViewModel(BatchProcessClient batchProcessClient, MainViewModel mainViewModel, DialogService dialogService) : base(ApplicationPageNames.Jobs)
    {
        _batchProcessClient = batchProcessClient;
        _mainViewModel = mainViewModel;
        _dialogService = dialogService;

        // Start empty — no mock data
        UpdateSummaryCards();
        ApplyFilter();
    }

    /// <summary>
    /// Adds a new job to the page and starts polling for its progress
    /// </summary>
    public void AddJob(BatchJobRequest request)
    {
        var jobItem = new JobItemViewModel
        {
            JobId = request.JobId,
            Name = request.JobName,
            Description = request.Description,
            TotalFiles = request.Files.Count,
            CompletedFiles = 0,
            Status = JobStatus.Running,
            DateText = $"Live — started {DateTime.Now:HH:mm}",
            SortIndex = AllJobs.Count,
        };

        AllJobs.Insert(0, jobItem);
        UpdateSummaryCards();
        ApplyFilter();

        // Start polling for progress
        StartPolling(jobItem);
    }

    private void StartPolling(JobItemViewModel jobItem)
    {
        var timer = new Timer(async _ => await PollJobProgress(jobItem), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        _pollingTimers[jobItem.JobId] = timer;
    }

    private async System.Threading.Tasks.Task PollJobProgress(JobItemViewModel jobItem)
    {
        var progress = await _batchProcessClient.GetJobProgressAsync(jobItem.JobId);

        if (progress is null)
            return;

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            jobItem.CompletedFiles = progress.CompletedFiles;
            jobItem.ElapsedMs = progress.ElapsedMs;
            jobItem.Steps = new ObservableCollection<JobStepDetail>(progress.Steps);
            jobItem.Errors = new ObservableCollection<string>(progress.Errors);

            if (progress.IsComplete)
            {
                jobItem.Status = progress.Status switch
                {
                    "Success" => JobStatus.Success,
                    "Failed" => JobStatus.Failed,
                    _ => JobStatus.Failed
                };

                jobItem.DateText = $"Completed {DateTime.Now:MMM dd, yyyy HH:mm}";

                // Stop polling
                StopPolling(jobItem.JobId);

                // Only rebuild the filtered list when status changes (not on every tick)
                UpdateSummaryCards();
                ApplyFilter();
            }
        });
    }

    private void StopPolling(string jobId)
    {
        if (_pollingTimers.Remove(jobId, out var timer))
        {
            timer.Dispose();
        }
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnActiveFilterChanged(string value)
    {
        IsFilterAll = value == "All";
        IsFilterRunning = value == "Running";
        IsFilterSuccessful = value == "Successful";
        IsFilterFailed = value == "Failed";
        IsFilterPaused = value == "Paused";
        ApplyFilter();
    }

    [RelayCommand]
    private void SetFilter(string filter)
    {
        ActiveFilter = filter;
    }

    [RelayCommand]
    private void NewJob()
    {
        // Placeholder for new job creation
    }

    [RelayCommand]
    private async Task OpenJobDetailAsync(JobItemViewModel job)
    {
        var dialog = new JobDetailDialogViewModel();
        dialog.LoadFromJob(job);
        await _dialogService.ShowDialog(_mainViewModel, dialog);
    }

    [RelayCommand]
    private void SortBy(string columnName)
    {
        var column = columnName switch
        {
            "Name" => SortColumn.Name,
            "Description" => SortColumn.Description,
            "Progress" => SortColumn.Progress,
            "Status" => SortColumn.Status,
            "Date" => SortColumn.Date,
            _ => SortColumn.Name
        };

        if (CurrentSortColumn == column)
            IsSortAscending = !IsSortAscending;
        else
        {
            CurrentSortColumn = column;
            IsSortAscending = true;
        }

        UpdateSortIndicators();
        ApplyFilter();
    }

    private void UpdateSortIndicators()
    {
        var arrow = IsSortAscending ? "\u25B2" : "\u25BC";

        IsSortByName = CurrentSortColumn == SortColumn.Name;
        IsSortByDescription = CurrentSortColumn == SortColumn.Description;
        IsSortByProgress = CurrentSortColumn == SortColumn.Progress;
        IsSortByStatus = CurrentSortColumn == SortColumn.Status;
        IsSortByDate = CurrentSortColumn == SortColumn.Date;

        SortArrowName = IsSortByName ? arrow : "";
        SortArrowDescription = IsSortByDescription ? arrow : "";
        SortArrowProgress = IsSortByProgress ? arrow : "";
        SortArrowStatus = IsSortByStatus ? arrow : "";
        SortArrowDate = IsSortByDate ? arrow : "";
    }

    private static bool FuzzyMatch(string text, string search)
    {
        if (string.IsNullOrEmpty(search))
            return true;

        if (text.Contains(search, StringComparison.OrdinalIgnoreCase))
            return true;

        var textLower = text.ToLowerInvariant();
        var searchLower = search.ToLowerInvariant();
        var textIndex = 0;

        foreach (var c in searchLower)
        {
            var found = false;
            while (textIndex < textLower.Length)
            {
                if (textLower[textIndex] == c)
                {
                    textIndex++;
                    found = true;
                    break;
                }
                textIndex++;
            }

            if (!found)
                return false;
        }

        return true;
    }

    private void ApplyFilter()
    {
        var filtered = AllJobs.AsEnumerable();

        // Apply status filter
        if (ActiveFilter != "All")
        {
            var statusFilter = ActiveFilter switch
            {
                "Running" => JobStatus.Running,
                "Successful" => JobStatus.Success,
                "Failed" => JobStatus.Failed,
                "Paused" => JobStatus.Paused,
                _ => (JobStatus?)null
            };

            if (statusFilter.HasValue)
                filtered = filtered.Where(j => j.Status == statusFilter.Value);
        }

        // Apply fuzzy search
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim();
            filtered = filtered.Where(j =>
                FuzzyMatch(j.Name, search) ||
                FuzzyMatch(j.Description, search) ||
                FuzzyMatch(j.StatusText, search) ||
                FuzzyMatch(j.DateText, search) ||
                FuzzyMatch(j.ProgressText, search));
        }

        // Apply sorting
        filtered = CurrentSortColumn switch
        {
            SortColumn.Name => IsSortAscending
                ? filtered.OrderBy(j => j.Name, StringComparer.OrdinalIgnoreCase)
                : filtered.OrderByDescending(j => j.Name, StringComparer.OrdinalIgnoreCase),
            SortColumn.Description => IsSortAscending
                ? filtered.OrderBy(j => j.Description, StringComparer.OrdinalIgnoreCase)
                : filtered.OrderByDescending(j => j.Description, StringComparer.OrdinalIgnoreCase),
            SortColumn.Progress => IsSortAscending
                ? filtered.OrderBy(j => j.ProgressPercentage)
                : filtered.OrderByDescending(j => j.ProgressPercentage),
            SortColumn.Status => IsSortAscending
                ? filtered.OrderBy(j => j.Status)
                : filtered.OrderByDescending(j => j.Status),
            SortColumn.Date => IsSortAscending
                ? filtered.OrderBy(j => j.SortIndex)
                : filtered.OrderByDescending(j => j.SortIndex),
            _ => filtered
        };

        FilteredJobs = new ObservableCollection<JobItemViewModel>(filtered);
    }

    private void UpdateSummaryCards()
    {
        TotalJobs = AllJobs.Count;
        CurrentlyRunning = AllJobs.Count(j => j.Status == JobStatus.Running);
        SuccessfulJobs = AllJobs.Count(j => j.Status == JobStatus.Success);
        FailedJobs = AllJobs.Count(j => j.Status == JobStatus.Failed);

        var pausedCount = AllJobs.Count(j => j.Status == JobStatus.Paused);
        SuccessRate = TotalJobs > 0 ? $"{(double)SuccessfulJobs / TotalJobs * 100:0}% success rate" : "0% success rate";
        FailedSummary = $"{pausedCount} paused, {FailedJobs} error";
    }
}
