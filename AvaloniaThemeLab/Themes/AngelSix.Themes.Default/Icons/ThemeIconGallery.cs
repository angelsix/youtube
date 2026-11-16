using AngelSix.ThemeEngine;

namespace AngelSix.Themes.Default;

/// <summary>Holds the icon paths used across the theme library. A custom gallery for a theme extending <see cref="DefaultTheme"/> derives from this class — <c>DefaultTheme.Icons</c> hands it out — and inherits every icon here: add one with <c>public virtual string BrandStar =&gt; "…"</c>, redraw one with <c>public override string ChevronDown =&gt; "…"</c>.</summary>
[IconGallery(SvgFolder = "Icons/Svg")]
public partial class ThemeIconGallery
{
    // Anything that is not a drawing belongs here.
}
