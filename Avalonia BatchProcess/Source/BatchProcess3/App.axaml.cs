using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using BatchProcess3.Data;
using BatchProcess3.Factories;
using BatchProcess3.Services;
using BatchProcess3.ViewModels;
using BatchProcess3.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

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
         collection.AddSingleton<MainViewModel>();
         collection.AddTransient<ActionsPageViewModel>();
         collection.AddTransient<HistoryPageViewModel>();
         collection.AddTransient<HomePageViewModel>();
         collection.AddTransient<MacrosPageViewModel>();
         collection.AddTransient<ProcessPageViewModel>();
         collection.AddTransient<ReporterPageViewModel>();
         collection.AddTransient<SettingsPageViewModel>();
        
         collection.AddSingleton<Func<Type, PageViewModel>>(x => type => type switch
         {
             _ when type == typeof(HomePageViewModel) => x.GetRequiredService<HomePageViewModel>(),
             _ when type == typeof(ProcessPageViewModel) => x.GetRequiredService<ProcessPageViewModel>(),
             _ when type == typeof(MacrosPageViewModel) => x.GetRequiredService<MacrosPageViewModel>(),
             _ when type == typeof(ActionsPageViewModel) => x.GetRequiredService<ActionsPageViewModel>(),
             _ when type == typeof(ReporterPageViewModel) => x.GetRequiredService<ReporterPageViewModel>(),
             _ when type == typeof(HistoryPageViewModel) => x.GetRequiredService<HistoryPageViewModel>(),
             _ when type == typeof(SettingsPageViewModel) => x.GetRequiredService<SettingsPageViewModel>(),
             _ => throw new InvalidOperationException($"Page of type {type?.FullName} has no view model"),
         });
        
         collection.AddSingleton<PageFactory>();
         collection.AddSingleton<DialogService>();

         collection.AddTransient<PrinterService>();

         // Database services
         collection.AddTransient<ApplicationDbContext>();
         collection.AddTransient<DatabaseService>();
         collection.AddSingleton<Func<DatabaseService>>(x => x.GetRequiredService<DatabaseService>);
         collection.AddSingleton<DatabaseFactory>();

         // TopLevel provider
         collection.AddSingleton<Func<TopLevel?>>(x => () =>
         {
             if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime topWindow)
                 return TopLevel.GetTopLevel(topWindow.MainWindow);
             else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
                 return TopLevel.GetTopLevel(singleViewPlatform.MainView);

             return null;
         });

         var services = collection.BuildServiceProvider();
            
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext  = services.GetRequiredService<MainViewModel>()
            };

            // new ErrorWindow { DataContext = new  ErrorViewModel() }.Show();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView()
            {
                DataContext = services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}