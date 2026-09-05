using System;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Themes.Prototype;

namespace AvaloniaThemeLab;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Weekends unselectable on the single-selection demo calendar
        DemoCalendar.IsDateDisabled = date => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        // Event dots on three days around today
        var today = DateTime.Today;
        DemoCalendar.Markers =
        [
            new CalendarViewMarker { Date = today.AddDays(1), Brush = Brushes.Crimson },
            new CalendarViewMarker { Date = today.AddDays(3), Brush = Brushes.SeaGreen },
            new CalendarViewMarker { Date = today.AddDays(3), Brush = Brushes.SteelBlue },
            new CalendarViewMarker { Date = today.AddDays(10) }
        ];
    }
}
