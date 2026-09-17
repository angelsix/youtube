using System;
using System.Globalization;
using AngelSix.ThemeEngine.Controls;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaThemeLab;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Weekends unselectable on the single-selection demo calendar
        DemoCalendar.IsDateDisabled = date => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        // ISO 8601 week numbers beside the month grid
        DemoCalendar.IsWeekNumberVisible = true;
        DemoCalendar.WeekNumberRule = CalendarWeekRule.FirstFourDayWeek;
        DemoCalendar.FirstDayOfWeek = DayOfWeek.Monday;

        // Example events drive the dots under their day numbers; hovering a dotted cell shows
        // each event's title, date-time and description. One carries application data through the
        // generic subclass to demonstrate the extension point. Each date is the NEXT occurrence
        // of a named weekday, so every one lands on a selectable day despite the weekend blackout.
        static DateTime NextWeekday(DayOfWeek day)
        {
            var date = DateTime.Today;
            while (date.DayOfWeek != day)
                date = date.AddDays(1);
            return date;
        }

        var monday = NextWeekday(DayOfWeek.Monday);
        var wednesday = NextWeekday(DayOfWeek.Wednesday);
        var friday = NextWeekday(DayOfWeek.Friday);

        DemoCalendar.Events =
        [
            new CalendarEvent
            {
                Date = monday,
                Time = new TimeSpan(9, 30, 0),
                Title = "Team stand-up",
                Description = "Weekly sync with the theme team."
            },
            new CalendarEvent<Guid>
            {
                Date = wednesday,
                Time = new TimeSpan(14, 0, 0),
                Title = "Client review",
                Description = "Walkthrough of the new control set.",
                Data = Guid.NewGuid()
            },
            new CalendarEvent
            {
                Date = wednesday,
                Time = new TimeSpan(16, 30, 0),
                Title = "Coffee with Sam",
                Description = ""
            },
            new CalendarEvent
            {
                Date = friday,
                Time = new TimeSpan(10, 0, 0),
                Title = "Ship v1.1",
                Description = "Publish the controls package."
            }
        ];
    }
}
