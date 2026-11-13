using System;
using System.Collections.Generic;
using Avalonia.Controls;
using AvaloniaThemeLab.ThemePlayground.Pages;

namespace AvaloniaThemeLab.ThemePlayground.Views;

/// <summary>Opens the Window page's dialogs and modal page, so its view model never constructs a view.</summary>
public sealed class DialogWindowService(Func<TopLevel?> owner)
{
    private readonly Stack<DialogWindow> _open = new();

    /// <summary>The dialogs currently open, topmost first.</summary>
    public IReadOnlyCollection<DialogWindow> OpenDialogs => _open;

    public void OpenDialog(object? dataContext)
    {
        var over = _open.Count > 0 ? _open.Peek() : owner() as Window;
        if (over is null)
            return;

        var dialog = new DialogWindow { DataContext = dataContext, Title = $"Dialog {_open.Count + 1}" };
        dialog.Closed += (_, _) => Remove(dialog);
        _open.Push(dialog);
        _ = dialog.ShowDialog(over);
    }

    public void CloseTopDialog()
    {
        if (_open.Count > 0)
            _open.Peek().Close();
    }

    public void PushModal(NavigationPage host, object? dataContext) =>
        _ = host.PushModalAsync(new WindowPageModal { DataContext = dataContext });

    public void PopModal(NavigationPage host) => _ = host.PopModalAsync();

    private void Remove(DialogWindow dialog)
    {
        // A dialog closed out of order (by the OS) leaves the others stacked as they were
        var keep = new Stack<DialogWindow>();
        while (_open.Count > 0)
        {
            var top = _open.Pop();
            if (!ReferenceEquals(top, dialog))
                keep.Push(top);
        }
        while (keep.Count > 0)
            _open.Push(keep.Pop());
    }
}
