using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Media;

namespace Avalonia.Themes.Skeleton.Theme;

/// <summary>
/// The smallest useful theme: one base size, two colours and one font. Every value a control theme reads is a
/// public property here, and the engine turns each one into a markup extension: <c>{size:ButtonPadding}</c>,
/// <c>{color:TextBrush}</c>, <c>{theme:FontFamily}</c>.
/// </summary>
/// <remarks>
/// Change a value here and every control that reads it follows. Subclass this class and override a property to make
/// a variant. Add a property when a control theme needs a value this class does not have yet.
/// </remarks>
[Theme]
public partial class SkeletonTheme
{
    /// <summary>The one size every other size is a multiple of.</summary>
    public virtual double BaseSize => 1.0;

    /// <summary>The ink for text and outlines.</summary>
    public virtual Color TextColor => Color.Parse("#1E1E1E");

    /// <summary>The window's canvas.</summary>
    public virtual Color SurfaceColor => Color.Parse("#FAFAFA");

    /// <summary>The one font.</summary>
    public virtual FontFamily FontFamily => new("fonts:Inter#Inter, $Default");

    // Values the control themes read, all derived from the four above

    public virtual SolidColorBrush TextBrush => new(TextColor);

    public virtual SolidColorBrush SurfaceBrush => new(SurfaceColor);

    public virtual double FontSize => 14 * BaseSize;

    public virtual Thickness BorderThickness => new(1 * BaseSize);

    public virtual Thickness ButtonPadding => new(12 * BaseSize, 6 * BaseSize);
}
