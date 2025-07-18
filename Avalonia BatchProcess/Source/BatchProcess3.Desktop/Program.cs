using Avalonia;
using BatchProcess3.Services;
using BatchProcess3.ViewModels;
using BatchProcess3.Views;
using System;
using System.Diagnostics;

namespace BatchProcess3.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            // Get previous crash data if any
            var lastCrash = CrashService.GetCrashData();
            
            // Write a crash log
            CrashService.SetCrashData(ex);

            // If we previously crashed in under 10 seconds, don't re-open
            if (lastCrash == null || lastCrash.CrashDate < DateTimeOffset.UtcNow - TimeSpan.FromSeconds(10))
            {
                // Restart application
                try { Process.Start(typeof(Program).Assembly.Location.Replace(".dll", ".exe")); }
                catch
                {
                    //Ignored
                }
            }
        }
    }
    
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}