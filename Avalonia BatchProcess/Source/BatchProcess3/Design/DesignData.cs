using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using BatchProcess3.Bootstrap;
using BatchProcess3.ViewModels;
using BatchProcess3.ViewModels.Actions;
using BatchProcess3.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using HomePageViewModel = BatchProcess3.ViewModels.Pages.HomePageViewModel;

namespace BatchProcess3.Design;

public static class DesignData
{
    private static readonly IServiceProvider Services;

    static DesignData()
    {
        var collection = new ServiceCollection();
        
        // Register the same services as the application
        Bootstrapper.RegisterCommonServices(collection);
        
        // Register design-time only services
        
        Services = collection.BuildServiceProvider();
    }
    
    public static HomePageViewModel HomePageViewModel => Services.GetRequiredService<HomePageViewModel>();
    
    public static MainViewModel MainViewModel => Services.GetRequiredService<MainViewModel>();
    
    public static ActionsPageViewModel ActionsPageViewModel => Services.GetRequiredService<ActionsPageViewModel>();

    public static ProcessPageViewModel ProcessPageViewModel => Services.GetRequiredService<ProcessPageViewModel>();
    
    public static SettingsPageViewModel SettingsPageViewModel => Services.GetRequiredService<SettingsPageViewModel>();

    public static ActionPrintSettingsViewModel ActionPrintSettingsViewModel => Services.GetRequiredService<ActionPrintSettingsViewModel>();

    public static ConfirmDialogViewModel ConfirmDialogViewModel => Services.GetRequiredService<ConfirmDialogViewModel>();
    
    public static ErrorViewModel ErrorViewModel => Services.GetRequiredService<ErrorViewModel>();
}