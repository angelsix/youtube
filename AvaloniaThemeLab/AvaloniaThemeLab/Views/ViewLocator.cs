using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaThemeLab.ViewModels;

namespace AvaloniaThemeLab.Views;

/// <summary>Builds the hand-written page a <see cref="PageEntryViewModel"/> names; the entry stays its DataContext.</summary>
public sealed class ViewLocator : IDataTemplate
{
    public Control Build(object? param) => (Control)Activator.CreateInstance(((PageEntryViewModel)param!).PageType)!;

    public bool Match(object? data) => data is PageEntryViewModel;
}
