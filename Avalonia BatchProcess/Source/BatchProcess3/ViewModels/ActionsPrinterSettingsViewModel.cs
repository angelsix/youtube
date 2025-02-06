using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels;

public partial class ActionsPrinterSettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _id;
    
    [ObservableProperty]
    private string _type;
    
    [ObservableProperty]
    private string _printerName;
    
    [ObservableProperty]
    private string _printerSize;
    
    [ObservableProperty]
    private double _width;
    
    [ObservableProperty]
    private double _height;
    
    [ObservableProperty]
    private bool? _portrait;
    
    [ObservableProperty]
    private string _sourceTray;
    
    [ObservableProperty]
    private string _drawingColor;
    
    [ObservableProperty]
    private bool _scaleToFit;
}