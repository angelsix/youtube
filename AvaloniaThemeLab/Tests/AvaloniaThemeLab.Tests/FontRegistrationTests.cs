using AngelSix.Themes.Default;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace AvaloniaThemeLab.Tests;

/// <summary>The theme registers its own fonts: no AppBuilder font call is needed for Inter to resolve.</summary>
public sealed class FontRegistrationTests
{
    [AvaloniaFact]
    public void The_theme_font_resolves_to_real_Inter_not_the_fallback()
    {
        // TestApp boots with no .WithInterFont(); the theme's FontFamily (ThemeFonts.Inter) must
        // have registered the embedded collection on its own.
        var theme = TestTheme.With(null);
        var family = theme.FontFamily;

        Assert.True(FontManager.Current.TryGetGlyphTypeface(new Typeface(family), out var glyph),
            "The theme's font family resolved no glyph typeface at all.");

        // The headless platform's fallback font is not Inter; if registration had not happened the
        // family would fall back to it, so the resolved family name is the proof.
        Assert.Equal("Inter", glyph!.FamilyName);
    }

    [AvaloniaFact]
    public void The_typed_choice_and_the_string_form_name_the_same_collection()
    {
        var typed = AngelSix.ThemeEngine.Fonts.ThemeFonts.Inter;
        var fromString = new FontFamily("fonts:AngelSix.Inter#Inter, $Default");

        Assert.Equal(fromString.Key, typed.Key);
        Assert.True(FontManager.Current.TryGetGlyphTypeface(new Typeface(fromString), out var glyph));
        Assert.Equal("Inter", glyph!.FamilyName);
    }

    [AvaloniaFact]
    public void An_unregistered_collection_falls_back_so_the_inter_assertion_means_registration()
    {
        // The refusal case: the same shape of family, but a key nothing registered. It must NOT
        // resolve to Inter — if it did, the tests above would prove nothing.
        var unregistered = new FontFamily("fonts:AngelSix.NeverRegistered#Inter, $Default");

        Assert.True(FontManager.Current.TryGetGlyphTypeface(new Typeface(unregistered), out var glyph));
        Assert.NotEqual("Inter", glyph!.FamilyName);
    }
}