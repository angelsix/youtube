using Avalonia;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels;

public partial class AvailableActionItemViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelectable))]
    [NotifyPropertyChangedFor(nameof(Padding))]
    private ProcessActionViewModel? _actionViewModel;
    
    [ObservableProperty]
    private string? _category;

    [ObservableProperty]
    private string? _iconPath;

    public bool IsSelectable => ActionViewModel != null;
    
    public Thickness Padding => IsSelectable ? new Thickness(5) : new Thickness(5, 5, 5, 2);
}