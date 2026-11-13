using System;
using AngelSix.Mvvm;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaThemeLab.Minimal.Theme;
using AvaloniaThemeLab.Minimal.ViewModels;
using AvaloniaThemeLab.Minimal.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.Minimal;

public partial class App : Application, IServiceHost
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // After the load, so the theme App.axaml creates is there to register
        Services = BuildServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainWindowViewModel>() };

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(MinimalTheme.Current);
        services.AddSingleton<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
