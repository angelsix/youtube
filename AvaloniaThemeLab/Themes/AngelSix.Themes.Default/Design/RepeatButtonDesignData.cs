using AngelSix.Mvvm;

namespace AngelSix.Themes.Default.Design;

/// <summary>Provides the design-time models the RepeatButton preview binds against.</summary>
internal static class RepeatButtonDesignData
{
    /// <summary>The model for the plain "Hold to repeat" example: it counts from the first click.</summary>
    public static RepeatButtonDesignModel HoldSample => new("Hold to repeat");

    /// <summary>The model for the "Delay 300 ms · Interval 100 ms" example.</summary>
    public static RepeatButtonDesignModel DelaySample => new("Delay 300 ms · Interval 100 ms");
}

/// <summary>One counting example: its label rises with every click, and with every repeat while the button is held.</summary>
internal sealed partial class RepeatButtonDesignModel(string prefix) : ObservableObject
{
    private int count;

    /// <summary>The label the button shows: its prefix and the running count.</summary>
    [Observable] public partial string Label { get; set; } = $"{prefix} · 0";

    [Command]
    private void Increment() => Label = $"{prefix} · {++count}";
}
