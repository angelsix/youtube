using System;
using AngelSix.ThemeEngine;
using Avalonia;
using Avalonia.Media;

namespace Avalonia.Themes.Skeleton.Theme;

/// <summary>
/// The smallest useful theme: one base size, a few colours and one font, plus the button's hover and press. Every value a control theme reads is a
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

    /// <summary>The ink a button takes under the pointer.</summary>
    public virtual Color HoverColor => Color.Parse("#2F6FEB");

    /// <summary>The ink a button takes while pressed.</summary>
    public virtual Color PressedColor => Color.Parse("#1A3F8F");

    /// <summary>How much a button grows under the pointer.</summary>
    public virtual double HoverScale => 1.05;

    /// <summary>How much a button shrinks while pressed.</summary>
    public virtual double PressedScale => 0.95;

    /// <summary>How long a state change takes to ease in.</summary>
    public virtual TimeSpan StateDuration => TimeSpan.FromMilliseconds(150);

    // Values the control themes read, all derived from the ones above

    public virtual SolidColorBrush TextBrush => new(TextColor);

    public virtual SolidColorBrush SurfaceBrush => new(SurfaceColor);

    public virtual SolidColorBrush HoverBrush => new(HoverColor);

    public virtual SolidColorBrush PressedBrush => new(PressedColor);

    public virtual double FontSize => 14 * BaseSize;

    public virtual Thickness BorderThickness => new(1 * BaseSize);

    public virtual Thickness ButtonPadding => new(12 * BaseSize, 6 * BaseSize);
}
