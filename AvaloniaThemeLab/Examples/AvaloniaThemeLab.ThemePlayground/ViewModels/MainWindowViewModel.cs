using System;
using System.Collections.Generic;
using System.Linq;
using AngelSix.ThemeEngine.Generated;
using AngelSix.Mvvm;
using AvaloniaThemeLab.ThemePlayground.Views;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>The gallery window: a searchable list of entries and the one being shown.</summary>
public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly GalleryCatalogue _catalogue;

    [Observable] public partial string SearchText { get; set; } = string.Empty;

    [Observable] public partial GalleryEntryViewModel? SelectedEntry { get; set; }

    [Observable] public partial GalleryEntryViewModel? ShownEntry { get; set; }

    // The app's one accent: the header picker's selection, styled into every preview.
    [Observable] public partial AccentHue SelectedAccent { get; set; } = Hue(AccentKind.Primary);

    /// <summary>The hues the header picker offers, in the order DefaultTheme declares them.</summary>
    public static IReadOnlyList<AccentHue> AccentChoices { get; } =
    [
        Hue(AccentKind.Neutral), Hue(AccentKind.Primary), Hue(AccentKind.Success), Hue(AccentKind.Warning),
        Hue(AccentKind.Favourite), Hue(AccentKind.Error), Hue(AccentKind.Info), Hue(AccentKind.Destructive), Hue(AccentKind.Subtle),
    ];

    /// <summary>The engine hue for one of this theme's generated accent kinds.</summary>
    public static AccentHue Hue(AccentKind kind) => new(kind.ToString());

    // Absent in design-time and headless tools, which have no window to open over
    private readonly ThemeSettingsWindowService? _settingsWindow;

    public MainWindowViewModel(GalleryCatalogue catalogue, ThemeSettingsWindowService? settingsWindow = null)
    {
        _catalogue = catalogue;
        _settingsWindow = settingsWindow;
        // No click is in flight, so the change handler shows the first entry straight away
        SelectedEntry = catalogue.Entries.FirstOrDefault();
    }

    public GalleryCatalogue Catalogue => _catalogue;

    /// <summary>Opens the live theme editor over this window.</summary>
    [Command]
    private void OpenSettings() => _settingsWindow?.Show();

    // Armed while a navigating click on the sidebar is in flight (its press has landed, its release has not yet routed).
    private GalleryEntryViewModel? _revealedArmedFrom;

    // Production default: the page is revealed only after the navigating click's release has routed (the whole point of the ShownEntry indirection).
    public bool DeferRevealPastNavigatingClick { get; set; } = true;

    partial void OnSelectedEntryChanged(GalleryEntryViewModel? value)
    {
        // While a navigating click is in flight its release still owns the pointer, so no other selection can happen meanwhile.
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
