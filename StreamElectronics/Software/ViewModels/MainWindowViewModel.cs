using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StreamElectronics.Multimeter;
using System;

namespace StreamElectronics.ViewModels;

public partial class MainWindowViewModel(DummyMultimeterService multimeterService) : ViewModelBase
{
    private IBrush _unknownModeBrush = new SolidColorBrush(Colors.White);
    private IBrush _connectedStatusBrush = new SolidColorBrush(Colors.Green);
    private IBrush _disconnectedStatusBrush = new SolidColorBrush(Colors.Brown);
    private IBrush _uninitializedStatusBrush = new SolidColorBrush(Colors.SlateGray);
    
    public string VisaId { get; } = "";
    
    [ObservableProperty]
    private IBrush _statusBrush = new SolidColorBrush(Colors.SlateGray);

    [ObservableProperty]
    private string _connectedStatus = "---";
    
    [ObservableProperty]
    private string _ModeTitle = "NOT CONNECTED";
    
    [ObservableProperty]
    private string _liveValue = "-.---";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SymbolsVisible))]
    private string _symbolTop = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SymbolsVisible))]
    private string _symbolBottom = "";

    public bool SymbolsVisible => !(string.IsNullOrEmpty(SymbolTop) && string.IsNullOrEmpty(SymbolBottom));

    [ObservableProperty]
    private IBrush _multimeterColor = new SolidColorBrush(Colors.White);

    [ObservableProperty]
    private string _deviceName = "[No Device]";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DeviceStatusShort))]
    private string _deviceStatus = "";

    public string DeviceStatusShort => DeviceStatus[..Math.Min(40, DeviceStatus.Length)];

    [ObservableProperty]
    private bool _isEnabled = false;

    [RelayCommand]
    private void EnabledChanged()
    {
        // Update multimeter status
        multimeterService.IsEnabled = IsEnabled;
        
        DeviceName = "[No Device]";
        DeviceStatus = "";
        LiveValue = "-.---";
        SymbolTop = SymbolBottom = DeviceStatus = "";
        ModeTitle = "NOT CONNECTED";
        MultimeterColor = _unknownModeBrush;
        
        // If we just enabled, presume service is attempting a connection
        if (IsEnabled)
            ConnectedStatus = "Connecting...";
    }
    
    [RelayCommand]
    private void Initalize()
    {
        StatusBrush = _uninitializedStatusBrush;
        
        multimeterService.Connected += () =>
        {
            ConnectedStatus = "Connected";
            StatusBrush = _connectedStatusBrush;
        };
        multimeterService.Disconnected += () =>
        {
            ConnectedStatus = IsEnabled ? "Disconnected" : "---";
            StatusBrush = IsEnabled ? _disconnectedStatusBrush : _uninitializedStatusBrush;
        };
        multimeterService.DataChanged += () =>
        {
            DeviceName = multimeterService.DeviceName.ToUpper();
            DeviceStatus = multimeterService.Status;
            SymbolTop = multimeterService.SymbolTop.ToUpper();
            SymbolBottom = multimeterService.SymbolBottom.ToUpper();
            ModeTitle = multimeterService.ModeTitle.ToUpper();
            LiveValue =  multimeterService.LiveValue;
            MultimeterColor = multimeterService.Color;
        };
    }

    public MainWindowViewModel() : this(new DummyMultimeterService())
    {
        if (!Avalonia.Controls.Design.IsDesignMode) 
            throw new InvalidOperationException("Parameterless constructor is only for design time use");

    }
}
