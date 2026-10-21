using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Avalonia.Themes.Default;

/// <summary>
/// Tags a page hosted by a page HOST — a <see cref="TabbedPage"/>'s tab strip, a
/// <see cref="NavigationPage"/>'s navigation bar — with a style class naming the edge the host's
/// chrome sits on, so the page's OWN theme can square the border edge that meets it.
/// </summary>
/// <remarks>
/// <para><b>Why a behaviour:</b> the relationship "this page is hosted by a TabbedPage whose strip
/// docks on side N" is invisible to every selector. A ControlTheme's child styles are only ever
/// tested against the themed control itself, so neither the TabbedPage nor the inner TabControl
/// theme can target the page; a <c>&lt;TabControl.Styles&gt;</c> block written in the theme is
/// populated only when the inner template instantiates — AFTER the page has already styled itself —
/// so it silently does nothing. Worse, the page's base theme sets <c>CornerRadius</c> with a bare
/// <c>ContentPage</c> selector in the IMPLICIT-THEME frame, which outranks any ancestor <c>Styles</c>
/// bag, so no externally-hosted rule can win the contest at all. The one lever that survives is the
/// page's own theme reacting to a CLASS the page carries — the same route <see cref="LabelPairing"/>
/// uses for label-field pairing. This behaviour finds the pages the host carries and reports the
/// edge the host's chrome sits on as a class.</para>
/// <para><b>The classes name the SQUARED edge, not the host:</b> <c>SquareTop</c>,
/// <c>SquareBottom</c>, <c>SquareLeft</c>, <c>SquareRight</c>. A tab strip on top and a navigation
/// bar on top both square the page's top edge, so they share one class and one theme rule. The
/// class is added to the page, so the page's own ControlTheme owns the corner rule — the theme
/// never reaches into a page it does not theme.</para>
/// <para>The border THICKNESS is untouched: only the CORNERS change, and only the pair meeting the
/// host's chrome — the opposite pair keeps the theme radius ({size:RadiusSm}).</para>
/// </remarks>
public sealed class PageContentCorners
{
    /// <summary>Squares the hosted page's chrome-side corners on the page host.</summary>
    public static readonly AttachedProperty<bool> ApplyProperty =
        AvaloniaProperty.RegisterAttached<PageContentCorners, Control, bool>("Apply");

    private const string Prefix = "Square";

#pragma warning disable IDE0028, IDE1006 // The repo's analyzers reject both spellings of these initializers (mirrors LabelPairing).
    private static readonly ConditionalWeakTable<Control, Attachment> Live = new();

    private sealed class Attachment
    {
        /// <summary>The pages currently carrying a class, and the class they carry.</summary>
        public List<Page> Tagged = new();

        public string? Current;
    }
#pragma warning restore IDE0028

    /// <summary>
    /// The type initializer subscribes to the flag's GLOBAL CHANGE OBSERVABLE
    /// (<c>AvaloniaProperty.Changed</c>: "fired when this property changes on ANY instance"), so the
    /// behaviour activates from either direction: the compiled XAML applies the flag with
    /// <c>SetValue</c> — which fires the observable — and any consumer touching this type arms it.
    /// Mirrors <see cref="LabelPairing"/>.
    /// </summary>
    static PageContentCorners()
    {
        try
        {
            ApplyProperty.Changed.Subscribe(new FlagWatcher());
        }
        catch
        {
            // Wiring trouble must never propagate into the shared application.
        }
    }

    private sealed class FlagWatcher : IObserver<AvaloniaPropertyChangedEventArgs>
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(AvaloniaPropertyChangedEventArgs value)
        {
            if (value.Sender is not Control target)
                return;
            try
            {
                if (target.GetValue<bool>(ApplyProperty))
                    Arm(target);
            }
            catch
            {
                // Wiring trouble must never propagate into the shared application.
            }
        }
    }

    private static void Arm(Control host)
    {
        // Re-running is harmless: Retag clears and re-adds the class, so a re-attach (design-time
        // reload, template rebuild) simply refreshes the tags.
        var att = Live.GetValue(host, _ => new Attachment());

        // Tag as soon as the host enters the logical tree. Its template hosts the pages a beat
        // later, so also re-tag on the next dispatcher frame once the pages exist; a late class still
        // reacts (the theme rule subscribes dynamically), so either timing lands the corners.
        var arm = () =>
        {
            Retag(att, host);
            Dispatcher.UIThread.Post(() => Retag(att, host));
        };
        if (((ILogical)host).IsAttachedToLogicalTree)
            arm();
        else
            host.AttachedToLogicalTree += (_, _) => arm();

        // A page added to or removed from the host's page collection, or a navigation that swaps the
        // current page, brings a new page in; re-tag so the newcomer carries the class too.
        if (host is MultiPage pages)
        {
            pages.PagesChanged += (_, _) => Retag(att, host);
            pages.CurrentPageChanged += (_, _) => Retag(att, host);
        }

        // The last resort: the host's template may present its page before the page exists as a
        // logical child, so every layout pass re-checks. The check is a comparison, not a re-tag,
        // so a stable host costs almost nothing.
        host.LayoutUpdated += (_, _) => Retag(att, host);

        // Re-tag when the tab strip MOVES sides; every page is a logical child from the start, so
        // selection changes need no re-tagging (each page already carries the class).
        host.PropertyChanged += (_, ev) =>
        {
            if (ev.Property == TabbedPage.TabPlacementProperty)
                Retag(att, host);
        };
    }

    private static void Retag(Attachment att, Control host)
    {
        string? cls = ClassFor(host);
        List<Page> wanted = PagesUnder(host);

        // Nothing to do when the edge and the page set are unchanged: the common case, and the one
        // the layout pass above hits on every frame.
        if (cls == att.Current && SamePages(att.Tagged, wanted))
            return;

        // Clear any previously reported class so a page that leaves the host stops squaring.
        foreach (var p in att.Tagged)
            RemoveClasses(p);
        att.Tagged.Clear();

        att.Current = cls;
        if (cls is null)
            return;

        foreach (var p in wanted)
        {
            p.Classes.Add(cls);
            att.Tagged.Add(p);
        }
    }

    private static bool SamePages(List<Page> a, List<Page> b)
    {
        if (a.Count != b.Count)
            return false;
        for (int i = 0; i < a.Count; i++)
            if (!ReferenceEquals(a[i], b[i]))
                return false;
        return true;
    }

    /// <summary>
    /// The pages the host carries. A page host exposes them directly: a tabbed page's
    /// <see cref="MultiPage.Pages"/> collection is its whole page set, and a navigation page's
    /// <see cref="Page.CurrentPage"/> is the page it shows. The logical descendants are read too,
    /// because a host may parent its pages without listing them.
    /// </summary>
    private static List<Page> PagesUnder(Control host)
    {
        var seen = new List<Page>();

        if (host is MultiPage pages)
        {
            foreach (var p in pages.Pages)
                if (p is not null && !seen.Contains(p))
                    seen.Add(p);
        }

        // A navigation page's active page is the one it presents; it is not in the Pages
        // collection, so it is read from the page itself.
        if (host is NavigationPage navigation && navigation.CurrentPage is Page current && !seen.Contains(current))
            seen.Add(current);

        foreach (var p in LogicalPagesUnder(host))
            if (!seen.Contains(p))
                seen.Add(p);

        return seen;
    }

    private static IEnumerable<Page> LogicalPagesUnder(ILogical root)
    {
        foreach (var child in root.GetLogicalChildren())
        {
            if (child is Page p)
                yield return p;
            else
                foreach (var d in LogicalPagesUnder(child))
                    yield return d;
        }
    }

    /// <summary>
    /// The edge the host's chrome sits on, as the class the page's own theme reads, or null when the
    /// host has no chrome edge to meet (nothing is squared then).
    /// </summary>
    private static string? ClassFor(Control host) => host switch
    {
        // The tab strip docks on one side, and the page squares the edge that meets it.
        TabbedPage tabbed => tabbed.TabPlacement switch
        {
            TabPlacement.Bottom => "SquareBottom",
            TabPlacement.Left => "SquareLeft",
            TabPlacement.Right => "SquareRight",
            _ => "SquareTop", // Top (and Auto-on-desktop): the strip docks on top
        },
        // The navigation bar docks on top, so the hosted page squares its top edge.
        NavigationPage => "SquareTop",
        _ => null,
    };

    private static void RemoveClasses(Page page)
    {
        foreach (string c in new List<string>(page.Classes))
            if (c.StartsWith(Prefix, StringComparison.Ordinal))
                page.Classes.Remove(c);
    }
}
