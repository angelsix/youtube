using AngelSix.ThemeEngine.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Prototype;

namespace Avalonia.Themes.Prototype.Tests;

/// <summary>
/// Guards the calendar against silently rendering unstyled now that its TYPES live in the
/// AngelSix.ThemeEngine.Controls package while its STYLING stays in this theme.
///
/// The engine registers each assembly's control dictionaries at load time, and ThemeStyles merges
/// only those registered for ITS OWN assembly. If the calendar's dictionary ever stopped being
/// registered under the theme's assembly — e.g. the .axaml was moved out, or the reference dropped —
/// the calendar would fall back to Avalonia's built-in theme with no error anywhere. This asserts
/// the one link that must hold: the theme's own merged dictionaries carry the calendar's dictionary.
/// </summary>
public class CalendarViewThemingSmokeTests
{
    private static readonly Uri CalendarDictionaryUri = new("avares://Avalonia.Themes.Prototype/Controls/CalendarView.axaml");

    [Fact]
    public void Theme_still_merges_the_calendar_dictionary_into_its_own_scope()
    {
        var theme = new PrototypeTheme();

        Assert.Contains(theme.Resources.MergedDictionaries,
            d => d is MergeResourceInclude include && include.Source == CalendarDictionaryUri);
    }

    [Fact]
    public void Calendar_types_come_from_the_controls_package_not_this_theme()
    {
        // Documents the intended split: the types are foreign to the theme assembly, proving the
        // styling relationship is "theme styles a foreign type" rather than "theme owns the type".
        Assert.NotEqual(typeof(PrototypeTheme).Assembly, typeof(CalendarView).Assembly);
        Assert.Equal("AngelSix.ThemeEngine.Controls", typeof(CalendarView).Namespace);
    }
}
