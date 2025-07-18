using System;

namespace BatchProcess3.Crash;

public class CrashData
{
    public DateTimeOffset CrashDate { get; set; }
    
    public string Source { get; set; }
    
    public string ErrorMessage { get; set; }
    
    public string StackTrace { get; set; }
}