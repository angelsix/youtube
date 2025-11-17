using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StreamElectronics.Multimeter;
using System;
using System.IO;

namespace StreamElectronics.ViewModels;

public partial class MainWindowViewModel(IMultimeterService multimeterService) : ViewModelBase
{
    private IBrush _unknownModeBrush = new SolidColorBrush(Colors.White);
    private IBrush _connectedStatusBrush = new SolidColorBrush(Colors.Green);
    private IBrush _disconnectedStatusBrush = new SolidColorBrush(Colors.Brown);
    private IBrush _uninitializedStatusBrush = new SolidColorBrush(Colors.SlateGray);
    
    // IMPORTANT: Installer files in AngelSix\Software Programs\Rigol UltraSigma
    // Install ni-visa_25.8_online.exe otherwise using var resourceManager = new ResourceManager();
    // will throw a dll not found exception
    //
    // UltraSigma(PC)Installer isn't needed except to get the actual instrument address
    // Connect instrument to network, copy its address for example TCPIP::192.168.1.118::INSTR
    //
    [ObservableProperty]
    private string _visaId = "";
    
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
    private bool _buttonVDCActive = false;
    
    [ObservableProperty]
    private bool _buttonVACActive = false;
    
    [ObservableProperty]
    private bool _buttonIDCActive = false;
    
    [ObservableProperty]
    private bool _buttonIACActive = false;
    
    [ObservableProperty]
    private bool _buttonCapacitanceActive = false;
    
    [ObservableProperty]
    private bool _buttonResistanceActive = false;
    
    [ObservableProperty]
    private bool _buttonDiodeActive = false;
    
    [ObservableProperty]
    private bool _buttonContinuityActive = false;
    
    [ObservableProperty]
    private bool _buttonTemperatureActive = false;
    
    [ObservableProperty]
    private bool _buttonFrequencyActive = false;
    
    [ObservableProperty]
    private bool _isEnabled = false;
    
    [ObservableProperty]
    private MultimeterMode _multimeterMode = MultimeterMode.Unknown;

    [RelayCommand]
    private void ChangeMode(MultimeterMode mode) => multimeterService.ChangeMode(mode);

    [RelayCommand]
    private void EnabledChanged()
    {
        // Write VISA value to file
        if (IsEnabled)
        {
            try
            {
                File.WriteAllText("visaid.txt", VisaId);
            }
            catch (Exception _)
            {
                // ignored
            }
        }
        
        DeviceName = "[No Device]";
        DeviceStatus = "";
        LiveValue = "-.---";
        SymbolTop = SymbolBottom = DeviceStatus = "";
        ModeTitle = "NOT CONNECTED";
        MultimeterColor = _unknownModeBrush;
        
        // If we just enabled, presume service is attempting a connection
        if (IsEnabled)
        {
            ConnectedStatus = "Connecting...";

            // Update multimeter status
            multimeterService.VisaId = VisaId;
        }

        // Update multimeter service
        multimeterService.IsEnabled = IsEnabled;
    }
    
    [RelayCommand]
    private void Initalize()
    {
        // Read VISA value from file
        try
        {
            if (File.Exists("visaid.txt"))
                VisaId = File.ReadAllText("visaid.txt");
        }
        catch (Exception _)
        {
            // ignored
        }
        
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
        multimeterService.ModeChanged += () =>
        {
            // Set all active classes
            ButtonVDCActive = multimeterService.Mode == MultimeterMode.VoltageDC;
            ButtonVACActive =  multimeterService.Mode == MultimeterMode.VoltageAC;
            ButtonIDCActive =  multimeterService.Mode == MultimeterMode.CurrentDC;
            ButtonIACActive =  multimeterService.Mode == MultimeterMode.CurrentAC;
            ButtonCapacitanceActive =  multimeterService.Mode == MultimeterMode.Capacitance;
            ButtonContinuityActive =  multimeterService.Mode == MultimeterMode.Continuity;
            ButtonResistanceActive =  multimeterService.Mode == MultimeterMode.Resistance;
            ButtonDiodeActive =  multimeterService.Mode == MultimeterMode.Diode;
            ButtonTemperatureActive =  multimeterService.Mode == MultimeterMode.Temperature;
            ButtonFrequencyActive =  multimeterService.Mode == MultimeterMode.Frequency; 
        };
    }

    public MainWindowViewModel() : this(new DummyMultimeterService())
    {
        if (!Avalonia.Controls.Design.IsDesignMode) 
            throw new InvalidOperationException("Parameterless constructor is only for design time use");

    }
}
