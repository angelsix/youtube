using System;
using System.Collections.Specialized;
using System.IO;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Dialogs.Internal;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace AngelSix.Themes.Default.Design;

// Design-time sample data for the ManagedFileChooser preview.

/// <summary>Provides the design-time models the ManagedFileChooser preview binds against.</summary>
internal static class ManagedFileChooserDesignData
{
    /// <summary>The name of the container the sample tree lives in.</summary>
    public const string SampleFolderName = "ThemeLabFileChooserSample";

    /// <summary>The container the sample tree lives in: the browsed sample folder and the places' own folders, side by side.</summary>
    public static string SampleRoot => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        SampleFolderName);

    /// <summary>The browsed sample folder's full path; it is created on first use.</summary>
    public static string SampleFolder => Path.Combine(SampleRoot, "sample");

    /// <summary>A fresh view model browsing the sample folder in Save mode, with "report.pdf" proposed as the file name.</summary>
    public static ManagedFileChooserViewModel Sample => CreateSample();

    private static ManagedFileChooserViewModel CreateSample()
    {
        EnsureSampleFolder();

        var model = new ManagedFileChooserViewModel(
            new FilePickerSaveOptions { SuggestedFileName = "report.pdf" },
            new ManagedFileDialogOptions { AllowDirectorySelection = true });

        model.Navigate(SampleFolder);
        ApplySamplePlaces(model);
        EnsurePlaceFolders();

        // Nothing is chosen at rest, which is what the stock chooser shows in Save mode (measured on the stock render.
        model.SelectedItems.Clear();

        // The file name the Save dialog proposes.
        Dispatcher.UIThread.Post(() => model.FileName = "report.pdf", DispatcherPriority.Background);

        return model;
    }

    /// <summary>Creates each place's own folder, holding the same four files as the sample.</summary>
    private static void EnsurePlaceFolders()
    {
        try
        {
            Directory.CreateDirectory(PlacesRoot);
            foreach (var place in samplePlaces)
            {
                WriteEntries(PlaceFolder(place));
            }
        }
        catch (Exception)
        {
            // A read-only or full application-data directory must not take the whole preview down.
        }
    }

    /// <summary>The folder the sample's places live in, beside the sample folder itself.</summary>
    public static string PlacesRoot => Path.Combine(SampleRoot, "places");
    /// <summary>Points the places rail at the sample's own three places (Documents, Downloads, Pictures), each of which opens the sample folder.</summary>
    private static void ApplySamplePlaces(ManagedFileChooserViewModel model)
    {
        var links = model.QuickLinks;
        links.CollectionChanged -= OnQuickLinksChanged;
        Fill(links);
        links.CollectionChanged += OnQuickLinksChanged;

        void OnQuickLinksChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // The stock view model replaced the rail with the machine's own places.
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
        foreach (var place in samplePlaces)
        {
            links.Add(new ManagedFileChooserItemViewModel
            {
                DisplayName = place,
                Path = PlaceFolder(place),
                ItemType = ManagedFileChooserItemType.Folder,
            });
        }
    }

    /// <summary>A place's own folder, holding the same four files as the sample folder, so the picture does not change when a place is pressed.</summary>
    public static string PlaceFolder(string place) => Path.Combine(PlacesRoot, place);

    /// <summary>Creates the sample folder and its contents if they are not there yet.</summary>
    private static void EnsureSampleFolder()
    {
        var folder = SampleFolder;

        try
        {
            Directory.CreateDirectory(folder);
            WriteEntries(folder);

            // The sample's three folders sit beside its four files.
            foreach (var name in sampleFolders)
            {
                WriteEntries(Path.Combine(folder, name));
            }
        }
        catch (Exception)
        {
            // A read-only or full application-data directory must not take the whole preview down.
        }
    }

    /// <summary>The sample's three folders: the places in the left rail.</summary>
    private static readonly string[] sampleFolders = ["Documents", "Pictures", "Projects"];

    /// <summary>The names the sample's places carry in the rail.</summary>
    private static readonly string[] samplePlaces = ["Documents", "Downloads", "Pictures"];

    /// <summary>Fills a folder with the sample's four files.</summary>
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
