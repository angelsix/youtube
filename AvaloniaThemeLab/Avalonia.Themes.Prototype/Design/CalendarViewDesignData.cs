using AngelSix.ThemeEngine.Controls;

namespace Avalonia.Themes.Prototype.Design;

// Design-time sample data for the CalendarView preview. Honours the house rule that sample
// collections live in C#, never inline in markup: the preview binds a NAMED property to this
// static provider (Events="{Binding Events}", DataContext set explicitly in the preview block).
// Internal so none of this leaks into the theme's public API; the designer resolves it in-assembly.
internal static class CalendarViewDesignData
{
    /// <summary>The design-time model the preview binds against.</summary>
    public static CalendarViewDesignModel Sample => new();
}

/// <summary>Holds the sample event collection the preview binds to.</summary>
internal sealed class CalendarViewDesignModel
{
    /// <summary>Always dated inside the CURRENT month (days 5, 12 and 20 exist in every month),
    /// so the dots stay visible whenever the designer opens — the preview calendar displays
    /// today's month.</summary>
    public IReadOnlyList<CalendarEvent> Events { get; } = Build();

    private static IReadOnlyList<CalendarEvent> Build()
    {
        var year = DateTime.Today.Year;
        var month = DateTime.Today.Month;

        return
        [
            new CalendarEvent
            {
                Date = new DateTime(year, month, 5),
                Time = new TimeSpan(9, 30, 0),
                Title = "Team stand-up",
                Description = "Weekly sync with the theme team."
            },
            // Concrete subtype threading sub-data (the meeting id) through TData, beside a plain
            // base event — exercises the extension point end to end.
            new SampleMeeting
            {
                Date = new DateTime(year, month, 12),
                Time = new TimeSpan(14, 0, 0),
                Title = "Client review",
                Description = "Walkthrough of the new control set.",
                Data = Guid.NewGuid()
            },
            new CalendarEvent
            {
                Date = new DateTime(year, month, 12),
                Time = new TimeSpan(16, 30, 0),
                Title = "Coffee with Sam",
                Description = string.Empty
            },
            new CalendarEvent
            {
                Date = new DateTime(year, month, 20),
                Time = new TimeSpan(10, 0, 0),
                Title = "Ship v1.1",
                Description = "Publish the controls package."
            }
        ];
    }
}

/// <summary>A consumer-defined event subtype carrying its own payload via TData.</summary>
internal sealed class SampleMeeting : CalendarEvent<Guid>;
