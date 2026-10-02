using System;
using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Default;
using AvaloniaThemeLab.ViewModels;
using AvaloniaThemeLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab;

public partial class App : Application
{
    public IServiceProvider Services { get; } = BuildServices();

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainWindowViewModel>() };

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new ThemeContext(new BrandTheme()));

        // The window a page's notifications are placed over
        services.AddSingleton<Func<TopLevel?>>(_ => () =>
            (Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);

        services.AddSingleton<GalleryCatalogue>();
        services.AddSingleton<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
