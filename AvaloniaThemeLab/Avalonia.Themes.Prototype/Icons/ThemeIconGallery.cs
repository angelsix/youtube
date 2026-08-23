using AngelSix.ThemeEngine;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// Holds the icon paths used across the theme library.
/// A theme exposes an instance via <see cref="DefaultTheme.IconGallery"/> so subclasses
/// can override the entire icon set while keeping the same token names.
/// </summary>
/// <remarks>
/// Marked with <c>[IconGallery]</c> so the source generator finds it by convention. It used to be
/// located by its hardcoded full type name, which meant every project referencing this assembly
/// also matched it and re-emitted the whole icon set on top of this one.
/// </remarks>
[IconGallery]
public class ThemeIconGallery
{
    // ── Chevron (triangle) ──────────────────────────────────────────
    // Upward-pointing triangle; rotate 180° for down, -90° for left, +90° for right.
    // Used in ButtonSpinner, ComboBox, NumericUpDown, Expander, TreeView.

    /// <summary>Small upward chevron triangle (24×14 viewBox).</summary>
    public virtual string ChevronUp => "M 0 9 L 12 0 L 24 9 L 22 11 L 12 3 L 2 11 Z";

    /// <summary>Small downward chevron triangle — 180° rotation of ChevronUp.</summary>
    public virtual string ChevronDown => "M 0 2 L 12 11 L 24 2 L 22 0 L 12 8 L 2 0 Z";

    /// <summary>Small leftward chevron triangle — -90° rotation of ChevronUp.</summary>
    public virtual string ChevronLeft => "M 9 0 L 0 12 L 9 24 L 11 22 L 3 12 L 11 2 Z";

    /// <summary>Small rightward chevron triangle — +90° rotation of ChevronUp.</summary>
    public virtual string ChevronRight => "M 2 0 L 11 12 L 2 24 L 0 22 L 8 12 L 0 2 Z";

    // This is the same triangle the original ButtonSpinner used (M0,9 L10,0 20,9 19,10 10,2 1,10 z),
    // but normalised to 24×14 viewBox and written in a clean path grammar so all icons are consistent.

    // ── Expand / collapse ───────────────────────────────────────────
    // Chevron-style expand/collapse indicators. The expand variant is two
    // chevron-like segments; collapse is a single chevron pointing back.

    /// <summary>Chevron pointing one direction, used as the "collapsed" indicator in TreeView.</summary>
    public virtual string ExpandChevron => "M 1 1 L 10 10 L 19 1 L 18 0 L 10 8 L 2 0 Z";

    /// <summary>Chevron pointing another direction, used as the "expanded" indicator in TreeView.</summary>
    public virtual string CollapseChevron => "M 1 0 L 10 10 L 19 0 L 18 -1 L 10 8 L 2 -1 Z";

    // ── Drop-down arrow (used by ComboBox, DropDownButton, SplitButton) ──
    // A simple downward-pointing triangle. Same geometry as ChevronDown
    // but we keep the semantic alias.

    /// <summary>Drop-down arrow — same as ChevronDown.</summary>
    public virtual string DropDown => "M 0 2 L 12 11 L 24 2 L 22 0 L 12 8 L 2 0 Z";

    // ── Close / X ───────────────────────────────────────────────────
    // Standard close icon, useful for NotificationCard, TabItem close buttons, etc.

    /// <summary>X-shaped close icon (24×24 viewBox).</summary>
    public virtual string Close => "M 1 1 L 10 10 L 1 19 L 3 21 L 12 12 L 21 21 L 23 19 L 14 10 L 23 1 L 21 -1 L 12 8 L 3 -1 Z";

    // ── Checkmark ───────────────────────────────────────────────────
    // Used in CheckBox for the check state.

    /// <summary>Checkmark tick (24×24 viewBox).</summary>
    public virtual string Checkmark => "M 0 12 L 8 20 L 24 3 L 22 1 L 8 17 L 2 11 Z";

    // ── Hamburger / Menu ────────────────────────────────────────────
    // Three horizontal lines.

    /// <summary>Hamburger menu icon (24×24 viewBox). Three filled bars.</summary>
    public virtual string Menu => "M 0 2 L 24 2 L 24 6 L 0 6 Z M 0 10 L 24 10 L 24 14 L 0 14 Z M 0 18 L 24 18 L 24 22 L 0 22 Z";

    // ── Star (favourite) ────────────────────────────────────────────

    /// <summary>Five-pointed star (24×24 viewBox).</summary>
    public virtual string Star => "M 12 0 L 15 9 L 24 9 L 17 15 L 20 24 L 12 19 L 4 24 L 7 15 L 0 9 L 9 9 Z";

    // ── Arrow variants ──────────────────────────────────────────────
    // Unlike chevrons (angled lines), these have a tail/shaft.

    /// <summary>Left-pointing arrow (24×24 viewBox).</summary>
    public virtual string ArrowLeft => "M 24 11 L 5 11 L 11 5 L 9 3 L 1 12 L 9 21 L 11 19 L 5 13 L 24 13 Z";

    /// <summary>Right-pointing arrow (24×24 viewBox).</summary>
    public virtual string ArrowRight => "M 0 11 L 19 11 L 13 5 L 15 3 L 23 12 L 15 21 L 13 19 L 19 13 L 0 13 Z";

    /// <summary>Upward-pointing arrow (24×24 viewBox).</summary>
    public virtual string ArrowUp => "M 11 24 L 11 5 L 5 11 L 3 9 L 12 1 L 21 9 L 19 11 L 13 5 L 13 24 Z";

    /// <summary>Downward-pointing arrow (24×24 viewBox).</summary>
    public virtual string ArrowDown => "M 11 0 L 11 19 L 5 13 L 3 15 L 12 23 L 21 15 L 19 13 L 13 19 L 13 0 Z";

    // ── More / ellipsis ─────────────────────────────────────────────
    // Three dots, each a full circle of four quarter arcs. The earlier form closed each
    // dot onto a point a hundredth away from its start ("A 3 3 0 1 1 5 12.01"), which
    // Avalonia's parser collapses — see the note on Error.

    /// <summary>Horizontal ellipsis (24×24 viewBox).</summary>
    public virtual string MoreHorizontal =>
        "M 5 9 A 3 3 0 0 1 8 12 A 3 3 0 0 1 5 15 A 3 3 0 0 1 2 12 A 3 3 0 0 1 5 9 Z " +
        "M 12 9 A 3 3 0 0 1 15 12 A 3 3 0 0 1 12 15 A 3 3 0 0 1 9 12 A 3 3 0 0 1 12 9 Z " +
        "M 19 9 A 3 3 0 0 1 22 12 A 3 3 0 0 1 19 15 A 3 3 0 0 1 16 12 A 3 3 0 0 1 19 9 Z";

    /// <summary>Vertical ellipsis (24×24 viewBox).</summary>
    public virtual string MoreVertical =>
        "M 12 2 A 3 3 0 0 1 15 5 A 3 3 0 0 1 12 8 A 3 3 0 0 1 9 5 A 3 3 0 0 1 12 2 Z " +
        "M 12 9 A 3 3 0 0 1 15 12 A 3 3 0 0 1 12 15 A 3 3 0 0 1 9 12 A 3 3 0 0 1 12 9 Z " +
        "M 12 16 A 3 3 0 0 1 15 19 A 3 3 0 0 1 12 22 A 3 3 0 0 1 9 19 A 3 3 0 0 1 12 16 Z";

    // ── Plus / Add ──────────────────────────────────────────────────

    /// <summary>Plus sign (24×24 viewBox).</summary>
    public virtual string Plus => "M 11 0 L 13 0 L 13 11 L 24 11 L 24 13 L 13 13 L 13 24 L 11 24 L 11 13 L 0 13 L 0 11 L 11 11 Z";

    /// <summary>Minus sign (24×24 viewBox).</summary>
    public virtual string Minus => "M 0 11 L 24 11 L 24 13 L 0 13 Z";

    // ── Error / validation ──────────────────────────────────────────
    // A solid disc with the exclamation knocked out of it, so the icon reads at
    // icon size against any background. The disc is wound clockwise and the bar and
    // dot anti-clockwise, which is what punches them out under the nonzero fill rule.
    //
    // Circles are built from four quarter arcs, never two semicircles: Avalonia's path
    // parser collapses an arc whose endpoints are exactly one diameter apart to a
    // zero-width line, so "A 12 12 0 1 1 12 24" from (12,0) renders as a vertical stroke.

    /// <summary>Exclamation mark in a filled circle (24×24 viewBox).</summary>
    public virtual string Error =>
        "M 12 0 A 12 12 0 0 1 24 12 A 12 12 0 0 1 12 24 A 12 12 0 0 1 0 12 A 12 12 0 0 1 12 0 Z " +
        "M 10.5 5 L 10.5 14 L 13.5 14 L 13.5 5 Z " +
        "M 12 16 A 2 2 0 0 0 10 18 A 2 2 0 0 0 12 20 A 2 2 0 0 0 14 18 A 2 2 0 0 0 12 16 Z";
    // ── Save ────────────────────────────────────────────────────────
    // Floppy outline: frame, shutter and label. The frame is a clockwise outer path with an
    // anti-clockwise inner path punched out of it, the same nonzero trick the Error icon uses.

    /// <summary>Floppy-disk save icon (24×24 viewBox).</summary>
    public virtual string Save =>
        "M 2 2 L 17 2 L 22 7 L 22 22 L 2 22 Z " +
        "M 4 4 L 4 20 L 20 20 L 20 8 L 16 4 Z " +
        "M 7 4 L 15 4 L 15 9 L 7 9 Z " +
        "M 7 13 L 17 13 L 17 20 L 7 20 Z";

    // ── Refresh / reload ────────────────────────────────────────────
    // Three-quarter ring with an arrowhead on the open end. Built from quarter arcs for the
    // reason given on Error — a half-circle arc collapses to a line in Avalonia's parser.

    /// <summary>Circular reload arrow (24×24 viewBox).</summary>
    public virtual string Refresh =>
        "M 12 2 A 10 10 0 0 1 22 12 A 10 10 0 0 1 12 22 A 10 10 0 0 1 2 12 " +
        "L 4.5 12 A 7.5 7.5 0 0 0 12 19.5 A 7.5 7.5 0 0 0 19.5 12 A 7.5 7.5 0 0 0 12 4.5 Z " +
        "M 10 0 L 17 3.5 L 10 7 Z";
    // ── Window chrome ───────────────────────────────────────────────
    // Caption-button icons. Minimise reuses Minus and close reuses Close; only the box shapes
    // are particular to a title bar.

    /// <summary>Empty square, for a maximise caption button (24×24 viewBox).</summary>
    public virtual string WindowMaximize =>
        "M 2 2 L 22 2 L 22 22 L 2 22 Z " +
        "M 4 4 L 4 20 L 20 20 L 20 4 Z";

    /// <summary>Two offset squares, for a restore-down caption button (24×24 viewBox).</summary>
    public virtual string WindowRestore =>
        "M 7 2 L 22 2 L 22 17 L 17 17 L 17 15 L 20 15 L 20 4 L 9 4 L 9 7 L 7 7 Z " +
        "M 2 7 L 17 7 L 17 22 L 2 22 Z " +
        "M 4 9 L 4 20 L 15 20 L 15 9 Z";

    /// <summary>Arrows pushing outwards to opposite corners, for entering full screen (24×24 viewBox).</summary>
    public virtual string FullScreenEnter =>
        "M 2 2 L 10 2 L 7 5 L 11 9 L 9 11 L 5 7 L 2 10 Z " +
        "M 22 22 L 14 22 L 17 19 L 13 15 L 15 13 L 19 17 L 22 14 Z";

    // ── Reveal / conceal ────────────────────────────────────────────
    // The eye is an almond with the iris punched out and the pupil filled back in, so it reads
    // as an outline at icon size. EyeOff is the same shape with a stroke through it.

    /// <summary>Eye outline, for revealing a password (24×24 viewBox).</summary>
    public virtual string Eye =>
        "M 12 4 C 5 4 1 12 1 12 C 1 12 5 20 12 20 C 19 20 23 12 23 12 C 23 12 19 4 12 4 Z " +
        "M 12 7 A 5 5 0 0 0 7 12 A 5 5 0 0 0 12 17 A 5 5 0 0 0 17 12 A 5 5 0 0 0 12 7 Z " +
        "M 12 9.5 A 2.5 2.5 0 0 1 14.5 12 A 2.5 2.5 0 0 1 12 14.5 A 2.5 2.5 0 0 1 9.5 12 A 2.5 2.5 0 0 1 12 9.5 Z";

    /// <summary>Closed eye with lashes, for concealing a password (24×24 viewBox).</summary>
    /// <remarks>
    /// A closed lid rather than the eye struck through. The struck-through form needs the stroke to
    /// break the outline it crosses, which a single filled path cannot express — drawn as one shape
    /// it reads as a gouged blob at icon size, whichever way the stroke is wound.
    /// </remarks>
    public virtual string EyeOff =>
        "M 2 9 C 6 15 18 15 22 9 L 20.4 7.7 C 17 12.6 7 12.6 3.6 7.7 Z " +
        "M 4 13 L 5.6 14.2 L 3.4 17 L 1.8 15.8 Z " +
        "M 11 15.2 L 13 15.2 L 13 18.5 L 11 18.5 Z " +
        "M 18.4 14.2 L 20 13 L 22.2 15.8 L 20.6 17 Z";


}
