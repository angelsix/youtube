using AngelSix.ThemeEngine;

namespace AngelSix.Themes.Default;

/// <summary>Holds the icon paths used across the theme library. A custom gallery handed to a theme that extends <see cref="DefaultTheme"/> derives from this class — <c>DefaultTheme.Icons</c> is narrowed to this type, so a bare gallery fails CS1715. Deriving keeps every icon here and lets you add (<c>public virtual string BrandStar =&gt; "…"</c>) or replace (<c>public override string ChevronDown =&gt; "…"</c>) by name.</summary>
[IconGallery(SvgFolder = "Icons/Svg")]
public partial class ThemeIconGallery
{
    // Anything that is not a drawing belongs here.
}
