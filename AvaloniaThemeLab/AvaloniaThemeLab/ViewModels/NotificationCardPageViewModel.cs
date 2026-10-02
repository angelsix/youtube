using System;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using AvaloniaThemeLab.Pages;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>The NotificationCard page. The manager is what places cards over the window, so it is shown working, not just its cards.</summary>
public sealed partial class NotificationCardPageViewModel(Func<TopLevel?> topLevel)
    : PageEntryViewModel(typeof(NotificationCard), typeof(NotificationCardPage))
{
    private WindowNotificationManager? _manager;

    [RelayCommand]
    private void ShowNotification()
    {
        if (topLevel() is not { } host)
            return;

        _manager ??= new WindowNotificationManager(host);
        _manager.Show(new Notification("Notification", "Shown by WindowNotificationManager"));
    }
}
