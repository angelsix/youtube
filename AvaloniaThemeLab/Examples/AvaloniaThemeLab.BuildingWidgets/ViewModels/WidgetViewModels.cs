using System;
using System.Collections.Generic;
using AngelSix.Mvvm;
using AvaloniaThemeLab.BuildingWidgets.Theme;

namespace AvaloniaThemeLab.BuildingWidgets.ViewModels;

/// <summary>One navigation row bound into a ListBox (optional icon name resolved via <c>Icon</c>).</summary>
public sealed record NavItem(string Label, string Caption, string? Icon = null);

/// <summary>One KPI metric card bound into the AppShell metrics ItemsControl.</summary>
public sealed record MetricCardItem(string Label, string Value);

public sealed partial class ExampleContentViewModel([Property] PrototypeTheme theme) : ViewModelBase
{
    #region Commands

    public IRelayCommand ToggleModeCommand => Command(() => Theme.IsDark = !Theme.IsDark);

    public IRelayCommand BiggerCommand => Command(() => Theme.BaseSize = Math.Min(1.5, Math.Round(Theme.BaseSize + 0.1, 2)));

    public IRelayCommand SmallerCommand => Command(() => Theme.BaseSize = Math.Max(0.75, Math.Round(Theme.BaseSize - 0.1, 2)));

    #endregion Commands
}

public sealed partial class MoonWidgetViewModel([Property] PrototypeTheme theme) : ViewModelBase;

public sealed partial class ModeToggleViewModel([Property] PrototypeTheme theme) : ViewModelBase;

public sealed partial class TopMenuViewModel([Property] PrototypeTheme theme) : ViewModelBase
{
    #region Observable Properties

    [Observable] public partial string Caption { get; set; } = "Pick something from the menu.";

    #endregion Observable Properties

    #region Commands

    public IRelayCommand<string?> ChooseMenuCommand => Command<string?>(choice =>
    {
        if (!string.IsNullOrWhiteSpace(choice))
            Caption = $"You chose {choice}.";
    });

    public IRelayCommand ToggleModeCommand => Command(() => Theme.IsDark = !Theme.IsDark);

    #endregion Commands
}

public sealed partial class SideNavViewModel : ViewModelBase
{
    #region Properties

    public IReadOnlyList<NavItem> TopItems { get; }

    public IReadOnlyList<NavItem> ShipmentItems { get; }

    public IReadOnlyList<NavItem> FooterItems { get; }

    public string PageTitle => ActiveItem.Label;

    public string PageCaption => ActiveItem.Caption;

    #endregion Properties

    #region Observable Properties

    [Observable(IgnoreNull = true)]
    [NotifyPropertyChangedFor(nameof(PageTitle))]
    [NotifyPropertyChangedFor(nameof(PageCaption))]
    public partial NavItem ActiveItem { get; set; }

    #endregion Observable Properties

    #region Constructors

    public SideNavViewModel()
    {
        TopItems =
        [
            new("Home", "Good evening. Here is today at a glance.", nameof(PrototypeIconGallery.Home)),
            new("Dashboard", "Sales and stock for the last 30 days.", nameof(PrototypeIconGallery.Dashboard)),
            new("Orders", "Every order, newest first.", nameof(PrototypeIconGallery.Orders)),
        ];
        ShipmentItems =
        [
            new("To pack", "12 shipments are waiting to be packed."),
            new("In transit", "7 shipments are with the courier."),
            new("Delivered", "41 shipments arrived this week."),
        ];
        FooterItems =
        [
            new("Settings", "Preferences for this app.", nameof(PrototypeIconGallery.Settings)),
        ];

        ActiveItem = ShipmentItems[0];
    }

    #endregion Constructors
}

public sealed partial class AppShellViewModel : ViewModelBase
{
    #region Properties

    public IReadOnlyList<NavItem> PrimaryItems { get; }

    public IReadOnlyList<NavItem> FooterItems { get; }

    public IReadOnlyList<MetricCardItem> Metrics { get; }

    public string Title => ActiveItem.Label;

    public string Caption => ActiveItem.Caption;

    public string Status => $"Showing {ActiveItem.Label}";

    #endregion Properties

    #region Observable Properties

    [Observable(IgnoreNull = true)]
    [NotifyPropertyChangedFor(nameof(Title))]
    [NotifyPropertyChangedFor(nameof(Caption))]
    [NotifyPropertyChangedFor(nameof(Status))]
    public partial NavItem ActiveItem { get; set; }

    [Observable] public partial bool IsNavVisible { get; set; } = true;

    #endregion Observable Properties

    #region Commands

    public IRelayCommand ToggleNavCommand => Command(() => IsNavVisible = !IsNavVisible);

    #endregion Commands

    #region Constructors

    public AppShellViewModel()
    {
        PrimaryItems =
        [
            new("Home", "Good evening. Here is today at a glance.", nameof(PrototypeIconGallery.Home)),
            new("Dashboard", "Sales and stock for the last 30 days.", nameof(PrototypeIconGallery.Dashboard)),
            new("Orders", "Every order, newest first.", nameof(PrototypeIconGallery.Orders)),
            new("Shipments", "Everything that is packed or on its way.", nameof(PrototypeIconGallery.Shipments)),
        ];
        FooterItems =
        [
            new("Settings", "Preferences for this app.", nameof(PrototypeIconGallery.Settings)),
        ];
        Metrics =
        [
            new("OPEN ORDERS", "24"),
            new("TO PACK", "12"),
            new("SHIPPED TODAY", "9"),
        ];

        ActiveItem = PrimaryItems[0];
    }

    #endregion Constructors
}
