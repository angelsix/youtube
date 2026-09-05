using System.Globalization;
using AngelSix.ThemeEngine.Controls;

namespace Avalonia.Themes.Prototype.Tests;

public class CalendarSelectionEngineTests
{
    private static readonly DateTime Day10 = new(2026, 9, 10);
    private static readonly DateTime Day17 = new(2026, 9, 17);

    [Fact]
    public void First_click_opens_a_range_and_leaves_it_selecting()
    {
        var engine = new CalendarSelectionEngine();

        engine.Click(Day10);

        Assert.Equal(Day10, engine.RangeStart);
        Assert.Null(engine.RangeEnd);
        Assert.True(engine.IsSelecting);
    }

    [Fact]
    public void Second_click_commits_the_range()
    {
        var engine = new CalendarSelectionEngine();

        engine.Click(Day10);
        engine.Click(Day17);

        Assert.Equal(Day10, engine.RangeStart);
        Assert.Equal(Day17, engine.RangeEnd);
        Assert.False(engine.IsSelecting);
    }

    [Fact]
    public void Clicking_backwards_swaps_the_endpoints()
    {
        var engine = new CalendarSelectionEngine();

        engine.Click(Day17);
        engine.Click(Day10);

        Assert.Equal(Day10, engine.RangeStart);
        Assert.Equal(Day17, engine.RangeEnd);
    }

    [Fact]
    public void Click_on_a_committed_range_starts_a_fresh_one()
    {
        var engine = new CalendarSelectionEngine();
        engine.Click(Day10);
        engine.Click(Day17);

        var day22 = new DateTime(2026, 9, 22);
        engine.Click(day22);

        Assert.Equal(day22, engine.RangeStart);
        Assert.Null(engine.RangeEnd);
        Assert.True(engine.IsSelecting);
    }

    [Fact]
    public void Hover_while_selecting_previews_the_band_endpoints_included()
    {
        var engine = new CalendarSelectionEngine();
        engine.Click(Day10);

        engine.Hover(new DateTime(2026, 9, 14));

        Assert.True(engine.IsInPreview(Day10));
        Assert.True(engine.IsInPreview(new DateTime(2026, 9, 12)));
        Assert.True(engine.IsInPreview(new DateTime(2026, 9, 14)));
        Assert.False(engine.IsInPreview(new DateTime(2026, 9, 15)));
        Assert.False(engine.IsInPreview(new DateTime(2026, 9, 9)));
    }

    [Fact]
    public void Hover_before_the_start_previews_backwards()
    {
        var engine = new CalendarSelectionEngine();
        engine.Click(Day10);

        engine.Hover(new DateTime(2026, 9, 7));

        Assert.True(engine.IsInPreview(new DateTime(2026, 9, 7)));
        Assert.True(engine.IsInPreview(new DateTime(2026, 9, 9)));
        Assert.False(engine.IsInPreview(new DateTime(2026, 9, 11)));
    }

    [Fact]
    public void Hover_without_a_start_previews_nothing()
    {
        var engine = new CalendarSelectionEngine();

        engine.Hover(Day10);

        Assert.False(engine.IsInPreview(Day10));
    }

    [Fact]
    public void Committed_range_reports_only_strictly_inner_days_in_range()
    {
        var engine = new CalendarSelectionEngine();
        engine.Click(Day10);
        engine.Click(Day17);

        Assert.False(engine.IsInRange(Day10));
        Assert.False(engine.IsInRange(Day17));
        Assert.True(engine.IsInRange(new DateTime(2026, 9, 11)));
        Assert.True(engine.IsInRange(new DateTime(2026, 9, 16)));
        Assert.False(engine.IsInRange(new DateTime(2026, 9, 18)));
    }

    [Fact]
    public void Time_of_day_is_ignored_when_clicking()
    {
        var engine = new CalendarSelectionEngine();

        engine.Click(new DateTime(2026, 9, 10, 14, 30, 0));

        Assert.Equal(Day10, engine.RangeStart);
    }

    [Fact]
    public void Month_cells_fill_six_weeks_starting_on_the_first_day_of_week()
    {
        var cells = CalendarSelectionEngine.MonthCells(2026, 9, DayOfWeek.Monday);

        Assert.Equal(42, cells.Length);
        Assert.Equal(new DateTime(2026, 8, 31), cells[0]);
        Assert.Equal(DayOfWeek.Monday, cells[0].DayOfWeek);
        Assert.Equal(new DateTime(2026, 9, 1), cells[1]);
        Assert.Equal(new DateTime(2026, 10, 11), cells[41]);
    }

    [Fact]
    public void Month_cells_respect_a_sunday_week_start()
    {
        var cells = CalendarSelectionEngine.MonthCells(2026, 9, DayOfWeek.Sunday);

        Assert.Equal(DayOfWeek.Sunday, cells[0].DayOfWeek);
        Assert.Equal(new DateTime(2026, 8, 30), cells[0]);
    }

    [Fact]
    public void Day_titles_rotate_with_the_first_day_of_week()
    {
        var culture = CultureInfo.GetCultureInfo("en-GB");

        var titles = CalendarSelectionEngine.DayTitles(DayOfWeek.Wednesday, culture);

        Assert.Equal(7, titles.Length);
        Assert.Equal(culture.DateTimeFormat.GetShortestDayName(DayOfWeek.Wednesday), titles[0]);
        Assert.Equal(culture.DateTimeFormat.GetShortestDayName(DayOfWeek.Tuesday), titles[6]);
    }

    [Fact]
    public void Decade_start_floors_to_the_decade()
    {
        Assert.Equal(2020, CalendarSelectionEngine.DecadeStart(2026));
        Assert.Equal(2020, CalendarSelectionEngine.DecadeStart(2020));
        Assert.Equal(2010, CalendarSelectionEngine.DecadeStart(2019));
    }

    [Fact]
    public void Selectable_honours_min_max_and_predicate()
    {
        var min = new DateTime(2026, 9, 5);
        var max = new DateTime(2026, 9, 25);
        static bool Weekend(DateTime d) => d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        Assert.False(CalendarSelectionEngine.IsSelectable(new DateTime(2026, 9, 4), min, max, Weekend));
        Assert.False(CalendarSelectionEngine.IsSelectable(new DateTime(2026, 9, 26), min, max, Weekend));
        Assert.False(CalendarSelectionEngine.IsSelectable(new DateTime(2026, 9, 12), min, max, Weekend));
        Assert.True(CalendarSelectionEngine.IsSelectable(new DateTime(2026, 9, 10), min, max, Weekend));
        Assert.True(CalendarSelectionEngine.IsSelectable(new DateTime(2026, 9, 10), null, null, null));
    }
}
