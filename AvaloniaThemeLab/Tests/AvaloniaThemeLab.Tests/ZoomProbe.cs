using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AvaloniaThemeLab.Tests;

/// <summary>One element as it was laid out: where it sits in its window and the size values it was given.</summary>
internal sealed record ElementShot(
    Visual Visual,
    string Path,
    string? ParentPath,
    Rect Bounds,
    IReadOnlyDictionary<string, SizeValue> Sizes);

/// <summary>One size-like property value, kept as numbers so two layouts can be compared.</summary>
internal sealed record SizeValue(double[] Numbers, string Text, string Priority);

/// <summary>Measures whether a layout is an exact zoom of another.</summary>
internal static class ZoomProbe
{
    /// <summary>Layout rounding moves an edge by up to one pixel at either size.</summary>
    public const double Tolerance = 1.0;

    // How many failing descendants a culprit-less root reports
    private const int InsideCulprits = 6;

    /// <summary>How many topmost failures a report lists.</summary>
    public const int ReportedRoots = 30;

    // Properties whose value decides a size or a position.
    private static readonly HashSet<string> sizeProperties =
    [
        "Width", "Height", "MinWidth", "MinHeight", "MaxWidth", "MaxHeight", "Margin", "Padding",
        "BorderThickness", "FontSize", "LineHeight", "LetterSpacing", "Spacing", "RowSpacing", "ColumnSpacing",
        "HorizontalSpacing", "VerticalSpacing", "ItemWidth", "ItemHeight", "StrokeThickness",
    ];

    /// <summary>Lets bindings, layout and transitions finish so the frame is the settled one.</summary>
    public static void Settle(TopLevel root, TimeSpan transitions, Action? step = null)
    {
        var clock = System.Diagnostics.Stopwatch.StartNew();
        do
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            root.UpdateLayout();
            step?.Invoke();
            Thread.Sleep(10);
        }
        while (clock.Elapsed < transitions);
        for (var i = 0; i < 3; i++)
        {
            Dispatcher.UIThread.RunJobs();
            root.UpdateLayout();
            step?.Invoke();
        }
    }

    /// <summary>Records every visible element the scope admits, keyed by its path in the visual tree.</summary>
    public static Dictionary<string, ElementShot> Shoot(TopLevel root, TimeSpan transitions, Func<Visual, bool> inScope, Action? step = null)
    {
        Settle(root, transitions, step);
        var shots = new Dictionary<string, ElementShot>();
        foreach (var visual in root.GetVisualDescendants())
        {
            if (!visual.IsEffectivelyVisible || !inScope(visual))
                continue;
            // Through every transform above it, so a glyph a Viewbox scales is measured as drawn
            if (visual.TransformToVisual(root) is not { } toRoot)
                continue;
            var path = PathOf(visual, root);
            var drawn = new Rect(visual.Bounds.Size).TransformToAABB(toRoot);
            shots[path] = new ElementShot(visual, path, ParentOf(path), drawn, SizesOf(visual));
        }
        return shots;
    }

    /// <summary>Compares a base layout with one that should be it scaled by <paramref name="factor"/>.</summary>
    public static ZoomReport Compare(Dictionary<string, ElementShot> baseline, Dictionary<string, ElementShot> zoomed, double factor)
    {
        var failing = new Dictionary<string, (ElementShot Base, ElementShot? Zoomed)>();
        var passing = 0;
        foreach (var (path, one) in baseline)
        {
            if (!zoomed.TryGetValue(path, out var two))
            {
                failing[path] = (one, null);
                continue;
            }
            if (Scales(one.Bounds, two.Bounds, factor))
                passing++;
            else
                failing[path] = (one, two);
        }
        var extra = zoomed.Values.Where(shot => !baseline.ContainsKey(shot.Path))
            .Select(shot => $"{Label(shot.Visual)} (in {Owner(shot.Visual)}) at …/{string.Join('/', shot.Path.Split('/').TakeLast(5))}").ToList();

        // Topmost: a failing element whose parent zoomed, or whose parent is outside the scope
        var roots = failing.Values
            .Where(f => f.Base.ParentPath is null || !failing.ContainsKey(f.Base.ParentPath))
            .Select(f => DescribeRoot(f.Base, f.Zoomed, baseline, zoomed, failing, factor))
            .OrderByDescending(r => r.Culprits.Count > 0)
            .ThenBy(r => r.Depth)
            .ToList();

        return new ZoomReport(baseline.Count, passing, failing.Count, extra, roots);
    }

    private static bool Scales(Rect one, Rect two, double factor)
    {
        return Math.Abs(two.X - factor * one.X) <= Tolerance
            && Math.Abs(two.Y - factor * one.Y) <= Tolerance
            && Math.Abs(two.Width - factor * one.Width) <= Tolerance
            && Math.Abs(two.Height - factor * one.Height) <= Tolerance;
    }

    private static ZoomRoot DescribeRoot(
        ElementShot one, ElementShot? two,
        Dictionary<string, ElementShot> baseline, Dictionary<string, ElementShot> zoomed,
        Dictionary<string, (ElementShot Base, ElementShot? Zoomed)> failing, double factor)
    {
        var culprits = new List<string>();
        if (two is not null)
            culprits.AddRange(Unscaled(one, two, factor).Select(c => $"self {c}"));

        // The parent zoomed, but its padding, spacing or grid lines can still have placed this child wrong
        var parent = one.Visual.GetVisualParent();
        if (parent is not null && one.ParentPath is not null)
        {
            var parentOne = baseline.GetValueOrDefault(one.ParentPath);
            var parentTwo = zoomed.GetValueOrDefault(one.ParentPath);
            if (parentOne is not null && parentTwo is not null)
                culprits.AddRange(Unscaled(parentOne, parentTwo, factor).Select(c => $"parent {Label(parent)} {c}"));
        }

        // Nothing here: the fixed size is further in (a preview's content sized by a literal on a child)
        if (culprits.Count == 0)
        {
            var inside = failing.Values
                .Where(f => f.Zoomed is not null && f.Base.Path.StartsWith(one.Path + "/", StringComparison.Ordinal))
                .Select(f => (Shot: f.Base, Unscaled: Unscaled(f.Base, f.Zoomed!, factor).ToList()))
                .Where(f => f.Unscaled.Count > 0)
                .OrderBy(f => f.Shot.Path.Count(ch => ch == '/'))
                .Take(InsideCulprits);
            foreach (var (shot, unscaled) in inside)
                culprits.AddRange(unscaled.Select(c => $"inside {Label(shot.Visual)} (in {Owner(shot.Visual)}) {c} at …{shot.Path[one.Path.Length..]}"));
        }

        return new ZoomRoot(one.Path, Label(one.Visual), Owner(one.Visual), one.Bounds, two?.Bounds, culprits,
            one.Path.Count(ch => ch == '/'));
    }

    // The size values that did not take the zoom: the literal (or fixed token) is the root cause
    private static IEnumerable<string> Unscaled(ElementShot one, ElementShot two, double factor)
    {
        foreach (var (name, a) in one.Sizes)
        {
            if (!two.Sizes.TryGetValue(name, out var b) || a.Numbers.Length != b.Numbers.Length)
                continue;
            var scaled = true;
            for (var i = 0; i < a.Numbers.Length && scaled; i++)
            {
                double x = a.Numbers[i], y = b.Numbers[i];
                scaled = double.IsFinite(x)
                    ? Math.Abs(y - factor * x) <= 0.01
                    : x.Equals(y);
            }
            if (!scaled)
                yield return $"{name}={a.Text} (zoomed {b.Text}, {a.Priority})";
        }
    }

    private static Dictionary<string, SizeValue> SizesOf(Visual visual)
    {
        var sizes = new Dictionary<string, SizeValue>();
        foreach (var property in AvaloniaPropertyRegistry.Instance.GetRegistered(visual))
        {
            if (!sizeProperties.Contains(property.Name))
                continue;
            var numbers = visual.GetValue(property) switch
            {
                double d => (double[])[d],
                Thickness t => (double[])[t.Left, t.Top, t.Right, t.Bottom],
                _ => null,
            };
            if (numbers is not null)
                sizes[property.Name] = new SizeValue(numbers, Format(numbers), PriorityOf(visual, property));
        }

        if (visual is Grid grid)
        {
            AddDefinitions(sizes, "ColumnDefinitions", grid.ColumnDefinitions.Select(c => c.Width));
            AddDefinitions(sizes, "RowDefinitions", grid.RowDefinitions.Select(r => r.Height));
        }
        return sizes;
    }

    // Only pixel lines take a size; star and Auto lines are proportions and always zoom
    private static void AddDefinitions(Dictionary<string, SizeValue> sizes, string name, IEnumerable<GridLength> lengths)
    {
        var list = lengths.ToArray();
        if (!list.Any(l => l.IsAbsolute))
            return;
        var numbers = list.Select(l => l.IsAbsolute ? l.Value : double.NaN).ToArray();
        sizes[name] = new SizeValue(numbers, string.Join(",", list.Select(l => l.ToString())), "Grid definition");
    }

    private static string PriorityOf(Visual visual, AvaloniaProperty property)
    {
        return visual.GetDiagnostic(property).Priority.ToString();
    }

    private static string Format(double[] numbers)
    {
        return string.Join(",", numbers.Select(n => n.ToString("0.##", CultureInfo.InvariantCulture)));
    }

    public static string Label(Visual visual)
    {
        return visual is Control { Name: { Length: > 0 } name } ? $"{visual.GetType().Name}#{name}" : visual.GetType().Name;
    }

    private static string Owner(Visual visual)
    {
        return visual is StyledElement { TemplatedParent: { } owner } ? owner.GetType().Name : "-";
    }

    private static string? ParentOf(string path)
    {
        var cut = path.LastIndexOf('/');
        return cut < 0 ? null : path[..cut];
    }

    private static string PathOf(Visual visual, TopLevel root)
    {
        var parts = new List<string>();
        for (Visual? current = visual; current is not null && !ReferenceEquals(current, root); current = current.GetVisualParent())
        {
            var parent = current.GetVisualParent();
            var index = parent is null ? 0 : parent.GetVisualChildren().TakeWhile(child => !ReferenceEquals(child, current)).Count();
            parts.Add($"{Label(current)}[{index}]");
        }
        parts.Reverse();
        return string.Join('/', parts);
    }
}

/// <summary>A topmost failing element and the unscaled size values found on it or its parent.</summary>
internal sealed record ZoomRoot(string Path, string Element, string Owner, Rect Base, Rect? Zoomed, IReadOnlyList<string> Culprits, int Depth);

/// <summary>The outcome of one zoom comparison, with the roots a fix should start from.</summary>
internal sealed record ZoomReport(int Elements, int Passing, int Failing, IReadOnlyList<string> Extra, IReadOnlyList<ZoomRoot> Roots)
{
    public bool Passed => Failing == 0 && Extra.Count == 0;

    public string Describe(string subject, double factor, string? trimPathBefore = null)
    {
        var text = new StringBuilder();
        text.AppendLine(CultureInfo.InvariantCulture,
            $"{subject}: {Failing} of {Elements} elements are not exactly x{factor} (within {ZoomProbe.Tolerance}px); " +
            $"{Extra.Count} appear only when zoomed; {Roots.Count} topmost failures (parent zoomed). The roots, culprits first:");
        foreach (var root in Roots.Take(ZoomProbe.ReportedRoots))
        {
            var zoomed = root.Zoomed is { } z ? Show(z) : "missing";
            text.AppendLine(CultureInfo.InvariantCulture, $"  {root.Element} (in {root.Owner}) {Show(root.Base)} -> {zoomed}");
            foreach (var culprit in root.Culprits)
                text.AppendLine(CultureInfo.InvariantCulture, $"      {culprit}");
            text.AppendLine(CultureInfo.InvariantCulture, $"      at {Trim(root.Path, trimPathBefore)}");
        }
        if (Roots.Count > ZoomProbe.ReportedRoots)
            text.AppendLine(CultureInfo.InvariantCulture, $"  ... and {Roots.Count - ZoomProbe.ReportedRoots} more topmost failures");
        foreach (var extra in Extra.Take(ZoomProbe.ReportedRoots))
            text.AppendLine(CultureInfo.InvariantCulture, $"  only when zoomed: {extra}");
        return text.ToString();
    }

    private static string Show(Rect r)
    {
        return string.Create(CultureInfo.InvariantCulture, $"({r.X:0.#},{r.Y:0.#} {r.Width:0.#}x{r.Height:0.#})");
    }

    private static string Trim(string path, string? before)
    {
        if (before is null)
            return path;
        var at = path.IndexOf(before, StringComparison.Ordinal);
        return at < 0 ? path : "…/" + path[at..];
    }
}
