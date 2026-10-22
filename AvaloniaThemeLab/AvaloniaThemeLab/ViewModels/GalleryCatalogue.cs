using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;
using Avalonia.Themes.Default;
using AvaloniaThemeLab.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>Discovers themed controls and their matching gallery pages from the loaded theme.</summary>
public sealed class GalleryCatalogue
{
    private readonly IReadOnlyList<GalleryEntryViewModel> _allEntries;

    public GalleryCatalogue(IServiceProvider services)
    {
        var app = Application.Current ?? throw new InvalidOperationException("The gallery catalogue requires the Avalonia application to be initialized.");
        var specializedPages = DiscoverSpecializedPages(services);
        _allEntries = DiscoverEntries(app, specializedPages);
        Entries = new ObservableCollection<GalleryEntryViewModel>(_allEntries);
        RegisterMissingPreviewTemplate(app);
    }

    // Seeded entries only: the designer cannot run discovery (no service container, no running
    // application), so a design-time view model builds its catalogue through this door instead.
    private GalleryCatalogue(IReadOnlyList<GalleryEntryViewModel> entries)
    {
        _allEntries = entries;
        Entries = new ObservableCollection<GalleryEntryViewModel>(entries);
    }

    /// <summary>A small fixed catalogue for design tools: enough variety to exercise both ways a
    /// page renders (theme preview template and hand-written page) without discovering anything.</summary>
    public static GalleryCatalogue CreateForDesign() => new([
        new ControlEntryViewModel(typeof(Avalonia.Controls.Button)),
        new ControlEntryViewModel(typeof(Avalonia.Controls.Carousel)),
        new ControlEntryViewModel(typeof(Avalonia.Controls.TextBox)),
        new PageEntryViewModel(typeof(Avalonia.Controls.Label), typeof(Pages.LabelPage)),
        new PageEntryViewModel(typeof(Avalonia.Controls.Separator), typeof(Pages.SeparatorPage)),
    ]);

    /// <summary>The catalogue entries currently shown by the main window.</summary>
    public ObservableCollection<GalleryEntryViewModel> Entries { get; }

    public void Filter(string? query)
    {
        var matches = string.IsNullOrWhiteSpace(query)
            ? _allEntries
            : _allEntries.Where(entry => entry.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();

        Entries.Clear();
        foreach (var match in matches)
            Entries.Add(match);
    }

    private static IReadOnlyList<GalleryEntryViewModel> DiscoverEntries(
        Application app,
        IReadOnlyDictionary<Type, PageEntryViewModel> specializedPages)
    {
        var entries = new List<GalleryEntryViewModel>();
        var mistakes = new List<string>();

        foreach (var controlType in DiscoverThemedControlTypes(app).OrderBy(type => type.Name, StringComparer.Ordinal))
        {
            // The theme's {Name}Preview template is the single source of truth for what a control
            // looks like, so the gallery prefers it; a hand-written page exists only where the
            // theme supplies no preview (or for the few pages that demo more than appearance).
            if (HasPreviewTemplate(app, controlType))
            {
                entries.Add(new ControlEntryViewModel(controlType));
            }
            else if (FindPageType(controlType) is { } pageType)
            {
                entries.Add(specializedPages.TryGetValue(pageType, out var page)
                    ? page
                    : new PageEntryViewModel(controlType, pageType));
            }
            else if (HasShownInMarker(app, controlType) || HasExcludedMarker(app, controlType))
            {
                // DELIBERATELY excluded, and the theme file says so: a {Name}PreviewShownIn marker
                // names the entry that shows this control, a {Name}PreviewExcluded marker says it
                // is shown nowhere and cannot have an entry of its own. The omission is declared,
                // so nothing is dropped silently.
            }
            else
            {
                // A MISTAKE: the control has a theme, so it is a control the gallery should show,
                // but it has neither a preview nor a declaration of why it has none. It used to
                // vanish SILENTLY, and it looked exactly like a deliberately folded-in control.
                // It is surfaced now: a flagged entry in the sidebar, and a warning on stderr.
                var name = controlType.FullName ?? controlType.Name;
                mistakes.Add(name);
                entries.Add(new MissingPreviewEntryViewModel(controlType));
            }
        }

        var ordered = entries.OrderBy(entry => entry.Name, StringComparer.Ordinal).ToArray();
        ReportMistakes(mistakes, ordered);
        return ordered;
    }

    /// <summary>
    /// Says LOUDLY, on stderr and in the debug output, which themed controls the gallery cannot
    /// show and why. A <c>Debug.Fail</c> would be louder still, but it kills the process on a
    /// Debug build — and the lab is a shared, running application, so a missing marker must never
    /// take the app down. The flagged sidebar entry and this warning are the two loud channels.
    /// </summary>
    private static void ReportMistakes(IReadOnlyList<string> mistakes, IReadOnlyList<GalleryEntryViewModel> entries)
    {
        if (mistakes.Count == 0)
            return;

        foreach (var mistake in mistakes)
        {
            Console.Error.WriteLine($"[gallery] MISSING PREVIEW: {mistake} has a ControlTheme but no " +
                                    "{Name}Preview template and no {Name}PreviewShownIn / {Name}PreviewExcluded marker. " +
                                    "It is listed in the gallery flagged as missing a preview.");
        }

        var message =
            $"[gallery] {mistakes.Count} themed control(s) have NO gallery preview and NO marker declaring why, " +
            "so they vanish from the gallery silently: " + string.Join(", ", mistakes) + ". " +
            $"Each one needs a marker in its own theme file: " +
            $"<default:PreviewShownIn x:Key=\"<Name>PreviewShownIn\" ShownIn=\"{{x:Type <host>}}\" Reason=\"…\" /> " +
            "(shown inside another entry) or <default:PreviewExcluded x:Key=\"<Name>PreviewExcluded\" Reason=\"…\" /> " +
            "(shown nowhere, and cannot have an entry of its own).";

        Console.Error.WriteLine(message);
        Debug.WriteLine(message);
    }

    private static IEnumerable<Type> DiscoverThemedControlTypes(Application app) =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .SelectMany(GetLoadableTypes)
            .Where(type => type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters)
            .Where(type => typeof(Control).IsAssignableFrom(type))
            .Where(type => app.TryFindResource(type, app.ActualThemeVariant, out var resource) && resource is ControlTheme)
            .Distinct();

    private static Type[] GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>().ToArray();
        }
    }

    private static IReadOnlyDictionary<Type, PageEntryViewModel> DiscoverSpecializedPages(IServiceProvider services)
    {
        var assembly = typeof(PageEntryViewModel).Assembly;
        return GetLoadableTypes(assembly)
            .Where(type => type.IsClass && !type.IsAbstract && type != typeof(PageEntryViewModel))
            .Where(type => typeof(PageEntryViewModel).IsAssignableFrom(type))
            .Select(type => (PageEntryViewModel)ActivatorUtilities.CreateInstance(services, type))
            .ToDictionary(page => page.PageType);
    }

    private static Type? FindPageType(Type controlType)
    {
        var pageName = $"{controlType.Name}Page";
        var page = typeof(AutoCompleteBoxPage).Assembly.GetType($"{typeof(AutoCompleteBoxPage).Namespace}.{pageName}");
        return page is not null && typeof(Control).IsAssignableFrom(page) ? page : null;
    }

    private static bool HasPreviewTemplate(Application app, Type controlType) =>
        app.TryFindResource($"{controlType.Name}Preview", app.ActualThemeVariant, out var resource) && resource is IDataTemplate;

    /// <summary>
    /// Gives a FLAGGED row something to open. Without this the page host has no template for the
    /// flagged entry and the click shows an empty area — the mistake would then be visible only in
    /// the sidebar's name. The page says what is missing and what to add.
    /// </summary>
    private static void RegisterMissingPreviewTemplate(Application app)
    {
        if (app.DataTemplates.Any(template => template is MissingPreviewTemplate))
            return;
        app.DataTemplates.Add(new MissingPreviewTemplate());
    }

    /// <summary>
    /// Whether the control's theme file declares it as shown inside another entry. The marker is
    /// the theme's own record of the exclusion: <c>{Name}PreviewShownIn</c>.
    /// </summary>
    private static bool HasShownInMarker(Application app, Type controlType) =>
        app.TryFindResource($"{controlType.Name}PreviewShownIn", app.ActualThemeVariant, out var resource) &&
        resource is PreviewShownIn;

    /// <summary>
    /// Whether the control's theme file declares that it can have no gallery entry at all (a
    /// TopLevel, a popup host, an adorner layer): <c>{Name}PreviewExcluded</c>.
    /// </summary>
    private static bool HasExcludedMarker(Application app, Type controlType) =>
        app.TryFindResource($"{controlType.Name}PreviewExcluded", app.ActualThemeVariant, out var resource) &&
        resource is PreviewExcluded;
}

/// <summary>
/// A themed control the gallery CANNOT show, because the theme gives it no preview template and
/// declares no reason for the omission. It is listed so the omission is visible, not silent.
/// </summary>
/// <remarks>
/// The name carries the flag, so the sidebar row reads as the mistake it is and a search for
/// "missing preview" finds it. The row is deliberately NOT a <c>ControlEntryViewModel</c>: there
/// is no preview template to render, so it must not claim one.
/// </remarks>
public sealed class MissingPreviewEntryViewModel : GalleryEntryViewModel
{
    /// <summary>The words every flagged row carries, so a reviewer can search for them.</summary>
    public const string Flag = "missing preview";

    public MissingPreviewEntryViewModel(Type controlType)
        : base($"{controlType.Name} — {Flag}")
    {
        ControlType = controlType;
    }

    /// <summary>The control whose preview is missing.</summary>
    public Type ControlType { get; }

    /// <summary>
    /// What the page host shows when this row is selected: the app has no template for a flagged
    /// row, so the content falls back to this text.
    /// </summary>
    public override string ToString() =>
        $"MISSING PREVIEW: {ControlType.FullName ?? ControlType.Name} has a ControlTheme but no " +
        "{Name}Preview template, and its theme file declares no {Name}PreviewShownIn or " +
        "{Name}PreviewExcluded marker. It therefore has no gallery entry of its own. Add the marker " +
        "to its theme file to make the exclusion deliberate.";
}

/// <summary>
/// The template a FLAGGED gallery row opens: the page that says what is missing. It is registered
/// on the application, so the page host finds it for the flagged entry without the sidebar's own
/// template having to know about it.
/// </summary>
public sealed class MissingPreviewTemplate : IDataTemplate
{
    public Control Build(object? param) => new Pages.MissingPreviewPage();

    public bool Match(object? data) => data is MissingPreviewEntryViewModel;
}
