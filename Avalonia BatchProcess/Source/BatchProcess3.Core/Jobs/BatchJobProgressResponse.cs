namespace BatchProcess3.Core.Jobs;

public class BatchJobProgressResponse
{
    public string JobId { get; set; } = "";
    public string Status { get; set; } = "Running";
    public int TotalFiles { get; set; }
    public int CompletedFiles { get; set; }
    public string CurrentFileName { get; set; } = "";
    public string CurrentActionName { get; set; } = "";
    public List<string> Errors { get; set; } = [];
    public List<JobStepDetail> Steps { get; set; } = [];
    public bool IsComplete { get; set; }
    public long ElapsedMs { get; set; }
}
