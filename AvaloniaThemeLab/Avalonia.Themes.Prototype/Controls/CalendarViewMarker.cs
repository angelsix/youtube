using Avalonia.Media;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// One event marker on a <see cref="CalendarView"/> day, rendered as a dot under the day number.
/// </summary>
/// <remarks>
/// Markers are deliberately just a date and an optional brush: richer event display is a separate
/// concern for a future control that binds the same data. A null <see cref="Brush"/> renders in the
/// calendar's accent colour.
/// </remarks>
public sealed class CalendarViewMarker
{
    /// <summary>The day the marker belongs to. Time of day is ignored.</summary>
    public DateTime Date { get; set; }

    /// <summary>The dot colour, or null to use the calendar's accent.</summary>
    public IBrush? Brush { get; set; }
}
