using AngelSix.Mvvm;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaThemeLab.StarterTheme.Theme;
using AvaloniaThemeLab.StarterTheme.ViewModels;
using AvaloniaThemeLab.StarterTheme.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.StarterTheme;

public partial class App : Application, IServiceHost
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Register services after XAML loads so AppTheme.Current exists.
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

        services.AddSingleton(AppTheme.Current);
        services.AddTransient<ExampleContentViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
