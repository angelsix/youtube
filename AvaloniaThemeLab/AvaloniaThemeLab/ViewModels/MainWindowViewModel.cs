using System;
using System.Linq;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>The gallery window: a searchable list of entries and the one being shown.</summary>
public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly GalleryCatalogue _catalogue;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private GalleryEntryViewModel? _selectedEntry;

    public MainWindowViewModel(GalleryCatalogue catalogue)
    {
        _catalogue = catalogue;
        _selectedEntry = catalogue.Entries.FirstOrDefault();
    }

    // Design-time constructor: the XAML previewer materialises the x:DataType and
    // <Design.DataContext> type itself, with no service container to pass. The previewer boots the
    // application (its entry point runs AppBuilder.SetupWithoutStarting), so the real catalogue is
    // reachable there and the gallery shows the full list exactly like the running app; a tool with
    // no application at all gets the seeded sample catalogue instead.
    public MainWindowViewModel()
        : this(Application.Current is App app
            ? app.Services.GetRequiredService<GalleryCatalogue>()
            : GalleryCatalogue.CreateForDesign())
    {
    }

    public GalleryCatalogue Catalogue => _catalogue;

    // Keep the shown entry while it still matches; otherwise show the first match
    partial void OnSearchTextChanged(string value)
    {
        GalleryEntryViewModel? shown = SelectedEntry;
        _catalogue.Filter(value);

        SelectedEntry = shown is not null && _catalogue.Entries.Contains(shown)
            ? shown
            : _catalogue.Entries.FirstOrDefault();
    }
}
