using System.Reflection;
using AngelSix.ThemeEngine;
using Avalonia.Media;

namespace Avalonia.Themes.Prototype.Tests;

/// <summary>
/// Proves the ColourRamp DarkSeed contract on the surface hue: the canvas token is the paper seed
/// in the light palette and exactly the DarkSeed literal in the dark palette, and the whole dark
/// ramp re-centres on that literal rather than mirroring the paper seed. The expected colours are
/// read off the attribute so retuning the seed never breaks these tests.
/// </summary>
public class DarkSeedTests
{
    private static Color DeclaredDarkSeed()
    {
        var attribute = typeof(DefaultTheme).GetProperty(nameof(DefaultTheme.AccentSurface))!
            .GetCustomAttribute<ColourRampAttribute>()!;
        return Color.Parse(attribute.DarkSeed);
    }

    [Fact]
    public void Light_canvas_is_exactly_the_paper_seed()
    {
        var theme = new DefaultTheme();

        Assert.Equal(theme.AccentSurface, theme.SurfaceDefault);
    }

    [Fact]
    public void Dark_canvas_is_exactly_the_dark_seed()
    {
        var theme = new DefaultTheme { IsDark = true };

        Assert.Equal(DeclaredDarkSeed(), theme.SurfaceDefault);
    }

    [Fact]
    public void Effective_seed_member_switches_with_the_palette()
    {
        Assert.Equal(new DefaultTheme().AccentSurface, new DefaultTheme().SurfaceSeed);
        Assert.Equal(DeclaredDarkSeed(), new DefaultTheme { IsDark = true }.SurfaceSeed);
    }

    [Fact]
    public void Dark_ramp_recentres_on_the_dark_seed()
    {
        // The paper seed is grey, so its ramp stages stay grey (R = G = B). A dark ramp merely
        // mirroring the paper seed would be grey too; one that re-centres on a coloured DarkSeed
        // is not. Guard the precondition so a future grey DarkSeed invalidates the test loudly.
        var seed = DeclaredDarkSeed();
        Assert.False(seed.R == seed.G && seed.G == seed.B);

        var light = new DefaultTheme().AccentSurfaceLight2;
        var dark = new DefaultTheme { IsDark = true }.AccentSurfaceLight2;

        Assert.True(light.R == light.G && light.G == light.B);
        Assert.False(dark.R == dark.G && dark.G == dark.B);
    }
}
