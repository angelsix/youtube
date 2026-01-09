using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using BatchProcess3.Actions;
using BatchProcess3.Bootstrap;
using BatchProcess3.Crash;
using BatchProcess3.DataStorage;
using BatchProcess3.Dialog;
using BatchProcess3.MainApp;
using BatchProcess3.Printer;
using BatchProcess3.SolidWorks;
using BatchProcess3.ViewModels;
using BatchProcess3.ViewModels.Pages;
using BatchProcess3.Views;
using BatchProcess3.Views.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

[assembly: XmlnsDefinition("https://github.com/avaloniaui", "BatchProcess3.Controls")]

namespace BatchProcess3;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
         var collection = new ServiceCollection();
         
         // Inject common services
         Bootstrapper.RegisterCommonServices(collection);
        
         var services = collection.BuildServiceProvider();
            
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext  = services.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView()
            {
                DataContext = services.GetRequiredService<MainViewModel>()
            };
        }

        // Get last crash data
        var lastCrash = CrashService.GetCrashData();

        // If we crashed the last time...
        if (lastCrash != null)
        {
            new ErrorWindow
            {
                DataContext = new ErrorViewModel
                {
                    Title = lastCrash.ErrorMessage,
                    Description = $"BatchProcess crashed at '{lastCrash.Source}'\r\n" +
                                  $"with the following error:\r\n\r\n" +
                                  $"{lastCrash.ErrorMessage}.\r\n\r\n" +
                                  $"Stack Trace:\r\n{lastCrash.StackTrace}"
                }
            }.Show();
            
            // Don't delete error log for 10 seconds
            Task.Delay(10000).ContinueWith(_ => CrashService.ClearCrashData());
        }

        base.OnFrameworkInitializationCompleted();
    }
}