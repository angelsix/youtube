using System;
using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Prototype;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab;

public partial class App : Application
{
    private readonly IServiceProvider _services = BuildServices();

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new ThemeContext(new BrandTheme()));

        return services.BuildServiceProvider();
    }
}
