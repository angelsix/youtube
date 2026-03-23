using BatchProcess3.Core.Jobs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BatchProcess3Host.Services;

public class JobExecutionState
{
    public string JobId { get; set; } = "";
    public string JobName { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Running";
    public int TotalFiles { get; set; }
    public int CompletedFiles { get; set; }
    public string CurrentFileName { get; set; } = "";
    public string CurrentActionName { get; set; } = "";
    public List<string> Errors { get; set; } = [];
    public List<JobStepDetail> Steps { get; set; } = [];
    public bool IsComplete { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public Stopwatch Stopwatch { get; set; } = Stopwatch.StartNew();
}

public class JobExecutionService
{
    private readonly ConcurrentDictionary<string, JobExecutionState> _jobs = new();
    private readonly Random _random = new();

    public event Action? JobsChanged;

    public IReadOnlyCollection<JobExecutionState> GetAllJobs() => _jobs.Values.ToList();

    public JobExecutionState? GetJobState(string jobId) =>
        _jobs.TryGetValue(jobId, out var state) ? state : null;

    public BatchJobResponse SubmitJob(BatchJobRequest request)
    {
        var state = new JobExecutionState
        {
            JobId = request.JobId,
            JobName = request.JobName,
            Description = request.Description,
            TotalFiles = request.Files.Count,
        };

        _jobs[request.JobId] = state;
        JobsChanged?.Invoke();

        _ = Task.Run(() => ProcessJobAsync(request, state));

        return new BatchJobResponse
        {
            JobId = request.JobId,
            Accepted = true,
            Message = $"Job '{request.JobName}' accepted with {request.Files.Count} files and {request.Actions.Count} actions"
        };
    }

    public BatchJobProgressResponse? GetProgress(string jobId)
    {
        if (!_jobs.TryGetValue(jobId, out var state))
            return null;

        return new BatchJobProgressResponse
        {
            JobId = state.JobId,
            Status = state.Status,
            TotalFiles = state.TotalFiles,
            CompletedFiles = state.CompletedFiles,
            CurrentFileName = state.CurrentFileName,
            CurrentActionName = state.CurrentActionName,
            Errors = [.. state.Errors],
            Steps = [.. state.Steps],
            IsComplete = state.IsComplete,
            ElapsedMs = state.Stopwatch.ElapsedMilliseconds,
        };
    }

    private async Task ProcessJobAsync(BatchJobRequest request, JobExecutionState state)
    {
        var hasErrors = false;

        try
        {
            foreach (var filePath in request.Files)
            {
                var fileName = Path.GetFileName(filePath);
                state.CurrentFileName = fileName;

                // Simulate opening the file
                state.CurrentActionName = "Opening file...";
                JobsChanged?.Invoke();
                var stepWatch = Stopwatch.StartNew();
                await Task.Delay(_random.Next(300, 800));

                state.Steps.Add(new JobStepDetail
                {
                    FileName = fileName,
                    ActionName = "Open File",
                    Status = "Success",
                    DurationMs = stepWatch.ElapsedMilliseconds,
                });

                foreach (var action in request.Actions.OrderBy(a => a.SortOrder))
                {
                    state.CurrentActionName = action.JobName;
                    JobsChanged?.Invoke();

                    stepWatch = Stopwatch.StartNew();
                    await Task.Delay(_random.Next(500, 2000));

                    // ~10% chance of failure
                    if (_random.Next(100) < 10)
                    {
                        var errorMessage = GenerateRandomError(fileName, action.JobName);
                        state.Errors.Add(errorMessage);
                        hasErrors = true;

                        state.Steps.Add(new JobStepDetail
                        {
                            FileName = fileName,
                            ActionName = action.JobName,
                            Status = "Failed",
                            DurationMs = stepWatch.ElapsedMilliseconds,
                            ErrorMessage = errorMessage,
                        });

                        continue;
                    }

                    // Simulate saving if applicable
                    if (action.JobName.Contains("Save", StringComparison.OrdinalIgnoreCase) ||
                        action.JobName.Contains("Export", StringComparison.OrdinalIgnoreCase))
                    {
                        state.CurrentActionName = $"Saving {fileName}...";
                        JobsChanged?.Invoke();
                        await Task.Delay(_random.Next(200, 600));

                        if (_random.Next(100) < 5)
                        {
                            var saveError = $"Failed to save '{fileName}': Disk write error (simulated)";
                            state.Errors.Add(saveError);
                            hasErrors = true;

                            state.Steps.Add(new JobStepDetail
                            {
                                FileName = fileName,
                                ActionName = action.JobName,
                                Status = "Failed",
                                DurationMs = stepWatch.ElapsedMilliseconds,
                                ErrorMessage = saveError,
                            });

                            continue;
                        }
                    }

                    state.Steps.Add(new JobStepDetail
                    {
                        FileName = fileName,
                        ActionName = action.JobName,
                        Status = "Success",
                        DurationMs = stepWatch.ElapsedMilliseconds,
                    });
                }

                // Simulate closing the file
                state.CurrentActionName = "Closing file...";
                JobsChanged?.Invoke();
                stepWatch = Stopwatch.StartNew();
                await Task.Delay(_random.Next(100, 300));

                state.Steps.Add(new JobStepDetail
                {
                    FileName = fileName,
                    ActionName = "Close File",
                    Status = "Success",
                    DurationMs = stepWatch.ElapsedMilliseconds,
                });

                state.CompletedFiles++;
                JobsChanged?.Invoke();
            }

            state.Status = hasErrors ? "Failed" : "Success";
        }
        catch (Exception ex)
        {
            state.Status = "Failed";
            state.Errors.Add($"Unexpected error: {ex.Message}");
        }
        finally
        {
            state.Stopwatch.Stop();
            state.IsComplete = true;
            state.CurrentFileName = "";
            state.CurrentActionName = "";
            JobsChanged?.Invoke();
        }
    }

    private string GenerateRandomError(string fileName, string actionName)
    {
        var errors = new[]
        {
            $"'{fileName}': Access denied while running '{actionName}'",
            $"'{fileName}': File locked by another process during '{actionName}'",
            $"'{fileName}': Timeout waiting for response in '{actionName}'",
            $"'{fileName}': Invalid data format encountered in '{actionName}'",
            $"'{fileName}': Out of memory while processing '{actionName}'",
        };

        return errors[_random.Next(errors.Length)];
    }
}
