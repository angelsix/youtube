using System;
using Avalonia.Controls;
using AvaloniaThemeLab.Pages;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>The DatePicker page, with a fixed date so every capture of it shows the same value.</summary>
public sealed class DatePickerPageViewModel() : PageEntryViewModel(typeof(DatePicker), typeof(DatePickerPage))
{
    public DateTimeOffset SelectedDate { get; } = new(2026, 8, 25, 0, 0, 0, TimeSpan.Zero);
}
