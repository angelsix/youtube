using AngelSix.ThemeEngine;
using AngelSix.Themes.Default;

namespace AvaloniaThemeLab.BuildingWidgets.Theme;

/// <summary>
/// Icon gallery for <see cref="PrototypeTheme"/>, inheriting every built-in icon from
/// <see cref="ThemeIconGallery"/> and adding the thin-line filled glyphs used by the widgets.
/// </summary>
[IconGallery]
public partial class PrototypeIconGallery : ThemeIconGallery
{
    /// <summary>Thin 3-bar hamburger menu overriding the default icon.</summary>
    public override string Menu =>
        "M 3 5.25 H 21 V 6.75 H 3 Z M 3 11.25 H 21 V 12.75 H 3 Z M 3 17.25 H 21 V 18.75 H 3 Z";

    /// <summary>Magnifying glass search icon.</summary>
    public virtual string Search =>
        "M 10.5 3 A 7.5 7.5 0 1 0 15.24 16.3 L 19.97 21.03 L 21.03 19.97 L 16.3 15.24 A 7.5 7.5 0 0 0 10.5 3 Z M 10.5 4.5 A 6 6 0 1 1 10.5 16.5 A 6 6 0 1 1 10.5 4.5 Z";

    /// <summary>House / Home icon.</summary>
    public virtual string Home =>
        "M 12 2.9 L 2.5 10.6 L 3.45 11.77 L 5 10.51 V 21 H 10.5 V 15.5 H 13.5 V 21 H 19 V 10.51 L 20.55 11.77 L 21.5 10.6 L 12 2.9 Z M 12 4.84 L 17.5 9.3 V 19.5 H 15 V 14 H 9 V 19.5 H 6.5 V 9.3 L 12 4.84 Z";

    /// <summary>4-tile dashboard grid icon.</summary>
    public virtual string Dashboard =>
        "M 3 3 H 10.5 V 12 H 3 Z M 4.5 4.5 V 10.5 H 9 V 4.5 Z M 13.5 3 H 21 V 8.5 H 13.5 Z M 15 4.5 V 7 H 19.5 V 4.5 Z M 13.5 11.5 H 21 V 21 H 13.5 Z M 15 13 V 19.5 H 19.5 V 13 Z M 3 15 H 10.5 V 21 H 3 Z M 4.5 16.5 V 19.5 H 9 V 16.5 Z";

    /// <summary>Orders / archive box icon.</summary>
    public virtual string Orders =>
        "M 5.6 3 L 3.25 7.35 V 21 H 20.75 V 7.35 L 18.4 3 H 5.6 Z M 6.5 4.5 H 17.5 L 18.85 7 H 5.15 L 6.5 4.5 Z M 4.75 8.5 H 19.25 V 19.5 H 4.75 V 8.5 Z M 9 11.25 V 12.75 H 15 V 11.25 H 9 Z";

    /// <summary>Delivery truck / shipments icon.</summary>
    public virtual string Shipments =>
        "M 2 5.5 V 17 H 4.2 A 2.75 2.75 0 0 0 9.3 17 H 14.7 A 2.75 2.75 0 0 0 19.8 17 H 22 V 12.4 L 18.3 8.5 H 15 V 5.5 H 2 Z M 3.5 7 H 13.5 V 15.5 H 9.15 A 2.75 2.75 0 0 0 4.35 15.5 H 3.5 V 7 Z M 15 10 H 17.65 L 20.5 13 V 15.5 H 19.65 A 2.75 2.75 0 0 0 15 14.6 V 10 Z M 6.75 15.25 A 1.25 1.25 0 1 1 6.75 17.75 A 1.25 1.25 0 1 1 6.75 15.25 Z M 17.25 15.25 A 1.25 1.25 0 1 1 17.25 17.75 A 1.25 1.25 0 1 1 17.25 15.25 Z";

    /// <summary>Sunburst / Settings gear icon.</summary>
    public virtual string Settings =>
        "M 12 8.25 A 3.75 3.75 0 1 0 12 15.75 A 3.75 3.75 0 1 0 12 8.25 Z M 12 9.75 A 2.25 2.25 0 1 1 12 14.25 A 2.25 2.25 0 1 1 12 9.75 Z M 11.25 2 H 12.75 V 5.5 H 11.25 Z M 11.25 18.5 H 12.75 V 22 H 11.25 Z M 2 11.25 H 5.5 V 12.75 H 2 Z M 18.5 11.25 H 22 V 12.75 H 18.5 Z M 4.4 5.46 L 5.46 4.4 L 7.9 6.84 L 6.84 7.9 Z M 16.1 17.16 L 17.16 16.1 L 19.6 18.54 L 18.54 19.6 Z M 4.4 18.54 L 6.84 16.1 L 7.9 17.16 L 5.46 19.6 Z M 16.1 6.84 L 18.54 4.4 L 19.6 5.46 L 17.16 7.9 Z";

    /// <summary>Clock icon for weather/time header.</summary>
    public virtual string Clock =>
        "M 12 2.5 A 9.5 9.5 0 1 0 12 21.5 A 9.5 9.5 0 1 0 12 2.5 Z M 12 4 A 8 8 0 1 1 12 20 A 8 8 0 1 1 12 4 Z M 11.25 7 V 12.4 L 14.8 14.8 L 15.65 13.55 L 12.75 11.6 V 7 H 11.25 Z";

    /// <summary>Sun glyph (filled disc + rays) for light-mode toggle and weather card.</summary>
    public virtual string Sun =>
        "M 12 7 A 5 5 0 1 0 12 17 A 5 5 0 1 0 12 7 Z M 11.25 1.5 H 12.75 V 4.75 H 11.25 Z M 11.25 19.25 H 12.75 V 22.5 H 11.25 Z M 1.5 11.25 H 4.75 V 12.75 H 1.5 Z M 19.25 11.25 H 22.5 V 12.75 H 19.25 Z M 4.1 5.16 L 5.16 4.1 L 7.45 6.39 L 6.39 7.45 Z M 16.55 17.61 L 17.61 16.55 L 19.9 18.84 L 18.84 19.9 Z M 4.1 18.84 L 6.39 16.55 L 7.45 17.61 L 5.16 19.9 Z M 16.55 6.39 L 18.84 4.1 L 19.9 5.16 L 17.61 7.45 Z";

    /// <summary>Crescent moon glyph for dark-mode toggle and weather card.</summary>
    public virtual string Moon =>
        "M 20.5 14.2 A 8.8 8.8 0 1 1 9.8 3.5 A 7 7 0 0 0 20.5 14.2 Z";
}
