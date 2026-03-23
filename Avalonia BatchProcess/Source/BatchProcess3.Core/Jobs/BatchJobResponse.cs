namespace BatchProcess3.Core.Jobs;

public class BatchJobResponse
{
    public string JobId { get; set; } = "";
    public bool Accepted { get; set; }
    public string Message { get; set; } = "";
}
