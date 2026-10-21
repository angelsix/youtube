using System;
using System.Linq;
using AngelSix.ThemeEngine.Generated;
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

    // What the page host ACTUALLY shows. Normally mirrors SelectedEntry, but the view holds it back
    // while a navigating click is still in flight (the sidebar selects on press, so SelectedEntry
    // flips mid-click); the view calls ShowSelectedEntry() once that click's release has fully
    // routed, so the incoming page is never born into a still-propagating release.
    [ObservableProperty]
    private GalleryEntryViewModel? _shownEntry;

    // The app's one accent (task 102): the header picker's selection, styled into every preview.
    // Defaults to Primary — a commonly used accent — rather than the dark Neutral the picker fell
    // back to when nothing was pinned.
    [ObservableProperty]
    private AccentKind _selectedAccent = AccentKind.Primary;

    public MainWindowViewModel(GalleryCatalogue catalogue)
    {
        _catalogue = catalogue;
        _selectedEntry = catalogue.Entries.FirstOrDefault();
        // The first entry is shown straight away: no click is in flight, so the page host mirrors it.
        _shownEntry = _selectedEntry;
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

    // Armed while a navigating click on the sidebar is in flight (its press has landed, its release
    // has not yet routed). While armed, the SelectedEntry flip that press causes does NOT reach
    // ShownEntry, so the incoming page is not born until the release has fully propagated. The view
    // arms it on the press (ArmDeferredReveal) and disarms + reveals it posted past the release
    // (CompleteDeferredReveal). Arming stores the entry that WAS shown, so a later unrelated
    // selection (keyboard, search) is recognised as not belonging to the click and mirrors at once.
    private GalleryEntryViewModel? _revealedArmedFrom;

    // Production default: the page is revealed only AFTER the navigating click's release has routed
    // (the whole point of the ShownEntry indirection). Flipped off solely by the headless proof to
    // replay the LEGACY reveal-eagerly-on-selection behaviour and show the bug it caused (a freshly
    // shown overlay drawer catching that same release and light-dismissing). Never touched by the app.
    public bool DeferRevealPastNavigatingClick { get; set; } = true;

    partial void OnSelectedEntryChanged(GalleryEntryViewModel? value)
    {
        // While a navigating click is in flight its release still owns the pointer, so no OTHER
        // selection can happen meanwhile: hold every flip back until the release has routed.
        // Un-clicked selections (arrow keys, search filtering, programmatic) arrive unarmed and
        // mirror straight away.
        if (DeferRevealPastNavigatingClick && _revealedArmedFrom is not null)
            return;
        ShownEntry = value;
    }

    /// <summary>Arms the hold-back for a navigating click whose press just landed.</summary>
    public void ArmDeferredReveal() => _revealedArmedFrom = ShownEntry;

    /// <summary>Disarms the hold-back and shows the selected entry now that the release has routed.</summary>
    public void CompleteDeferredReveal()
    {
        _revealedArmedFrom = null;
        ShownEntry = SelectedEntry;
    }

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
