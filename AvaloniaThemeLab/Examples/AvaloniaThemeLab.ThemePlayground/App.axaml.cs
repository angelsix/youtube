using System;
using AngelSix.Mvvm;
using AngelSix.ThemeEngine;
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
    private ThemeContext? _themeContext;
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var active = ThemeContext.Resolver?.Invoke();
        _themeContext = ReferenceEquals(active?.Active, DefaultTheme.Current)
            ? active : new ThemeContext(DefaultTheme.Current);

        // After the load, so the theme App.axaml creates is there to register
        Services = BuildServices();
        DataTemplates.Add(Services.GetRequiredService<ViewLocator>());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainWindowViewModel>() };
            desktop.Exit += (_, _) =>
            {
                (_themeContext as IDisposable)?.Dispose();
                (Services as IDisposable)?.Dispose();
            };
        }

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
        services.AddSingleton<IThemeSettingsWindowService>(sp => new ThemeSettingsWindowService(
            sp.GetRequiredService<Func<TopLevel?>>(), sp.GetRequiredService<ThemeSettingsViewModel>));
        // The Window page's dialogs and modal page
        services.AddSingleton<IDialogWindowService>(sp => new DialogWindowService(sp.GetRequiredService<Func<TopLevel?>>()));
        services.AddTransient<IModalNavigationService, ModalNavigationService>();
        services.AddTransient<WindowPageViewModel>();
        services.AddTransient<Pages.WindowPage>(sp => new Pages.WindowPage
        {
            DataContext = sp.GetRequiredService<WindowPageViewModel>(),
        });
        services.AddSingleton(sp => new ViewLocator(() => sp.GetRequiredService<Pages.WindowPage>()));
        services.AddSingleton<PageEntryViewModel>(sp => sp.GetRequiredService<WindowPageViewModel>());

        return services.BuildServiceProvider();
    }
}
