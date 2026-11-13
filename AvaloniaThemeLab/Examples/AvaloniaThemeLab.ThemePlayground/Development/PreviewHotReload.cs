#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AvaloniaThemeLab.ThemePlayground.Development;

/// <summary>Re-parses a theme control file when it is edited on disk and swaps its <c>{Name}Preview</c> template into the running app, so a preview updates within a second of a save.</summary>
internal static class PreviewHotReload
{
    private const int DebounceMilliseconds = 300;

    private static FileSystemWatcher? watcher;
    private static readonly HashSet<string> pendingFiles = [];
    private static Timer? debounceTimer;

    /// <summary>Begins watching the theme's Controls directory; safe to call repeatedly.</summary>
    public static void Start()
    {
        if (watcher is not null) return;

        var controlsDir = FindControlsDirectory(AppContext.BaseDirectory);
        if (controlsDir is null)
        {
            Debug.WriteLine($"[hotreload] No Themes/AngelSix.Themes.Default/Controls above {AppContext.BaseDirectory}; " +
                            "preview hot-reload disabled.");
            return;
        }

        watcher = new FileSystemWatcher(controlsDir.FullName, "*.axaml")
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
        };
        watcher.Changed += (_, e) => QueueChanged(e.FullPath);
        watcher.Created += (_, e) => QueueChanged(e.FullPath);
        watcher.EnableRaisingEvents = true;

        Debug.WriteLine($"[hotreload] Watching {controlsDir.FullName}");
    }

    private static DirectoryInfo? FindControlsDirectory(string startAt)
    {
        for (var dir = Directory.GetParent(startAt); dir is not null; dir = dir.Parent)
            if (Directory.Exists(Path.Combine(dir.FullName, "Themes", "AngelSix.Themes.Default", "Controls")))
                return new DirectoryInfo(Path.Combine(dir.FullName, "Themes", "AngelSix.Themes.Default", "Controls"));

        return null;
    }

    // Records the file and resets the debounce clock; the settled pass drains everything queued so far
    private static void QueueChanged(string fullPath)
    {
        lock (pendingFiles)
            pendingFiles.Add(fullPath);

        debounceTimer?.Dispose();
        debounceTimer = new Timer(static _ => DrainAndApply(), null, DebounceMilliseconds, Timeout.Infinite);
    }

    private static void DrainAndApply()
    {
        // A dev tool must never take the app down: whatever slips past the per-file guards dies here
        try
        {
            var parsed = CollectTemplates(DrainPendingFiles());
            if (parsed.Count == 0) return;

            // Resources and the gallery live on the UI thread; the watcher thread must never touch them
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    foreach (var (key, template) in parsed)
                        SwapIntoAppResources(key, template);

                    ForceGalleryRefresh();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[hotreload] Aborted reload: {ex}");
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[hotreload] Aborted reload: {ex}");
        }
    }

    private static List<string> DrainPendingFiles()
    {
        lock (pendingFiles)
        {
            List<string> files = [.. pendingFiles];
            pendingFiles.Clear();
            return files;
        }
    }

    // Parses every file, collecting the Preview entries of the ones that succeed.
    private static Dictionary<string, object> CollectTemplates(List<string> files)
    {
        var parsed = new Dictionary<string, object>();
        foreach (var path in files.Distinct())
        {
            try
            {
                var dict = AvaloniaRuntimeXamlLoader.Parse<ResourceDictionary>(File.ReadAllText(path), typeof(ResourceDictionary).Assembly);
                foreach (var (key, value) in dict)
                    if (key is string s && s.EndsWith("Preview") && value is not null)
                        parsed[s] = value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[hotreload] Keeping last-good templates for {Path.GetFileName(path)}: {ex.Message}");
            }
        }

        return parsed;
    }

    // Overrides the compiled theme's copy at the highest-precedence slot, so every lookup.
    private static void SwapIntoAppResources(string key, object template)
    {
        var app = Application.Current;
        if (app is null) return;

        if (!app.TryFindResource(key, app.ActualThemeVariant, out _))
        {
            Debug.WriteLine($"[hotreload] Skipping '{key}': the app does not load it.");
            return;
        }

        app.Resources[key] = template;
        Debug.WriteLine($"[hotreload] Swapped '{key}'.");
    }

    private static void ForceGalleryRefresh()
    {
        if ((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow is not Window window)
            return;

        var converter = new ValueConverters.PreviewTemplateConverter();
        foreach (var visual in window.GetVisualDescendants().OfType<AngelSix.ThemeEngine.Controls.LightDarkPreview>())
            if (visual.DataContext is ViewModels.ControlEntryViewModel controlEntry)
            {
                var template = converter.Convert(controlEntry.ControlType, typeof(DataTemplate), null, CultureInfo.InvariantCulture) as DataTemplate;
                if (template is not null)
                    visual.SetCurrentValue(AngelSix.ThemeEngine.Controls.LightDarkPreview.PaneContentTemplateProperty, template);
            }

        Debug.WriteLine("[hotreload] Visible previews refreshed.");
    }
}
#endif
