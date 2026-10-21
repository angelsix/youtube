using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Avalonia.Themes.Default.Design;

// Design-time sample data for the RepeatButton preview. A RepeatButton is BEHAVIOUR - it fires
// repeatedly while it is held - so a still label cannot show what it does; the counting examples
// bind their button's Content and Command to the model below and their number rises while held.
// The house rule applies: the sample state lives in C#, never inline in markup. Nothing here is
// public API - every type is internal.
//
// Each example gets its OWN model: the provider properties below return a NEW model every time they
// are read, and the preview template is instantiated once per pane, so the light pane, the dark pane
// and the two counting examples each count on their own.

/// <summary>Provides the design-time models the RepeatButton preview binds against.</summary>
internal static class RepeatButtonDesignData
{
    /// <summary>The model for the plain "Hold to repeat" example: it counts from the first click.</summary>
    public static RepeatButtonDesignModel HoldSample => new("Hold to repeat");

    /// <summary>The model for the "Delay 300 ms · Interval 100 ms" example.</summary>
    public static RepeatButtonDesignModel DelaySample => new("Delay 300 ms · Interval 100 ms");
}

/// <summary>Holds the running count of one counting preview example.</summary>
internal sealed class RepeatButtonDesignModel : INotifyPropertyChanged
{
    private int _count;

    public RepeatButtonDesignModel(string prefix)
    {
        Prefix = prefix;
        Increment = new IncrementCommand(this);
    }

    /// <summary>The label the example starts from ("Hold to repeat").</summary>
    public string Prefix { get; }

    /// <summary>How many times the example's button has fired.</summary>
    public int Count
    {
        get => _count;
        private set
        {
            if (_count == value)
                return;
            _count = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Label));
        }
    }

    /// <summary>The label the button shows: its prefix and the running count.</summary>
    public string Label => $"{Prefix} · {_count}";

    /// <summary>The command the button runs on every click, including every repeat while held.</summary>
    public ICommand Increment { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>Increments the count; the button's own repeat behaviour does the rest.</summary>
    private sealed class IncrementCommand(RepeatButtonDesignModel model) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => model.Count++;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
