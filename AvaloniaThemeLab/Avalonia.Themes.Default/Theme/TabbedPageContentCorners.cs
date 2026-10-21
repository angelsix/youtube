using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Avalonia.Themes.Default;

/// <summary>
/// Tags the page hosted by a <see cref="TabbedPage"/> with a <c>Tabs&lt;Side&gt;</c> style class so
/// the page's OWN theme can square the border edge that meets the tabs.
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
/// uses for label-field pairing. This behaviour walks the page's LOGICAL ancestors (the framework
/// reparents the hosted page under the TabbedPage), reports the nearest one's
/// <see cref="TabbedPage.TabPlacement"/> as a class, and re-tags on selection/placement changes.</para>
/// <para>The border THICKNESS is untouched: only the CORNERS change, and only the pair meeting the
/// strip — the opposite pair keeps the theme radius ({size:RadiusSm}).</para>
/// </remarks>
public sealed class TabbedPageContentCorners
{
    /// <summary>Squares the hosted page's tab-side corners on the TabbedPage.</summary>
    public static readonly AttachedProperty<bool> ApplyProperty =
        AvaloniaProperty.RegisterAttached<TabbedPageContentCorners, TabbedPage, bool>("Apply");

    private const string Prefix = "Tabs";

#pragma warning disable IDE0028 // The repo's analyzers reject both spellings of these initializers (mirrors LabelPairing).
    private static readonly ConditionalWeakTable<TabbedPage, Attachment> Live = new();

    private sealed class Attachment
    {
        public List<Page> Tagged = new();
    }
#pragma warning restore IDE0028

    /// <summary>
    /// The type initializer subscribes to the flag's GLOBAL CHANGE OBSERVABLE
    /// (<c>AvaloniaProperty.Changed</c>: "fired when this property changes on ANY instance"), so the
    /// behaviour activates from either direction: the compiled XAML applies the flag with
    /// <c>SetValue</c> — which fires the observable — and any consumer touching this type arms it.
    /// Mirrors <see cref="LabelPairing"/>.
    /// </summary>
    static TabbedPageContentCorners()
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
            if (value.Sender is not TabbedPage target)
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

    private static void Arm(TabbedPage page)
    {
        // Re-running is harmless: Retag clears and re-adds the class, so a re-attach (design-time
        // reload, template rebuild) simply refreshes the tags.
        var att = Live.GetValue(page, _ => new Attachment());

        // Tag as soon as the TabbedPage enters the logical tree. Its template hosts the pages a beat
        // later, so also re-tag on the next dispatcher frame once the pages exist; a late class still
        // reacts (the theme rule subscribes dynamically), so either timing lands the corners.
        var arm = () =>
        {
            Retag(att, page);
            Avalonia.Threading.Dispatcher.UIThread.Post(() => Retag(att, page));
        };
        if (((ILogical)page).IsAttachedToLogicalTree)
            arm();
        else
            page.AttachedToLogicalTree += (_, _) => arm();

        // Re-tag only if the strip MOVES sides; every page is a logical child from the start, so
        // selection changes need no re-tagging (each page already carries the class).
        page.PropertyChanged += (_, ev) =>
        {
            if (ev.Property == TabbedPage.TabPlacementProperty)
                Retag(att, page);
        };
    }

    private static void Retag(Attachment att, TabbedPage page)
    {
        // Clear any previously reported class so a page that leaves the host stops squaring.
        foreach (var p in att.Tagged)
            RemoveClasses(p);
        att.Tagged.Clear();

        string cls = ClassFor(page.TabPlacement);
        foreach (var p in LogicalPagesUnder(page))
        {
            p.Classes.Add(cls);
            att.Tagged.Add(p);
        }
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

    private static string ClassFor(TabPlacement placement) => placement switch
    {
        TabPlacement.Bottom => "TabsBottom",
        TabPlacement.Left => "TabsLeft",
        TabPlacement.Right => "TabsRight",
        _ => "TabsTop", // Top (and Auto-on-desktop): the strip docks on top
    };

    private static void RemoveClasses(Page page)
    {
        foreach (string c in new List<string>(page.Classes))
            if (c.StartsWith(Prefix))
                page.Classes.Remove(c);
    }
}
