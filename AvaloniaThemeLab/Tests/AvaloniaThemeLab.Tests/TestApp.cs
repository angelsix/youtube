using System;
using System.Linq;
using AngelSix.Themes.Default;
using Avalonia.Threading;
using Avalonia;
using Avalonia.Headless;
using AvaloniaThemeLab.Tests;

// One lab App for the whole assembly: booting it discovers the gallery by reflection, which is the slow part.
[assembly: AvaloniaTestApplication(typeof(TestApp))]
[assembly: AvaloniaTestIsolation(AvaloniaTestIsolationLevel.PerAssembly)]

// Every test shares the app's one theme, so tests run one at a time and each leaves the theme at its defaults
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]
[assembly: ResetThemeAfterEachTest]

namespace AvaloniaThemeLab.Tests;

/// <summary>Boots the real playground application headless, drawing with Skia and the Inter font it ships with.</summary>
public static class TestApp
{
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UseSkia()
            .WithInterFont()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
    }
}

/// <summary>The app's one theme, as tests configure it.</summary>
public static class TestTheme
{
    public static DefaultTheme Current => Application.Current!.Styles.OfType<DefaultTheme>().Single();

    /// <summary>Puts the app's theme back to its defaults, then applies <paramref name="configure"/>.</summary>
    public static DefaultTheme With(Action<DefaultTheme>? configure)
    {
        var theme = Current;
        theme.IsDark = false;
        theme.ResetToDefaults();
        configure?.Invoke(theme);
        Dispatcher.UIThread.RunJobs();
        return theme;
    }
}

/// <summary>Puts the app's theme back to its defaults after every test.</summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ResetThemeAfterEachTestAttribute : Xunit.v3.BeforeAfterTestAttribute
{
    public override void After(System.Reflection.MethodInfo methodUnderTest, Xunit.v3.IXunitTest test) =>
        Dispatcher.UIThread.Invoke(() => TestTheme.With(null));
}
