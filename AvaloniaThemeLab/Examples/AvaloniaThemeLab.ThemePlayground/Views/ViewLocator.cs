using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Pages;

namespace AvaloniaThemeLab.ThemePlayground.Views;

/// <summary>Creates a fresh view and view model for the registered hand-written Window page.</summary>
public sealed class ViewLocator(Func<WindowPage> createWindowPage) : IDataTemplate
{
    public Control Build(object? param) => param switch
    {
        WindowPageViewModel => createWindowPage(),
        _ => throw new ArgumentException($"No gallery view is registered for {param?.GetType().FullName ?? "null"}.", nameof(param)),
    };

    public bool Match(object? data) => data is WindowPageViewModel;
}
