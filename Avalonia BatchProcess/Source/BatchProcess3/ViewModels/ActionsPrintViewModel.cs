using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels;

public partial class ActionsPrintViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private string _jobName;
    
    [ObservableProperty]
    private bool _isSelected;
    
    [ObservableProperty]
    private string _description;
    
    [ObservableProperty]
    private string _printDrawingRange;
    
    [ObservableProperty]
    private string _drawingExclusionList;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DrawingExclusionListTitle))]
    private bool _drawingExlusionIsWhiteList;
    
    public string DrawingExclusionListTitle => DrawingExlusionIsWhiteList ? "White List" : "Black List";
    
    [ObservableProperty]
    private bool _printModels;
    
    [ObservableProperty]
    private bool _printDrawings;

    [ObservableProperty]
    private bool _isNewItem;
}