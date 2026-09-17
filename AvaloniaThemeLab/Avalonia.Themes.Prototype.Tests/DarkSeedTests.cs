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
        // Both seeds are greys, so "is the stage coloured?" cannot tell the two ramps apart — it
        // only worked while the DarkSeed happened to be a colour. What still separates a ramp that
        // re-centres from one mirroring the paper seed is where its stages sit, so measure that:
        // every dark stage must land nearer the DarkSeed than the paper seed, and vice versa.
        var paperSeed = new DefaultTheme().AccentSurface;
        var darkSeed = DeclaredDarkSeed();

        var light = new DefaultTheme();
        var dark = new DefaultTheme { IsDark = true };

        foreach (var stage in new[] { dark.AccentSurfaceLight2, dark.AccentSurfaceDark2 })
            Assert.True(Distance(stage, darkSeed) < Distance(stage, paperSeed),
                $"Dark stage {stage} sits nearer the paper seed {paperSeed} than the dark seed {darkSeed}.");

        foreach (var stage in new[] { light.AccentSurfaceLight2, light.AccentSurfaceDark2 })
            Assert.True(Distance(stage, paperSeed) < Distance(stage, darkSeed),
                $"Light stage {stage} sits nearer the dark seed {darkSeed} than the paper seed {paperSeed}.");
    }

    /// <summary>
    /// Channel-sum distance between two colours. Crude on purpose: the ramps here are far enough
    /// apart that anything more elaborate would only obscure what the assertion is claiming.
    /// </summary>
    private static int Distance(Color a, Color b)
        => Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);
}
