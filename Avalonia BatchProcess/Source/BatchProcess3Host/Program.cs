using Avalonia;
using BatchProcess3.Core.Jobs;
using BatchProcess3.Core.SolidWorks;
using BatchProcess3Host.Services;
using BatchProcess3Host.SolidWorks;
using BatchProcess3Host.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BatchProcess3Host;

sealed class Program
{
    private static CancellationTokenSource _cts = new CancellationTokenSource();

    public static WebApplication? WebApp { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        // Run Kestrel
        var builder = WebApplication.CreateBuilder(args);

        // Inject services
        builder.Services.AddSingleton<JobExecutionService>();
        builder.Services.AddSingleton<MainWindowViewModel>();
        builder.Services.AddSingleton<BatchProcessHost>();

        builder.WebHost.UseUrls($"http://0.0.0.0:{BatchProcessHostUrls.DefaultPort}");

        WebApp = builder.Build();

        // Existing endpoints
        WebApp.MapGet("/", ([FromServices] BatchProcessHost host)
            => $"BatchProcess Host — {host.GetSolidWorksVersion()}");

        WebApp.MapGet(BatchProcessHostUrls.Ping, () => new { Name = "BatchProcess Host", Version = "3.0.0" });
        
        WebApp.MapGet(BatchProcessHostUrls.SolidWorksActiveFileList, ([FromServices] BatchProcessHost host)
            => host.GetActiveFileReferences());

        // Job submission endpoint
        WebApp.MapPost(BatchProcessHostUrls.JobsSubmit, ([FromBody] BatchJobRequest request, [FromServices] JobExecutionService jobService) =>
        {
            return jobService.SubmitJob(request);
        });

        // Job progress endpoint
        WebApp.MapGet("/jobs/{id}/progress", (string id, [FromServices] JobExecutionService jobService) =>
        {
            var progress = jobService.GetProgress(id);
            return progress is not null ? Microsoft.AspNetCore.Http.Results.Ok(progress) : Microsoft.AspNetCore.Http.Results.NotFound();
        });
        
        // Start a UDP discovery listener - respond to broadcast discovery requests from clients
        Task.Run(async () =>
        {
            using var udp = new UdpClient(BatchProcessHostUrls.DiscoveryPort);

            try
            {
                // Infinite loop until cancelled
                while (!_cts.IsCancellationRequested)
                {
                    var result = await udp.ReceiveAsync(_cts.Token);
                    var message = Encoding.UTF8.GetString(result.Buffer);
                    if (message == BatchProcessHostUrls.DiscoveryRequest)
                    {
                        var response = Encoding.UTF8.GetBytes(BatchProcessHostUrls.DiscoveryResponse);
                        await udp.SendAsync(response, result.RemoteEndPoint, _cts.Token);
                    }
                }
            }
            catch (Exception e)
            {
                // ignored
            }
            
        });

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
