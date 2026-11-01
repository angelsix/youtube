using Avalonia;
using Avalonia.Headless;
using AvaloniaThemeLab.Tests;

// One lab App for the whole assembly: booting it discovers the gallery by reflection, which is the
// slow part. Each test restores the theme values it changes and closes the windows it opens.
[assembly: AvaloniaTestApplication(typeof(TestApp))]
[assembly: AvaloniaTestIsolation(AvaloniaTestIsolationLevel.PerAssembly)]

namespace AvaloniaThemeLab.Tests;

/// <summary>Boots the real lab application headless, drawing with Skia and the Inter font it ships with.</summary>
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
