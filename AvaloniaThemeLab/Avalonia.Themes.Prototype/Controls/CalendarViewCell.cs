using Avalonia.Controls;
using Avalonia.Media;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// One cell in a <see cref="CalendarView"/> grid — a day in month view, or a month or year in the
/// zoomed-out views. Created by the calendar, never by consumers; all interaction is handled by
/// the owning calendar, and state reaches the theme purely through pseudo-classes.
/// </summary>
/// <remarks>
/// Pseudo-classes: <c>:today</c>, <c>:selected</c>, <c>:inactive</c> (outside the displayed
/// month or decade), <c>:blackout</c> (outside bounds or disabled by predicate),
/// <c>:range-start</c>, <c>:range-end</c>, <c>:in-range</c>, <c>:preview</c> (hover preview band)
/// and <c>:day-focused</c> (keyboard focus).
/// </remarks>
public sealed class CalendarViewCell : ContentControl
{
    /// <summary>Defines the <see cref="Markers"/> property.</summary>
    public static readonly StyledProperty<IReadOnlyList<IBrush>?> MarkersProperty =
        AvaloniaProperty.Register<CalendarViewCell, IReadOnlyList<IBrush>?>(nameof(Markers));

    /// <summary>The dot brushes shown under the cell content, one per event marker.</summary>
    public IReadOnlyList<IBrush>? Markers
    {
        get => GetValue(MarkersProperty);
        set => SetValue(MarkersProperty, value);
    }

    /// <summary>The date this cell represents (first of month or year for zoom cells).</summary>
    internal DateTime Date { get; set; }

    /// <summary>Sets or clears one of the cell's state pseudo-classes.</summary>
    internal void SetState(string pseudoClass, bool value) => PseudoClasses.Set(pseudoClass, value);
}
