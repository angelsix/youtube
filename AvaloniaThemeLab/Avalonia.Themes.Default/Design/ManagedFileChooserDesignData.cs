using System;
using System.Collections.Specialized;
using System.IO;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Dialogs.Internal;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace Avalonia.Themes.Default.Design;

// Design-time sample data for the ManagedFileChooser preview. The chooser is a real browser over
// the file system, so a preview that binds no view model shows an empty shell: the house rule
// applies and the sample state lives in C#, never inline in markup.
//
// The sample is the REAL view model (Avalonia.Dialogs.Internal.ManagedFileChooserViewModel) driven
// at a real folder, so the preview shows the same browsing UI a user meets: a places rail, a typed
// path, a folder of files and the Save/Cancel actions.
//
// The folder is created (if missing) under the user's temp directory, with the same shape as the
// reference sample: three folders and four files. Nothing here is public API - every type is
// internal.
//
//
// Each pane gets its OWN view model: the provider property below returns a NEW model every time it
// is read, and the preview template is instantiated once per pane, so the light pane and the dark
// pane each browse their own copy.

/// <summary>Provides the design-time models the ManagedFileChooser preview binds against.</summary>
internal static class ManagedFileChooserDesignData
{
    /// <summary>The name of the sample folder under the user's temp directory.</summary>
    public const string SampleFolderName = "ThemeLabFileChooserSample";

    /// <summary>The sample folder's full path; it is created on first use.</summary>
    public static string SampleFolder => Path.Combine(Path.GetTempPath(), SampleFolderName);

    /// <summary>
    /// A fresh view model browsing the sample folder in Save mode, with "report.pdf" proposed as
    /// the file name. A new model every read, so the two preview panes never share one.
    /// </summary>
    public static ManagedFileChooserViewModel Sample => CreateSample();

    private static ManagedFileChooserViewModel CreateSample()
    {
        EnsureSampleFolder();

        var model = new ManagedFileChooserViewModel(
            new FilePickerSaveOptions { SuggestedFileName = "report.pdf" },
            // The stock chooser refuses to select a FOLDER unless the dialog allows directory
            // selection, so a click on the first row of the sample (a folder) selects nothing and
            // the picture never shows a chosen row. The sample allows it, so a folder row really
            // is chosen when a person (or the proof crawler) clicks it.
            new ManagedFileDialogOptions { AllowDirectorySelection = true });

        model.Navigate(SampleFolder);
        ApplySamplePlaces(model);
        EnsurePlaceFolders();

        // NOTHING is chosen at rest, which is what the STOCK chooser shows in Save mode (measured
        // on the stock render: no file row carries a selection fill). A chosen row costs the
        // gallery two things it cannot afford:
        //
        //  * Avalonia's SelectingItemsControl scrolls the chosen row into view when the
        //    selection is set (AutoScrollToSelectedItem), and that BringIntoView bubbles up the
        //    gallery's page scroller - so the page would open SCROLLED DOWN, with the chooser's
        //    path box and the top of its list off-screen.
        //  * The stock view model rewrites the file-name field with the chosen rows' names as
        //    soon as more than one row is chosen (OnSelectionChangedAsync), so the field would
        //    read a quoted list ("Documents" "Pictures" "Projects") instead of the suggested name.
        model.SelectedItems.Clear();

        // The file name the Save dialog PROPOSES. It is set after the navigation and after the
        // clear above, because the view model rewrites the field whenever the selection changes -
        // and the clear is such a change. The assignment is posted at BACKGROUND priority so it
        // lands AFTER the view model's own reaction to that clear (which the dispatcher runs at
        // NORMAL priority), so the field keeps the proposed name.
        Dispatcher.UIThread.Post(() => model.FileName = "report.pdf", DispatcherPriority.Background);

        return model;
    }

    /// <summary>Creates each place's own folder, holding the same four files as the sample.</summary>
    private static void EnsurePlaceFolders()
    {
        try
        {
            Directory.CreateDirectory(PlacesRoot);
            foreach (var place in SamplePlaces)
            {
                WriteEntries(PlaceFolder(place));
            }
        }
        catch (Exception)
        {
            // A read-only or full temp directory must not take the whole preview down: the place
            // then points at a folder that does not exist, and the stock chooser falls back to its
            // root.
        }
    }

    /// <summary>The folder the sample's places live in, beside the sample folder itself.</summary>
    public static string PlacesRoot => SampleFolder + " Places";
    /// <summary>
    /// Points the places rail at the sample's own three places (Documents, Downloads, Pictures),
    /// each of which opens the sample folder.
    /// </summary>
    /// <remarks>
    /// The stock view model builds its quick links from the user's real special folders and the
    /// mounted volumes, and it REBUILDS them itself whenever the volume list changes. In a preview
    /// that means the rail shows whatever this machine happens to have - and, worse, pressing one
    /// of those places NAVIGATES the chooser: the file list is rebuilt, the rows a proof crawler
    /// had measured are detached, and every later measurement in that pass describes a different
    /// tree.
    ///
    /// The sample therefore names three places of its own, all of which open the sample folder, so
    /// the rail reads the same everywhere and pressing a place leaves the picture unchanged. The
    /// stock view model's own refresh is re-asserted over, because it arrives on the UI thread
    /// after the model was built.
    /// </remarks>
    private static void ApplySamplePlaces(ManagedFileChooserViewModel model)
    {
        var links = model.QuickLinks;
        links.CollectionChanged -= OnQuickLinksChanged;
        Fill(links);
        links.CollectionChanged += OnQuickLinksChanged;

        void OnQuickLinksChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // The stock view model replaced the rail with the machine's own places. Put the
            // sample's back once that operation has finished, so the collection is never mutated
            // from inside its own change notification.
            if (sender is not Avalonia.Collections.AvaloniaList<ManagedFileChooserItemViewModel> list)
                return;
            if (list.Count > 0 && list[0].DisplayName is "Documents" or "Downloads" or "Pictures")
                return;

            Dispatcher.UIThread.Post(() =>
            {
                list.CollectionChanged -= OnQuickLinksChanged;
                Fill(list);
                list.CollectionChanged += OnQuickLinksChanged;
            }, DispatcherPriority.Background);
        }
    }

    private static void Fill(Avalonia.Collections.AvaloniaList<ManagedFileChooserItemViewModel> links)
    {
        links.Clear();
        foreach (var place in SamplePlaces)
        {
            links.Add(new ManagedFileChooserItemViewModel
            {
                DisplayName = place,
                Path = PlaceFolder(place),
                ItemType = ManagedFileChooserItemType.Folder,
            });
        }
    }

    /// <summary>
    /// A place's own folder, holding the same four files as the sample folder, so the picture
    /// does not change when a place is pressed.
    /// </summary>
    /// <remarks>
    /// Each place points at a folder of its own rather than at the sample folder itself, because the
    /// stock view model finds the SELECTED place by matching the current location against the
    /// places' paths. Three places sharing one path would all match at once, and the rail could
    /// never show which one is current. With a folder each, pressing a place navigates to it - the
    /// stock behaviour - the rail marks that place as the chosen one, and the list still shows the
    /// same four files.
    ///
    /// The place folders sit BESIDE the sample folder, not inside it: a folder inside the browsed
    /// folder would show up as a row of the list, and pressing that row would navigate one level
    /// deeper, so the picture would change under the reader.
    /// </remarks>
    public static string PlaceFolder(string place) => Path.Combine(PlacesRoot, place);

    /// <summary>
    /// Creates the sample folder and its contents if they are not there yet. The files carry a
    /// little content so the size column has something to show.
    /// </summary>
    private static void EnsureSampleFolder()
    {
        var folder = SampleFolder;

        try
        {
            Directory.CreateDirectory(folder);
            WriteEntries(folder);

            // The sample's three FOLDERS sit beside its four files. They hold the same four files,
            // so browsing into one shows the same list the sample folder shows.
            foreach (var name in SampleFolders)
            {
                WriteEntries(Path.Combine(folder, name));
            }
        }
        catch (Exception)
        {
            // A read-only or full temp directory must not take the whole preview down: the chooser
            // then browses whatever folder it can reach.
        }
    }

    /// <summary>The sample's three folders: the places in the left rail.</summary>
    private static readonly string[] SampleFolders = ["Documents", "Pictures", "Projects"];

    /// <summary>The names the sample's places carry in the rail.</summary>
    private static readonly string[] SamplePlaces = ["Documents", "Downloads", "Pictures"];

    /// <summary>
    /// Fills a folder with the sample's four files.
    /// </summary>
    /// <remarks>
    /// The sample's three FOLDERS are rows of the file list AND the places in the left rail, and
    /// every one of them holds the same four files. The stock chooser navigates into a folder row
    /// on the SECOND click of that row (ManagedFileChooser.OnPointerPressed navigates when
    /// ClickCount is 2), so a folder row that a person (or the proof crawler) clicks twice would
    /// browse away, and the list's item containers would be rebuilt - the rows a measurement had
    /// just taken would be detached. Every folder holding the same four files keeps the list the
    /// same size and the same content, so browsing into one changes nothing the reader can see.
    /// </remarks>
    private static void WriteEntries(string folder)
    {
        Directory.CreateDirectory(folder);

        WriteFile(folder, "budget.xlsx", [0x50, 0x4B, 0x03, 0x04, 0x14, 0x00]);
        WriteFile(folder, "notes.txt", System.Text.Encoding.UTF8.GetBytes("Theme lab sample notes.\n"));
        WriteFile(folder, "photo.png", [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]);
        WriteFile(folder, "report.pdf", System.Text.Encoding.UTF8.GetBytes("%PDF-1.4 sample report\n"));
    }

    /// <summary>Writes one sample file, if it is not there yet.</summary>
    private static void WriteFile(string folder, string name, byte[] content)
    {
        var path = Path.Combine(folder, name);
        if (!File.Exists(path))
            File.WriteAllBytes(path, content);
    }
}
