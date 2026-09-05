namespace Avalonia.Themes.Prototype;

/// <summary>
/// How a <see cref="CalendarView"/> responds to day clicks.
/// </summary>
public enum CalendarViewSelectionMode
{
    /// <summary>Days cannot be selected; the calendar is display-only.</summary>
    None,

    /// <summary>One day at a time, bound through <see cref="CalendarView.SelectedDate"/>.</summary>
    Single,

    /// <summary>A start and end day, bound through <see cref="CalendarView.RangeStart"/> and
    /// <see cref="CalendarView.RangeEnd"/>.</summary>
    Range
}
