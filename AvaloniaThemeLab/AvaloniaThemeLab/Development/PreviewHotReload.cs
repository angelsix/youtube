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

namespace AvaloniaThemeLab.Development;

/// <summary>
/// Re-parses a theme control's .axaml when it is edited on disk and swaps its <c>{Name}Preview</c>
/// template into the running app, so a preview updates within a second of a save — no rebuild, no
/// restart. Compiled out of Release entirely: the call site and the parser package are both
/// Debug-gated.
/// </summary>
/// <remarks>
/// <para>
/// Only string-keyed entries ending in "Preview" are taken from a reparsed file; ControlThemes and
/// every other key are ignored, so a broken mid-edit ControlTheme can never leak past the parse
/// guard. A file that fails to parse keeps serving its last-good template — a malformed save must
/// never crash the app. Entries whose key the app does not load are skipped with a log.
/// </para>
/// <para>
/// Swaps override the compiled theme's copy by writing the new template into the application's own
/// resource dictionary — the highest-precedence slot of <c>TryFindResource</c> — so every lookup,
/// present and future pages alike, sees the new one. Refresh propagation: the page area binds
/// <c>SelectedEntry</c>, and the preview template reaches it through
/// <see cref="ValueConverters.PreviewTemplateConverter"/>'s per-conversion <c>TryFindResource</c>;
/// after a swap the converter is simply re-run against the updated resources and pushed onto each
/// visible <c>LightDarkPreview.PaneContentTemplate</c>. Everything runs on the UI thread; the
/// watcher thread touches nothing.
/// </para>
/// </remarks>
internal static class PreviewHotReload
{
    private const int DebounceMilliseconds = 300; // editors fire several events per save

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
            Debug.WriteLine($"[hotreload] No ../Avalonia.Themes.Default/Controls above {AppContext.BaseDirectory}; " +
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

    // Walks up from the exe's folder until the sibling Controls directory exists — bin/<config>/<tfm>
    // sits three levels below the repo root today, but deeper layouts (publish, CI) must not break it
    private static DirectoryInfo? FindControlsDirectory(string startAt)
    {
        for (var dir = Directory.GetParent(startAt); dir is not null; dir = dir.Parent)
            if (Directory.Exists(Path.Combine(dir.FullName, "Avalonia.Themes.Default", "Controls")))
                return new DirectoryInfo(Path.Combine(dir.FullName, "Avalonia.Themes.Default", "Controls"));

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

    // Parses every file, collecting the Preview entries of the ones that succeed; a file that fails
    // keeps serving its last-good template rather than blocking its siblings
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

    // Overrides the compiled theme's copy at the highest-precedence slot, so every lookup — the
    // converter's per-page TryFindResource included — sees the new template. Whether the app loads
    // the control is tested with that very resolution: a preview the theme does not supply is a
    // control the app never shows, so it is skipped with a log rather than orphaned in memory.
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

    // Re-resolves the selected entry's template through the very converter the page uses and pushes
    // it onto each visible preview host, so the current page repaints without touching the view model
    private static void ForceGalleryRefresh()
    {
        if ((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow is not Window window)
            return;

        var converter = new ValueConverters.PreviewTemplateConverter();
        foreach (var visual in window.GetVisualDescendants().OfType<Avalonia.Themes.Default.LightDarkPreview>())
            if (visual.DataContext is ViewModels.ControlEntryViewModel controlEntry)
            {
                var template = converter.Convert(controlEntry.ControlType, typeof(DataTemplate), null, CultureInfo.InvariantCulture) as DataTemplate;
                if (template is not null)
                    visual.SetCurrentValue(Avalonia.Themes.Default.LightDarkPreview.PaneContentTemplateProperty, template);
            }

        Debug.WriteLine("[hotreload] Visible previews refreshed.");
    }
}
#endif
