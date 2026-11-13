using System;
using AngelSix.Mvvm;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.ThemePlayground;

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

#if DEBUG
        // Dev-only: re-parse edited theme previews into the running app; no-op outside a dev checkout
        Development.PreviewHotReload.Start();
#endif

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(DefaultTheme.Current);

        // The window a page's notifications are placed over
        services.AddSingleton<Func<TopLevel?>>(_ => () =>
            (Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);

        services.AddSingleton<GalleryCatalogue>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<ThemeSettingsViewModel>();
        services.AddSingleton(sp => new ThemeSettingsWindowService(
            sp.GetRequiredService<Func<TopLevel?>>(), sp.GetRequiredService<ThemeSettingsViewModel>));
        // The Window page's dialogs and modal page
        services.AddSingleton(sp => new DialogWindowService(sp.GetRequiredService<Func<TopLevel?>>()));

        return services.BuildServiceProvider();
    }
}
