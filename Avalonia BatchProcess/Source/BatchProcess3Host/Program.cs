using Avalonia;
using BatchProcess3.Core.SolidWorks;
using BatchProcess3Host.SolidWorks;
using BatchProcess3Host.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BatchProcess3Host;

sealed class Program
{
    private static CancellationTokenSource _cts = new CancellationTokenSource();
    
    public static WebApplication? WebApp { get; private set; }
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Run Kestrel
        var builder = WebApplication.CreateBuilder(args);

        // Inject services
        builder.Services.AddSingleton<MainWindowViewModel>();
        builder.Services.AddSingleton<BatchProcessHost>();
        
        builder.WebHost.UseUrls("http://localhost:5000");
        
        WebApp = builder.Build();
        
        WebApp.MapGet("/", async ([FromServices] MainWindowViewModel viewModel, [FromServices]BatchProcessHost host) =>
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                // viewModel.IncrementValueCommand.Execute(null);
                viewModel.Greeting = host.GetSolidWorksVersion();
            });
            
            return $"Hello {viewModel.Greeting}!";
        });
                
        WebApp.MapGet(BatchProcessHostUrls.SolidWorksActiveFileList, ([FromServices]BatchProcessHost host) 
            => host.GetActiveFileReferences());

        // Start kestrel on background thread
        Task.Run(() => WebApp.RunAsync(_cts.Token));

        try
        {
            // Run Avalonia
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            // Close kestrel
            _cts.Cancel();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
