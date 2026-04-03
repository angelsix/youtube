namespace BatchProcess3.Core.SolidWorks;

public static class BatchProcessHostUrls
{
    public static string SolidWorksActiveFileList = "/solidworks/active/list";
    public static string JobsSubmit = "/jobs/submit";
    public static string JobsProgress = "/jobs/{0}/progress";
    public static string Ping = "/ping";

    public const int DefaultPort = 5100;
    public const int DiscoveryPort = 5101;
    public const string DiscoveryRequest = "BATCHPROCESS_DISCOVERY_REQUEST";
    public const string DiscoveryResponse = "BATCHPROCESS_DISCOVERY_RESPONSE";
}