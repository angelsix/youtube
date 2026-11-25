using System;
using Avalonia;
using Avalonia.Controls;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;

namespace AvaloniaThemeLab.ThemePlayground.Pages;

public partial class WindowPage : UserControl
{
    private ModalNavigationService? _navigation;

    public WindowPage() => InitializeComponent();

    protected override void OnDataContextChanged(EventArgs e)
    {
        _navigation?.Detach(ModalHost);
        base.OnDataContextChanged(e);
        _navigation = (DataContext as WindowPageViewModel)?.ModalNavigation as ModalNavigationService;
        _navigation?.Attach(ModalHost);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _navigation?.Attach(ModalHost);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _navigation?.Detach(ModalHost);
        base.OnDetachedFromVisualTree(e);
    }
}
