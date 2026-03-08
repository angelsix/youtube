using System;
using System.Collections.ObjectModel;
using System.Linq;
using BatchProcess3.MainApp;
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

    public JobsPageViewModel() : base(ApplicationPageNames.Jobs)
    {
        LoadMockData();
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

        // Direct substring match (case insensitive)
        if (text.Contains(search, StringComparison.OrdinalIgnoreCase))
            return true;

        // Character-by-character fuzzy match: all search chars appear in order
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

        // Apply fuzzy search across all relevant fields
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

    private void LoadMockData()
    {
        AllJobs =
        [
            new() { Name = "Weekly Export", Description = "Export all CAD drawings to PDF format", TotalFiles = 20, CompletedFiles = 12, Status = JobStatus.Running, DateText = "Live — started 09:42", SortIndex = 0 },
            new() { Name = "Model Backup", Description = "Backup all active models to archive drive", TotalFiles = 50, CompletedFiles = 50, Status = JobStatus.Success, DateText = "Feb 19, 2025 08:15", SortIndex = 1 },
            new() { Name = "Print Batch A3", Description = "Print selected A3 drawings to plotter", TotalFiles = 8, CompletedFiles = 8, Status = JobStatus.Success, DateText = "Feb 18, 2025 17:30", SortIndex = 2 },
            new() { Name = "DXF Conversion", Description = "Convert DWG files to DXF for client delivery", TotalFiles = 35, CompletedFiles = 18, Status = JobStatus.Running, DateText = "Live — started 10:05", SortIndex = 3 },
            new() { Name = "Archive 2024 Q4", Description = "Move completed Q4 files to cold storage", TotalFiles = 120, CompletedFiles = 67, Status = JobStatus.Paused, DateText = "Paused — Feb 17, 2025", SortIndex = 4 },
            new() { Name = "Title Block Update", Description = "Apply new company title block to all sheets", TotalFiles = 15, CompletedFiles = 15, Status = JobStatus.Success, DateText = "Feb 17, 2025 14:22", SortIndex = 5 },
            new() { Name = "Layer Cleanup", Description = "Remove unused layers from legacy drawings", TotalFiles = 40, CompletedFiles = 12, Status = JobStatus.Failed, DateText = "Feb 16, 2025 11:55", SortIndex = 6 },
            new() { Name = "STEP Export Run", Description = "Export 3D models to STEP for manufacturing", TotalFiles = 22, CompletedFiles = 22, Status = JobStatus.Success, DateText = "Feb 15, 2025 09:00", SortIndex = 7 },
            new() { Name = "Revision Stamp", Description = "Apply revision C stamp to all modified sheets", TotalFiles = 9, CompletedFiles = 4, Status = JobStatus.Running, DateText = "Live — started 10:33", SortIndex = 8 },
            new() { Name = "File Rename Batch", Description = "Rename all files to new naming convention", TotalFiles = 60, CompletedFiles = 60, Status = JobStatus.Success, DateText = "Feb 14, 2025 16:45", SortIndex = 9 },
            new() { Name = "Material BOM Export", Description = "Generate BOM reports from all assemblies", TotalFiles = 18, CompletedFiles = 5, Status = JobStatus.Failed, DateText = "Feb 13, 2025 13:10", SortIndex = 10 },
            new() { Name = "Thumbnail Generation", Description = "Generate preview thumbnails for asset library", TotalFiles = 200, CompletedFiles = 140, Status = JobStatus.Paused, DateText = "Paused — Feb 12, 2025", SortIndex = 11 },
        ];

        TotalRuntime = "14h 32m";
        UpdateSummaryCards();
        ApplyFilter();
    }
}
