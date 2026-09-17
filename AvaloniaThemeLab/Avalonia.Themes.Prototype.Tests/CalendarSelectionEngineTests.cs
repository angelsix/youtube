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

    // --- Week numbers ---

    [Theory]
    // ISO 8601: week 1 of 2023 starts on Monday 2 Jan 2023
    [InlineData(2023, 1, 2, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday, 1)]
    // 2022-12-31 is still in ISO week 52 of 2022
    [InlineData(2022, 12, 31, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday, 52)]
    // .NET bug: 2018-12-31 is a Monday and is ISO week 1 of 2019, not week 53 of 2018
    [InlineData(2018, 12, 31, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday, 1)]
    // 29 Dec 2014 (Monday) is ISO week 1 of 2015
    [InlineData(2014, 12, 29, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday, 1)]
    // US rule: week 1 always starts on Jan 1; 2023 has 53 Sunday-start weeks
    [InlineData(2023, 1, 1, CalendarWeekRule.FirstDay, DayOfWeek.Sunday, 1)]
    [InlineData(2023, 12, 31, CalendarWeekRule.FirstDay, DayOfWeek.Sunday, 53)]
    public void GetWeekOfYear_returns_the_expected_number_under_each_rule(
        int year, int month, int day, CalendarWeekRule rule, DayOfWeek firstDayOfWeek, int expectedWeek)
    {
        var culture = CultureInfo.GetCultureInfo("en-GB");
        var date = new DateTime(year, month, day);

        Assert.Equal(expectedWeek, CalendarSelectionEngine.GetWeekOfYear(date, rule, firstDayOfWeek, culture));
    }

    [Fact]
    public void Week_numbers_align_with_the_month_grid_rows()
    {
        var culture = CultureInfo.GetCultureInfo("en-GB");
        // September 2026 begins on a Tuesday; with a Monday start the grid's first row is
        // Mon 31 Aug through Sun 6 Sep — ISO week 36.
        var cells = CalendarSelectionEngine.MonthCells(2026, 9, DayOfWeek.Monday);

        var numbers = CalendarSelectionEngine.WeekNumbers(cells, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday, culture);

        Assert.Equal(6, numbers.Length);
        // Row zero begins Mon 31 Aug (ISO 36); row four begins Mon 28 Sep (ISO 40); row five
        // spills into October (Mon 5 Oct, ISO 41) — the gutter must number the ROW, not the month.
        Assert.Equal(ISOWeek.GetWeekOfYear(new DateTime(2026, 8, 31)), int.Parse(numbers[0]));
        Assert.Equal(ISOWeek.GetWeekOfYear(new DateTime(2026, 9, 7)), int.Parse(numbers[1]));
        Assert.Equal(ISOWeek.GetWeekOfYear(new DateTime(2026, 9, 28)), int.Parse(numbers[4]));
        Assert.Equal(ISOWeek.GetWeekOfYear(new DateTime(2026, 10, 5)), int.Parse(numbers[5]));
    }

    [Fact]
    public void Week_numbers_follow_a_sunday_week_start()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var cells = CalendarSelectionEngine.MonthCells(2026, 9, DayOfWeek.Sunday);

        var numbers = CalendarSelectionEngine.WeekNumbers(cells, CalendarWeekRule.FirstDay, DayOfWeek.Sunday, culture);

        Assert.Equal(6, numbers.Length);
        Assert.All(numbers, n => Assert.Matches(@"^\d+$", n));
        // Non-ISO rules delegate to the BCL calendar — each row's number must agree with the raw
        // CultureCalendar computation for that row's first day.
        for (var row = 0; row < 6; row++)
            Assert.Equal(
                culture.Calendar.GetWeekOfYear(cells[row * 7], CalendarWeekRule.FirstDay, DayOfWeek.Sunday),
                int.Parse(numbers[row]));
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
