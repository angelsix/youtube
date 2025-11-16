using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StreamElectronics.Multimeter;
using System;

namespace StreamElectronics.ViewModels;

public partial class MainWindowViewModel(DummyMultimeterService multimeterService) : ViewModelBase
{
    private IBrush _connectedStatusBrush = new SolidColorBrush(Colors.Green);
    private IBrush _disconnectedStatusBrush = new SolidColorBrush(Colors.Brown);
    private IBrush _uninitializedStatusBrush = new SolidColorBrush(Colors.SlateGray);
    
    public string VisaId { get; } = "";
    
    [ObservableProperty]
    private IBrush _statusBrush = new SolidColorBrush(Colors.SlateGray);

    [ObservableProperty]
    private string _connectedStatus = "---";

    [ObservableProperty]
    private bool _isEnabled = false;

    [RelayCommand]
    private void EnabledChanged()
    {
        // Update multimeter status
        multimeterService.IsEnabled = IsEnabled;
        
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
    }

    public MainWindowViewModel() : this(new DummyMultimeterService())
    {
        if (!Avalonia.Controls.Design.IsDesignMode) 
            throw new InvalidOperationException("Parameterless constructor is only for design time use");

    }
}
