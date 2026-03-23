namespace BatchProcess3.Core.Jobs;

public class BatchJobRequest
{
    public string JobId { get; set; } = "";
    public string JobName { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Files { get; set; } = [];
    public List<JobActionInfo> Actions { get; set; } = [];
    public bool QuickView { get; set; }
    public bool SaveOnClose { get; set; }
}
