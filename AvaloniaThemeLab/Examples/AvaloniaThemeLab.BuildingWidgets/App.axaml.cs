using System;
using AngelSix.Mvvm;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaThemeLab.BuildingWidgets.Theme;
using AvaloniaThemeLab.BuildingWidgets.ViewModels;
using AvaloniaThemeLab.BuildingWidgets.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.BuildingWidgets;

public partial class App : Application, IServiceHost
{
    #region Fields

    private ThemeSettingsWindow? _settingsWindow;

    #endregion Fields

    #region Properties

    public IServiceProvider Services { get; private set; } = null!;

    #endregion Properties

    #region Methods

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Services = BuildServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainWindowViewModel>() };

        base.OnFrameworkInitializationCompleted();
    }

    private IServiceProvider BuildServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton(PrototypeTheme.Current);
        services.AddSingleton<Action>(OpenSettingsWindow);

        services.AddTransient<ExampleContentViewModel>();
        services.AddTransient<TopMenuViewModel>();
        services.AddTransient<SideNavViewModel>();
        services.AddTransient<AppShellViewModel>();
        services.AddTransient<MoonWidgetViewModel>();
        services.AddTransient<ModeToggleViewModel>();
        services.AddTransient<ThemeSettingsViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }

    private void OpenSettingsWindow()
    {
        if (_settingsWindow is { IsVisible: true })
        {
            _settingsWindow.Activate();
            return;
        }

        _settingsWindow = new ThemeSettingsWindow
        {
            DataContext = Services.GetRequiredService<ThemeSettingsViewModel>(),
        };
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;

        if ((ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow is { } owner)
            _settingsWindow.Show(owner);
        else
            _settingsWindow.Show();
    }

    #endregion Methods
}
