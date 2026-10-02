using System;
using System.Collections;
using System.ComponentModel;
using Avalonia.Controls;
using AvaloniaThemeLab.Pages;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>The DataValidationErrors page: a value that always reports one error, so the error template shows.</summary>
public sealed class DataValidationErrorsPageViewModel() : PageEntryViewModel(typeof(DataValidationErrors), typeof(DataValidationErrorsPage)), INotifyDataErrorInfo
{
    public string Value { get; set; } = "Field with a validation error";

    public bool HasErrors => true;

    // The error never changes, so there is nothing to raise
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged { add { } remove { } }

    public IEnumerable GetErrors(string? propertyName) =>
        propertyName == nameof(Value) ? (string[])["This value is not valid"] : [];
}
