using Avalonia;
using BatchProcess3.Crash;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Velopack;

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
            VelopackApp.Build().Run();
            _ = UpdateMyApp();
            
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            // Get previous crash data if any
            var lastCrash = CrashService.GetCrashData();
            
            // Write a crash log
            if (CrashService.SetCrashData(ex))
            {
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
    }
    
    private static async Task UpdateMyApp()
    {
        return;
        
        var mgr = new UpdateManager("https://the.place/you-host/updates");

        // check for new version
        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
            return; // no update available

        // download new version
        await mgr.DownloadUpdatesAsync(newVersion);

        // install new version and restart app
        mgr.ApplyUpdatesAndRestart(newVersion);
    }
    
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}