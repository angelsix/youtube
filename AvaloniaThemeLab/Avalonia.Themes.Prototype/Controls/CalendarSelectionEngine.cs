using System.Globalization;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// The pure selection and layout logic behind <see cref="CalendarView"/> — range state machine,
/// month-grid date maths and header text — kept free of any UI type so it can be unit tested.
/// </summary>
public sealed class CalendarSelectionEngine
{
    /// <summary>Cells in a month grid: six weeks of seven days.</summary>
    public const int MonthCellCount = 42;

    /// <summary>Cells in a zoomed-out grid: twelve months, or a decade padded to twelve years.</summary>
    public const int ZoomCellCount = 12;

    /// <summary>The committed start of the range, or the pending start while selecting.</summary>
    public DateTime? RangeStart { get; private set; }

    /// <summary>The committed end of the range; null while a start awaits its end.</summary>
    public DateTime? RangeEnd { get; private set; }

    /// <summary>The hovered candidate end while selecting, driving the preview band.</summary>
    public DateTime? PreviewDate { get; private set; }

    /// <summary>True after a start is picked and before the end commits.</summary>
    public bool IsSelecting => RangeStart is not null && RangeEnd is null;

    /// <summary>
    /// Applies a day click to the range state machine. First click opens a range at that day;
    /// second click commits it (swapping ends if clicked backwards); a click on a committed
    /// range starts a fresh one.
    /// </summary>
    public void Click(DateTime date)
    {
        date = date.Date;

        if (!IsSelecting)
        {
            RangeStart = date;
            RangeEnd = null;
            PreviewDate = null;
            return;
        }

        var start = RangeStart!.Value;
        (RangeStart, RangeEnd) = date < start ? (date, start) : (start, date);
        PreviewDate = null;
    }

    /// <summary>Records the hovered day; only meaningful while selecting.</summary>
    public void Hover(DateTime date) => PreviewDate = IsSelecting ? date.Date : null;

    /// <summary>Clears the hover preview, e.g. when the pointer leaves the grid.</summary>
    public void ClearHover() => PreviewDate = null;

    /// <summary>Clears the whole range state.</summary>
    public void Reset()
    {
        RangeStart = null;
        RangeEnd = null;
        PreviewDate = null;
    }

    /// <summary>Restores committed state from bound properties without touching the preview.</summary>
    public void Load(DateTime? start, DateTime? end)
    {
        RangeStart = start?.Date;
        RangeEnd = end?.Date;
    }

    /// <summary>True when the day sits strictly between the committed start and end.</summary>
    public bool IsInRange(DateTime date) =>
        RangeStart is { } start && RangeEnd is { } end &&
        date.Date > start && date.Date < end;

    /// <summary>True when the day sits inside the hover preview band, endpoints included.</summary>
    public bool IsInPreview(DateTime date)
    {
        if (!IsSelecting || PreviewDate is not { } preview)
            return false;

        var start = RangeStart!.Value;
        var (low, high) = preview < start ? (preview, start) : (start, preview);
        return date.Date >= low && date.Date <= high;
    }

    /// <summary>
    /// The forty-two dates filling a six-week month grid, starting on the week containing the
    /// first of the month.
    /// </summary>
    public static DateTime[] MonthCells(int year, int month, DayOfWeek firstDayOfWeek)
    {
        var first = new DateTime(year, month, 1);
        var lead = ((int)first.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        var start = first.AddDays(-lead);

        var cells = new DateTime[MonthCellCount];
        for (var i = 0; i < MonthCellCount; i++)
            cells[i] = start.AddDays(i);

        return cells;
    }

    /// <summary>The seven abbreviated day titles in display order for the given first day.</summary>
    public static string[] DayTitles(DayOfWeek firstDayOfWeek, CultureInfo culture)
    {
        var titles = new string[7];
        for (var i = 0; i < 7; i++)
            titles[i] = culture.DateTimeFormat.GetShortestDayName((DayOfWeek)(((int)firstDayOfWeek + i) % 7));

        return titles;
    }

    /// <summary>The first year of the decade containing the given year.</summary>
    public static int DecadeStart(int year) => year - year % 10;

    /// <summary>The header label for the current view: month name, year, or decade span.</summary>
    public static string HeaderText(DateTime displayDate, CalendarViewMode mode, CultureInfo culture) => mode switch
    {
        CalendarViewMode.Month => displayDate.ToString("MMMM yyyy", culture),
        CalendarViewMode.Year => displayDate.ToString("yyyy", culture),
        _ => $"{DecadeStart(displayDate.Year)} – {DecadeStart(displayDate.Year) + 9}"
    };

    /// <summary>True when the day is selectable given the bounds and the disabled predicate.</summary>
    public static bool IsSelectable(DateTime date, DateTime? minDate, DateTime? maxDate, Func<DateTime, bool>? isDisabled)
    {
        date = date.Date;

        if (minDate is { } min && date < min.Date)
            return false;

        if (maxDate is { } max && date > max.Date)
            return false;

        return isDisabled is null || !isDisabled(date);
    }
}
