namespace BatchProcess3.Core.Jobs;

public class JobStepDetail
{
    public string FileName { get; set; } = "";
    public string ActionName { get; set; } = "";
    public string Status { get; set; } = "Running";
    public long DurationMs { get; set; }
    public string ErrorMessage { get; set; } = "";
}
