namespace BatchProcess3.Core.Jobs;

public class JobActionInfo
{
    public string ActionId { get; set; } = "";
    public string JobName { get; set; } = "";
    public string Description { get; set; } = "";
    public int SortOrder { get; set; }
}
