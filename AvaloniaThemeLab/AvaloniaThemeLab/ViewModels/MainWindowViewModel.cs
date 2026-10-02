using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>The gallery window: a searchable list of entries and the one being shown.</summary>
public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly GalleryCatalogue _catalogue;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private GalleryEntryViewModel? _selectedEntry;

    public GalleryCatalogue Catalogue => _catalogue;

    public MainWindowViewModel(GalleryCatalogue catalogue)
    {
        _catalogue = catalogue;
        _selectedEntry = catalogue.Entries.FirstOrDefault();
    }

    // Keep the shown entry while it still matches; otherwise show the first match
    partial void OnSearchTextChanged(string value)
    {
        var shown = SelectedEntry;
        _catalogue.Filter(value);
        SelectedEntry = shown is not null && _catalogue.Entries.Contains(shown)
            ? shown
            : _catalogue.Entries.FirstOrDefault();
    }
}
