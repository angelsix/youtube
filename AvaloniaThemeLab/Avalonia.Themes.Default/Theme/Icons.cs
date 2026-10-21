using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AngelSix.ThemeEngine;
using Avalonia.Controls;
using Avalonia.Media;

namespace Avalonia.Themes.Default;

/// <summary>
/// The theme's icon set as a control: one cell per icon the theme generates, so the gallery
/// can show every glyph the theme ships without anyone keeping a list in step.
/// </summary>
/// <remarks>
/// <para>
/// The cells are built from the ACTIVE theme's icon gallery — the <c>ThemeIconGallery</c> the
/// engine's source generator fills in from <c>Icons/Svg</c> — read by reflection, exactly the way
/// the engine's own <c>{icons:Name}</c> markup extensions read it. So the set is whatever the
/// theme generates: adding a drawing to <c>Icons/Svg</c> adds a cell, and no hand-kept list
/// exists anywhere to fall out of date.
/// </para>
/// <para>
/// The geometry is parsed with <see cref="StreamGeometry.Parse(string)"/>, the same call the
/// engine's markup extensions make, so a cell's glyph is byte-for-byte the glyph the theme
/// draws everywhere else.
/// </para>
/// </remarks>
public class Icons : ItemsControl
{
    /// <summary>Builds the gallery from the active theme's icon set.</summary>
    public Icons()
    {
        ItemsSource = BuildEntries();
    }

    /// <summary>
    /// Reads the active theme's icon gallery and turns every icon it exposes into an entry.
    /// </summary>
    /// <remarks>
    /// The gallery is read the way the engine's markup extensions read it: the active theme's
    /// <c>Icons</c> property, and then that gallery's own public string properties — one per
    /// generated drawing. A property with no path is skipped rather than shown as an empty cell.
    /// </remarks>
    private static IReadOnlyList<IconEntry> BuildEntries()
    {
        // The theme the tokens are read from — the same one {icons:…} resolves against.
        var theme = ThemeContext.Resolver?.Invoke()?.Active;
        var gallery = theme?.GetType()
            .GetProperty("Icons", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            ?.GetValue(theme);

        if (gallery is null)
            return Array.Empty<IconEntry>();

        var entries = new List<IconEntry>();
        foreach (var property in gallery.GetType()
                     .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Where(property => property.PropertyType == typeof(string) && property.CanRead)
                     .OrderBy(property => property.Name, StringComparer.Ordinal))
        {
            if (property.GetValue(gallery) is not string path || string.IsNullOrWhiteSpace(path))
                continue;

            entries.Add(new IconEntry(property.Name, Parse(path)));
        }

        return entries;
    }

    /// <summary>Turns one icon's path into the geometry a <see cref="PathIcon"/> draws.</summary>
    /// <remarks>
    /// A path that will not parse yields an empty geometry rather than throwing: the gallery is a
    /// showcase, and one unreadable drawing must not take the whole set down with it.
    /// </remarks>
    private static Geometry Parse(string path)
    {
        try
        {
            return StreamGeometry.Parse(path);
        }
        catch (Exception)
        {
            return new PathGeometry();
        }
    }
}

/// <summary>One icon in the gallery: its name and the geometry its glyph is drawn from.</summary>
/// <param name="Name">The icon's name — the name of the drawing that generated it.</param>
/// <param name="Geometry">The geometry the glyph is drawn from.</param>
public sealed record IconEntry(string Name, Geometry Geometry);
