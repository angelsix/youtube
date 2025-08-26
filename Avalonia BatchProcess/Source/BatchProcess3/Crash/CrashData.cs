using System;

namespace BatchProcess3.Crash;

public record CrashData(DateTimeOffset CrashDate, string Source, string ErrorMessage, string StackTrace);