using AngelSix.ThemeEngine;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// Holds the icon paths used across the theme library.
/// A theme exposes an instance via <see cref="DefaultTheme.Icons"/> so subclasses
/// can override the entire icon set while keeping the same token names.
/// </summary>
/// <remarks>
/// <para>
/// The properties are not written here. <c>SvgFolder</c> has the engine generate one per drawing in
/// <c>Icons/Svg</c>, named after the file — so an icon is added by adding a drawing, and the shape
/// is editable in something that can show it to you. See <c>Icons/Svg/Categories.md</c>.
/// </para>
/// <para>
/// Marked with <c>[IconGallery]</c> so the source generator finds it by convention. It used to be
/// located by its hardcoded full type name, which meant every project referencing this assembly
/// also matched it and re-emitted the whole icon set on top of this one.
/// </para>
/// </remarks>
[IconGallery(SvgFolder = "Icons/Svg")]
public partial class ThemeIconGallery
{
    // Anything that is not a drawing belongs here. An alias is the usual case, and wins over a file
    // of the same name:
    //
    //     public virtual string Foo => Bar;
}
