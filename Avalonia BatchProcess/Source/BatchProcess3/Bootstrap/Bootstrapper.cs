using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using BatchProcess3.Actions;
using BatchProcess3.DataStorage;
using BatchProcess3.Design;
using BatchProcess3.Dialog;
using BatchProcess3.MainApp;
using BatchProcess3.Printer;
using BatchProcess3.SolidWorks;
using BatchProcess3.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BatchProcess3.Bootstrap;

public static class Bootstrapper
{
    public static void RegisterCommonServices(IServiceCollection collection)
    {
        // Singleton Services
         collection.AddSingleton<MainViewModel>();
         collection.AddSingleton<HomePageViewModel>();
         collection.AddSingleton<DialogService>();

         // Page Factory Callback
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
        
         // Page Factory
         collection.AddSingleton<PageFactory>();
         
         // Transient Services
         collection.AddTransient<ActionsPageViewModel>();
         collection.AddTransient<HistoryPageViewModel>();
         collection.AddTransient<MacrosPageViewModel>();
         collection.AddTransient<ProcessPageViewModel>();
         collection.AddTransient<ReporterPageViewModel>();
         collection.AddTransient<SettingsPageViewModel>();
         collection.AddTransient<ActionService>();
         collection.AddTransient<PrinterService>();
         collection.AddTransient<BatchProcessClient>();

         collection.AddTransient<ActionPrintSettingsViewModel>();
         collection.AddTransient<ConfirmDialogViewModel>();
         collection.AddTransient<ErrorViewModel>();
         
         // Database services
         collection.AddTransient<ApplicationDbContext>();
         collection.AddTransient<DatabaseService>();
         collection.AddSingleton<Func<DatabaseService>>(x => x.GetRequiredService<DatabaseService>);
         collection.AddSingleton<DatabaseFactory>();
    }
}