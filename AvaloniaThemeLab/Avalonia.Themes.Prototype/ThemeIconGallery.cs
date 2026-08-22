using AngelSix.ThemeEngine;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// Holds common icon glyph paths used across the theme library.
/// A theme exposes an instance via <see cref="DefaultTheme.IconGallery"/> so subclasses
/// can override the entire icon set while keeping the same token names.
/// </summary>
/// <remarks>
/// Marked with <c>[IconGallery]</c> so the source generator finds it by convention. It used to be
/// located by its hardcoded full type name, which meant every project referencing this assembly
/// also matched it and re-emitted the whole glyph set on top of this one.
/// </remarks>
[IconGallery]
public class ThemeIconGallery
{
    // ── Chevron (triangle) ──────────────────────────────────────────
    // Upward-pointing triangle; rotate 180° for down, -90° for left, +90° for right.
    // Used in ButtonSpinner, ComboBox, NumericUpDown, Expander, TreeView.

    /// <summary>Small upward chevron triangle (24×14 viewBox).</summary>
    public virtual string ChevronUpGlyph => "M 0 9 L 12 0 L 24 9 L 22 11 L 12 3 L 2 11 Z";

    /// <summary>Small downward chevron triangle — 180° rotation of ChevronUpGlyph.</summary>
    public virtual string ChevronDownGlyph => "M 0 2 L 12 11 L 24 2 L 22 0 L 12 8 L 2 0 Z";

    /// <summary>Small leftward chevron triangle — -90° rotation of ChevronUpGlyph.</summary>
    public virtual string ChevronLeftGlyph => "M 9 0 L 0 12 L 9 24 L 11 22 L 3 12 L 11 2 Z";

    /// <summary>Small rightward chevron triangle — +90° rotation of ChevronUpGlyph.</summary>
    public virtual string ChevronRightGlyph => "M 2 0 L 11 12 L 2 24 L 0 22 L 8 12 L 0 2 Z";

    // This is the same triangle the original ButtonSpinner used (M0,9 L10,0 20,9 19,10 10,2 1,10 z),
    // but normalised to 24×14 viewBox and written in a clean path grammar so all glyphs are consistent.

    // ── Expand / collapse ───────────────────────────────────────────
    // Chevron-style expand/collapse indicators. The expand variant is two
    // chevron-like segments; collapse is a single chevron pointing back.

    /// <summary>Chevron pointing one direction, used as the "collapsed" indicator in TreeView.</summary>
    public virtual string ExpandChevronGlyph => "M 1 1 L 10 10 L 19 1 L 18 0 L 10 8 L 2 0 Z";

    /// <summary>Chevron pointing another direction, used as the "expanded" indicator in TreeView.</summary>
    public virtual string CollapseChevronGlyph => "M 1 0 L 10 10 L 19 0 L 18 -1 L 10 8 L 2 -1 Z";

    // ── Drop-down arrow (used by ComboBox, DropDownButton, SplitButton) ──
    // A simple downward-pointing triangle. Same geometry as ChevronDownGlyph
    // but we keep the semantic alias.

    /// <summary>Drop-down arrow — same as ChevronDownGlyph.</summary>
    public virtual string DropDownGlyph => "M 0 2 L 12 11 L 24 2 L 22 0 L 12 8 L 2 0 Z";

    // ── Close / X ───────────────────────────────────────────────────
    // Standard close icon, useful for NotificationCard, TabItem close buttons, etc.

    /// <summary>X-shaped close icon (24×24 viewBox).</summary>
    public virtual string CloseGlyph => "M 1 1 L 10 10 L 1 19 L 3 21 L 12 12 L 21 21 L 23 19 L 14 10 L 23 1 L 21 -1 L 12 8 L 3 -1 Z";

    // ── Checkmark ───────────────────────────────────────────────────
    // Used in CheckBox for the check state.

    /// <summary>Checkmark tick (24×24 viewBox).</summary>
    public virtual string CheckmarkGlyph => "M 0 12 L 8 20 L 24 3 L 22 1 L 8 17 L 2 11 Z";

    // ── Hamburger / Menu ────────────────────────────────────────────
    // Three horizontal lines.

    /// <summary>Hamburger menu icon (24×24 viewBox). Three filled bars.</summary>
    public virtual string MenuGlyph => "M 0 2 L 24 2 L 24 6 L 0 6 Z M 0 10 L 24 10 L 24 14 L 0 14 Z M 0 18 L 24 18 L 24 22 L 0 22 Z";

    // ── Star (favourite) ────────────────────────────────────────────

    /// <summary>Five-pointed star (24×24 viewBox).</summary>
    public virtual string StarGlyph => "M 12 0 L 15 9 L 24 9 L 17 15 L 20 24 L 12 19 L 4 24 L 7 15 L 0 9 L 9 9 Z";

    // ── Arrow variants ──────────────────────────────────────────────
    // Unlike chevrons (angled lines), these have a tail/shaft.

    /// <summary>Left-pointing arrow (24×24 viewBox).</summary>
    public virtual string ArrowLeftGlyph => "M 24 11 L 5 11 L 11 5 L 9 3 L 1 12 L 9 21 L 11 19 L 5 13 L 24 13 Z";

    /// <summary>Right-pointing arrow (24×24 viewBox).</summary>
    public virtual string ArrowRightGlyph => "M 0 11 L 19 11 L 13 5 L 15 3 L 23 12 L 15 21 L 13 19 L 19 13 L 0 13 Z";

    /// <summary>Upward-pointing arrow (24×24 viewBox).</summary>
    public virtual string ArrowUpGlyph => "M 11 24 L 11 5 L 5 11 L 3 9 L 12 1 L 21 9 L 19 11 L 13 5 L 13 24 Z";

    /// <summary>Downward-pointing arrow (24×24 viewBox).</summary>
    public virtual string ArrowDownGlyph => "M 11 0 L 11 19 L 5 13 L 3 15 L 12 23 L 21 15 L 19 13 L 13 19 L 13 0 Z";

    // ── More / ellipsis ─────────────────────────────────────────────
    // Three dots, each a full circle of four quarter arcs. The earlier form closed each
    // dot onto a point a hundredth away from its start ("A 3 3 0 1 1 5 12.01"), which
    // Avalonia's parser collapses — see the note on ErrorGlyph.

    /// <summary>Horizontal ellipsis (24×24 viewBox).</summary>
    public virtual string MoreHorizontalGlyph =>
        "M 5 9 A 3 3 0 0 1 8 12 A 3 3 0 0 1 5 15 A 3 3 0 0 1 2 12 A 3 3 0 0 1 5 9 Z " +
        "M 12 9 A 3 3 0 0 1 15 12 A 3 3 0 0 1 12 15 A 3 3 0 0 1 9 12 A 3 3 0 0 1 12 9 Z " +
        "M 19 9 A 3 3 0 0 1 22 12 A 3 3 0 0 1 19 15 A 3 3 0 0 1 16 12 A 3 3 0 0 1 19 9 Z";

    /// <summary>Vertical ellipsis (24×24 viewBox).</summary>
    public virtual string MoreVerticalGlyph =>
        "M 12 2 A 3 3 0 0 1 15 5 A 3 3 0 0 1 12 8 A 3 3 0 0 1 9 5 A 3 3 0 0 1 12 2 Z " +
        "M 12 9 A 3 3 0 0 1 15 12 A 3 3 0 0 1 12 15 A 3 3 0 0 1 9 12 A 3 3 0 0 1 12 9 Z " +
        "M 12 16 A 3 3 0 0 1 15 19 A 3 3 0 0 1 12 22 A 3 3 0 0 1 9 19 A 3 3 0 0 1 12 16 Z";

    // ── Plus / Add ──────────────────────────────────────────────────

    /// <summary>Plus sign (24×24 viewBox).</summary>
    public virtual string PlusGlyph => "M 11 0 L 13 0 L 13 11 L 24 11 L 24 13 L 13 13 L 13 24 L 11 24 L 11 13 L 0 13 L 0 11 L 11 11 Z";

    /// <summary>Minus sign (24×24 viewBox).</summary>
    public virtual string MinusGlyph => "M 0 11 L 24 11 L 24 13 L 0 13 Z";

    // ── Error / validation ──────────────────────────────────────────
    // A solid disc with the exclamation knocked out of it, so the glyph reads at
    // icon size against any background. The disc is wound clockwise and the bar and
    // dot anti-clockwise, which is what punches them out under the nonzero fill rule.
    //
    // Circles are built from four quarter arcs, never two semicircles: Avalonia's path
    // parser collapses an arc whose endpoints are exactly one diameter apart to a
    // zero-width line, so "A 12 12 0 1 1 12 24" from (12,0) renders as a vertical stroke.

    /// <summary>Exclamation mark in a filled circle (24×24 viewBox).</summary>
    public virtual string ErrorGlyph =>
        "M 12 0 A 12 12 0 0 1 24 12 A 12 12 0 0 1 12 24 A 12 12 0 0 1 0 12 A 12 12 0 0 1 12 0 Z " +
        "M 10.5 5 L 10.5 14 L 13.5 14 L 13.5 5 Z " +
        "M 12 16 A 2 2 0 0 0 10 18 A 2 2 0 0 0 12 20 A 2 2 0 0 0 14 18 A 2 2 0 0 0 12 16 Z";
}
