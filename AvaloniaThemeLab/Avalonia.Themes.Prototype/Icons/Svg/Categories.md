# Theme icons

These SVG files are the icon set. `ThemeIconGallery` is marked
`[IconGallery(SvgFolder = "Icons/Svg")]`, and AngelSix.ThemeEngine's source generator turns each
file into one `virtual string` property named after it — so XAML's `{icons:ChevronUp}` and
IntelliSense still see plain C# strings.

Adding an icon is adding a file. `ChevronUp.svg` becomes `ChevronUp`; there is no list to update,
and nothing is written into the source tree — the properties are generated into `obj` on build, and
into IntelliSense as you type.

What the generator reads from a file:

- every `<path d="…">`, in document order, concatenated into the one geometry string. Subpaths
  punch each other out under the nonzero fill rule, which is what the outlined icons rely on, so
  their order matters.
- `<title>` becomes the property's `<summary>`, `<desc>` becomes its `<remarks>`.
- `data-category` groups the property under a heading in the generated file. The headings below are
  documentation for whoever is drawing; the generator only reads the attribute.

Only `<path>` is understood. `<circle>`, `<rect>`, `<line>` and `transform` are not, and are
reported as build warnings (ASTE014/ASTE015) rather than silently dropped — convert them to paths
first. Nothing is generated at all while any error stands, so you never get a half-populated gallery.

A `PathIcon` fills its geometry and never strokes it — see `Controls/PathIcon.axaml`, which binds
`Path.Fill` and sets no `Stroke`. Most downloadable icon sets draw with `stroke` and `fill="none"`,
and their path data is a centreline rather than an outline, so filling it gives a blob or nothing.
Convert stroke to outline before importing, or take a set drawn as filled shapes.

Build circles from four quarter arcs. A single arc command is defined by its endpoints, so a full
circle needs more than one, and the trick of closing onto a point a fraction from the start
(`A 3 3 0 1 1 5 12.01`) gives the major arc of a tiny chord rather than a circle — which is what the
first version of the ellipsis dots did. Two semicircles render correctly on Avalonia 12.0.1, so the
older "never two semicircles" rule in this theme no longer holds; verified by rendering both forms
and comparing filled pixels.

Icons are drawn on a nominal 24×24 grid. A `viewBox` wider than that means the shape genuinely
spills outside it.

To alias one icon to another, write it by hand in `ThemeIconGallery` rather than duplicating the
drawing — a hand-written property wins over a file of the same name (ASTE019).

## Categories

### Chevron (triangle)

Upward-pointing triangle; rotate 180° for down, -90° for left, +90° for right.
Used in ButtonSpinner, ComboBox, NumericUpDown, Expander, TreeView.

This is the same triangle the original ButtonSpinner used (M0,9 L10,0 20,9 19,10 10,2 1,10 z),
but normalised and written in a clean path grammar so all icons are consistent.

### Expand / collapse

Chevron-style expand/collapse indicators. The expand variant is two
chevron-like segments; collapse is a single chevron pointing back.

### Drop-down arrow (used by ComboBox, DropDownButton, SplitButton)

A simple downward-pointing triangle. Same geometry as ChevronDown, kept as a semantic alias.

### Close / X

Standard close icon, useful for NotificationCard, TabItem close buttons, etc.

### Checkmark

Used in CheckBox for the check state.

### Hamburger / Menu

Three horizontal lines.

### Star (favourite)

### Arrow variants

Unlike chevrons (angled lines), these have a tail/shaft.

### More / ellipsis

Three dots, each a full circle of four quarter arcs. The earlier form closed each
dot onto a point a hundredth away from its start ("A 3 3 0 1 1 5 12.01"), which is the major arc of
a 3.01 chord rather than a circle — it renders about a fifth smaller and off-centre.

### Plus / Add

### Error / validation

A solid disc with the exclamation knocked out of it, so the icon reads at
icon size against any background. The disc is wound clockwise and the bar and
dot anti-clockwise, which is what punches them out under the nonzero fill rule.

### Save

Floppy outline: frame, shutter and label. The frame is a clockwise outer path with an
anti-clockwise inner path punched out of it, the same nonzero trick the Error icon uses.

### Refresh / reload

Three-quarter ring with an arrowhead on the open end, built from quarter arcs like the other
round shapes here.

### Window chrome

Caption-button icons. Minimise reuses Minus and close reuses Close; only the box shapes
are particular to a title bar.

### Reveal / conceal

The eye is an almond with the iris punched out and the pupil filled back in, so it reads
as an outline at icon size. EyeOff is the same shape with a stroke through it.
