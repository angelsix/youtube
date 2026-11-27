using System;
using System.Collections.Generic;
using AngelSix.Mvvm;
using AvaloniaThemeLab.BuildingWidgets.Theme;

namespace AvaloniaThemeLab.BuildingWidgets.ViewModels;

/// <summary>One navigation row bound into a ListBox (optional icon name resolved via <c>Icon</c>).</summary>
public sealed record NavItem(string Label, string Caption, string? Icon = null);

/// <summary>One KPI metric card bound into the AppShell metrics ItemsControl.</summary>
public sealed record MetricCardItem(string Label, string Value);

public sealed partial class ExampleContentViewModel(PrototypeTheme theme) : ViewModelBase
{
    #region Properties

    public PrototypeTheme Theme { get; } = theme;

    #endregion Properties

    #region Commands

    public IRelayCommand ToggleModeCommand => Command(() => Theme.IsDark = !Theme.IsDark);

    public IRelayCommand BiggerCommand => Command(() => Theme.BaseSize = Math.Min(1.5, Math.Round(Theme.BaseSize + 0.1, 2)));

    public IRelayCommand SmallerCommand => Command(() => Theme.BaseSize = Math.Max(0.75, Math.Round(Theme.BaseSize - 0.1, 2)));

    #endregion Commands
}

public sealed class MoonWidgetViewModel(PrototypeTheme theme) : ViewModelBase
{
    #region Properties

    public PrototypeTheme Theme { get; } = theme;

    #endregion Properties
}

public sealed class ModeToggleViewModel(PrototypeTheme theme) : ViewModelBase
{
    #region Properties

    public PrototypeTheme Theme { get; } = theme;

    #endregion Properties
}

public sealed partial class TopMenuViewModel(PrototypeTheme theme) : ViewModelBase
{
    #region Properties

    public PrototypeTheme Theme { get; } = theme;

    #endregion Properties

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

public sealed class SideNavViewModel : ViewModelBase
{
    #region Fields

    private NavItem _activeItem;

    #endregion Fields

    #region Properties

    public IReadOnlyList<NavItem> TopItems { get; }

    public IReadOnlyList<NavItem> ShipmentItems { get; }

    public IReadOnlyList<NavItem> FooterItems { get; }

    public NavItem ActiveItem
    {
        get => _activeItem;
        set
        {
            if (value is null || EqualityComparer<NavItem>.Default.Equals(_activeItem, value))
                return;

            _activeItem = value;
            OnPropertyChanged(nameof(ActiveItem));
            OnPropertyChanged(nameof(PageTitle));
            OnPropertyChanged(nameof(PageCaption));
        }
    }

    public string PageTitle => ActiveItem.Label;

    public string PageCaption => ActiveItem.Caption;

    #endregion Properties

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

        _activeItem = ShipmentItems[0];
    }

    #endregion Constructors
}

public sealed partial class AppShellViewModel : ViewModelBase
{
    #region Fields

    private NavItem _activeItem;

    #endregion Fields

    #region Properties

    public IReadOnlyList<NavItem> PrimaryItems { get; }

    public IReadOnlyList<NavItem> FooterItems { get; }

    public IReadOnlyList<MetricCardItem> Metrics { get; }

    public NavItem ActiveItem
    {
        get => _activeItem;
        set
        {
            if (value is null || EqualityComparer<NavItem>.Default.Equals(_activeItem, value))
                return;

            _activeItem = value;
            OnPropertyChanged(nameof(ActiveItem));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Caption));
            OnPropertyChanged(nameof(Status));
        }
    }

    public string Title => ActiveItem.Label;

    public string Caption => ActiveItem.Caption;

    public string Status => $"Showing {ActiveItem.Label}";

    #endregion Properties

    #region Observable Properties

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

        _activeItem = PrimaryItems[0];
    }

    #endregion Constructors
}
