using System.Globalization;
using System.Reflection;
using AngelSix.ThemeEngine;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Themes.Prototype.Tests;

/// <summary>
/// Pins down WHICH paint tokens may legally drop their <c>Brush</c> suffix in an <see cref="IBrush"/>
/// slot, so the shorthand cleanup cannot be widened past the safe set. Three families behave
/// differently:
/// <list type="bullet">
///   <item><b>Roles</b> (<c>TextDefault</c>, …): the bare extension omits the <c>PropertySuffix</c>
///     override and inherits the base <c>"Brush"</c>, so it reads the SAME <c>…Brush</c> member as the
///     suffixed form — a pure alias.</item>
///   <item><b>Plain</b> (<c>AccentPrimary</c>, …): the bare form reads the <see cref="Color"/> member,
///     a different type that Avalonia refuses to seat in an <see cref="IBrush"/> slot — so the suffix
///     is load-bearing there.</item>
///   <item><b>Overlays</b> (<c>OverlayWeakBrush</c>, …): emit no bare form at all.</item>
/// </list>
/// </summary>
public class ShorthandSafetyTests
{
    private static readonly Type Base = typeof(AccentValueExtension);

    /// <summary>Bare and suffixed ROLE extensions must compose the identical theme-member name.</summary>
    [Theory]
    [InlineData("TextDefault")]
    [InlineData("BackgroundDefault")]
    [InlineData("BorderDefault")]
    [InlineData("HoverBackgroundDefault")]
    [InlineData("PressedBackgroundDefault")]
    [InlineData("DimTextDefault")]
    [InlineData("HoverTextDefault")]
    public void Bare_role_extension_resolves_the_same_member_as_its_brush_twin(string roleName)
    {
        var bare = Activator.CreateInstance(ExtensionType(roleName))!;
        var twin = Activator.CreateInstance(ExtensionType(roleName + "Brush"))!;

        Assert.True(MemberName(bare) == MemberName(twin),
            $"{roleName}: bare form reads '{MemberName(bare)}' but the suffixed form reads '{MemberName(twin)}' — not a safe alias.");
    }

    /// <summary>A bare PLAIN token is a Color, not a brush — so it cannot stand in for the …Brush form.</summary>
    [Theory]
    [InlineData(nameof(DefaultTheme.AccentPrimary))]
    [InlineData(nameof(DefaultTheme.AccentSuccess))]
    [InlineData(nameof(DefaultTheme.AccentWarning))]
    [InlineData(nameof(DefaultTheme.AccentError))]
    [InlineData(nameof(DefaultTheme.AccentInfo))]
    [InlineData(nameof(DefaultTheme.AccentDestructive))]
    [InlineData(nameof(DefaultTheme.AccentSubtle))]
    [InlineData(nameof(DefaultTheme.AccentNeutral))]
    [InlineData(nameof(DefaultTheme.SurfaceDefault))]
    public void Bare_plain_token_reads_a_Color_not_a_Brush(string propertyName)
    {
        var themeType = typeof(DefaultTheme);

        Assert.True(themeType.GetProperty(propertyName)!.PropertyType == typeof(Color),
            $"{propertyName}: expected the bare token to read a Color.");
        Assert.True(themeType.GetProperty(propertyName + "Brush")!.PropertyType == typeof(SolidColorBrush),
            $"{propertyName}Brush: expected the suffixed token to read a SolidColorBrush.");
    }

    /// <summary>
    /// Exercises the EXACT function the runtime converter calls when a bare token lands on a styled
    /// property — <see cref="TypeUtilities.ConvertOrDefault"/> against the target type. A bare plain
    /// token hands the binding a <see cref="Color"/>; this asserts what that becomes in an
    /// <see cref="IBrush"/> slot versus a <see cref="Color"/> slot, which is precisely the distinction
    /// the shorthand rule draws.
    /// </summary>
    [Theory]
    [InlineData(nameof(DefaultTheme.AccentPrimary))]
    [InlineData(nameof(DefaultTheme.SurfaceDefault))]
    public void Converter_path_for_a_bare_plain_token(string propertyName)
    {
        var color = (Color)typeof(DefaultTheme).GetProperty(propertyName)!.GetValue(new DefaultTheme())!;
        var culture = CultureInfo.InvariantCulture;

        // Paint slot: the framework must refuse the Color rather than silently seating something wrong.
        Assert.True(TypeUtilities.ConvertOrDefault(color, typeof(IBrush), culture) is null,
            $"{propertyName}: a Color coerced into an IBrush unexpectedly — the shorthand rule must be revisited.");

        // Color slot: the identity path succeeds, confirming the bare form belongs there.
        Assert.True(TypeUtilities.ConvertOrDefault(color, typeof(Color), culture) is Color c && c == color,
            $"{propertyName}: Color did not pass through its own slot unchanged.");
    }

    /// <summary>
    /// Guards the opacity hazard: a brush carrying sub-1 opacity loses it when read back as a bare
    /// Color. No NON-overlay brush may carry explicit opacity, so a bare read never drops translucency.
    /// Overlays are excluded — they have no bare form and are the only translucent paints.
    /// </summary>
    [Fact]
    public void No_non_overlay_brush_carries_explicit_opacity()
    {
        foreach (var prop in typeof(DefaultTheme).GetProperties())
        {
            if (prop.PropertyType != typeof(SolidColorBrush) || prop.Name.StartsWith("Overlay"))
                continue;

            var brush = (SolidColorBrush)prop.GetValue(new DefaultTheme())!;
            Assert.True(brush.Opacity == 1.0,
                $"{prop.Name} carries opacity {brush.Opacity}; a bare {PropWithoutBrush(prop.Name)} read would drop it.");
        }
    }

    /// <summary>The generated extension type for a token name, living in the emitted Colours namespace.</summary>
    private static Type ExtensionType(string tokenName)
        => typeof(DefaultTheme).Assembly.GetType($"AngelSix.ThemeEngine.Colors.{tokenName}Extension")
           ?? throw new InvalidOperationException($"{tokenName}Extension not found");

    /// <summary>Composes the theme-member name an accent extension resolves, via its virtual parts.</summary>
    private static string MemberName(object extension)
        => Virtual(extension, "PropertyPrefix")
           + "Primary"
           + Virtual(extension, "VariantSuffix")
           + Virtual(extension, "PropertySuffix");

    private static string Virtual(object instance, string propertyName)
        => (string)Base.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(instance)!;

    private static string PropWithoutBrush(string name) => name.EndsWith("Brush") ? name[..^5] : name;
}
