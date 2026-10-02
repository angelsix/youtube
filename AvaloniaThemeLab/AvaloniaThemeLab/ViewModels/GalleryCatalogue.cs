using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;
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
    }

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

        foreach (var controlType in DiscoverThemedControlTypes(app).OrderBy(type => type.Name, StringComparer.Ordinal))
        {
            var pageType = FindPageType(controlType);
            if (pageType is not null)
            {
                entries.Add(specializedPages.TryGetValue(pageType, out var page)
                    ? page
                    : new PageEntryViewModel(controlType, pageType));
            }
            else if (HasPreviewTemplate(app, controlType))
            {
                entries.Add(new ControlEntryViewModel(controlType));
            }
        }

        return entries.OrderBy(entry => entry.Name, StringComparer.Ordinal).ToArray();
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
}
