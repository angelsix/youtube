using Avalonia;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace StreamElectronics.Multimeter;

public enum MultimeterMode
{
    Unknown = 0,
    VoltageDC = 1,
    VoltageAC = 2,
    CurrentDC = 3,
    CurrentAC = 4,
    Resistance = 5,
    Capacitance = 6,
    Diode = 7,
    Continuity = 8,
    Frequency = 9,
    Temperature = 10
}

public class DummyMultimeterService
{
    #region Private Members
    
    private readonly IBrush _colorVoltage = new SolidColorBrush(Colors.Yellow);
    private readonly IBrush _colorCurrent = new SolidColorBrush(Colors.MediumPurple);
    private readonly IBrush _colorResistance = new SolidColorBrush(Colors.CornflowerBlue);
    private readonly IBrush _colorCapacitance = new SolidColorBrush(Colors.White);
    private readonly IBrush _colorDiode = new SolidColorBrush(Colors.Orange);
    private readonly IBrush _colorContinuity = new SolidColorBrush(Colors.Red);
    private readonly IBrush _colorFrequency = new LinearGradientBrush 
    { 
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative), 
        GradientStops = new GradientStops
        {
            new GradientStop(Colors.Cyan, 0.0), 
            new GradientStop(Colors.Magenta, 1.0) 
        }
    };
    private readonly IBrush _colorTemperature = new LinearGradientBrush 
    { 
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative), 
        GradientStops = new GradientStops
        {
            new GradientStop(Colors.Red, 0.0), 
            new GradientStop(Colors.Orange, 1.0) 
        }
    };
    private readonly IBrush _colorUnknown = new SolidColorBrush(Colors.White);
    
    private float _liveValueTest = 0;
    
    private string _visaId = "";
    
    #endregion

    #region Public Members
    
    public bool IsConnected { get; set; } = false;
    
    public bool IsEnabled { get; set; } = false;
    
    public bool IsConnecting  { get; set; } = false;
    
    public string VisaId { get; set; } = "";

    public string DeviceName { get; set; } = "";
    
    public string Status { get; set; } = "";

    public string LiveValue { get; set; } = "";

    public string ModeTitle { get; set; } = "";

    public string SymbolTop { get; set; } = "";

    public string SymbolBottom { get; set; } = "";
    
    public MultimeterMode Mode { get; set; } = MultimeterMode.Unknown;
    
    public IBrush Color { get; set; } = new SolidColorBrush(Colors.White);
    
    #endregion
    
    #region Public Events
    
    public event Action Connected = () => { };
    public event Action Disconnected = () => { };
    public event Action DataChanged = () => { };

    #endregion
    
    #region Constructor
    
    public DummyMultimeterService()
    {
        Task.Run(async () =>
        {
            var i = 0;
            while (true)
            {
                // Classic infinite loop
                // TODO: Connect to service if enabled and not connected
                if (IsEnabled && !IsConnected)
                    Connect();
                
                // Disconnect if disabled
                if (!IsEnabled && IsConnected)
                    Disconnect();
                
                // If connected, get live value
                if (IsConnected && IsEnabled)
                {
                    FetchLiveValue();

                    i++;
                    if (i > 20)
                    {
                        //IncrementMode();
                        i = 0;
                    }
                }

                // Run 5 times a second
                await Task.Delay(50);
            }
        });
    }

    #endregion
    
    #region Methods
    
    public void FetchLiveValue()
    {
        _liveValueTest += 0.05f;

        LiveValue =  $"{_liveValueTest:0.000}";
        
        // Voltage DC color gradients
        if (_liveValueTest < 1)
            Color = _colorUnknown;
        else if (_liveValueTest < 3.5)
            Color = _colorVoltage;
        else if (_liveValueTest < 6)
            Color = _colorDiode;
        else 
            Color = _colorTemperature;
            
        DataChanged();
    }

    public void IncrementMode() => ChangeMode(Mode+1);

    public void ChangeMode(MultimeterMode mode)
    {
        Mode = mode;
        
        ModeTitle = mode switch
        {
            MultimeterMode.VoltageDC or MultimeterMode.VoltageAC => "Voltage",
            MultimeterMode.CurrentDC or MultimeterMode.CurrentAC => "Current",
            MultimeterMode.Resistance => "Resistance",
            MultimeterMode.Capacitance => "Capacitance",
            MultimeterMode.Diode => "Diode",
            MultimeterMode.Continuity => "Continuity",
            MultimeterMode.Frequency => "Frequency",
            MultimeterMode.Temperature => "Temperature",
            MultimeterMode.Unknown => "Unknown",
            _ => "Unknown"
        };
        
        SymbolTop = mode switch
        {
            MultimeterMode.VoltageDC or MultimeterMode.VoltageAC => "V",
            MultimeterMode.CurrentDC or MultimeterMode.CurrentAC => "A",
            MultimeterMode.Resistance => "",
            MultimeterMode.Capacitance => "",
            MultimeterMode.Diode => "",
            MultimeterMode.Continuity => "",
            MultimeterMode.Frequency => "",
            MultimeterMode.Temperature => "",
            MultimeterMode.Unknown => "",
            _ => ""
        };
        
        SymbolBottom = mode switch
        {
            MultimeterMode.VoltageDC or MultimeterMode.CurrentDC => "DC",
            MultimeterMode.VoltageAC or MultimeterMode.CurrentAC => "AC",
            MultimeterMode.Resistance => "",
            MultimeterMode.Capacitance => "",
            MultimeterMode.Diode => "",
            MultimeterMode.Continuity => "",
            MultimeterMode.Frequency => "",
            MultimeterMode.Temperature => "",
            MultimeterMode.Unknown => "",
            _ => ""
        };
        
        Color = mode switch
        {
            MultimeterMode.VoltageDC or MultimeterMode.VoltageAC => _colorVoltage,
            MultimeterMode.CurrentDC or MultimeterMode.CurrentAC => _colorCurrent,
            MultimeterMode.Resistance =>  _colorResistance,
            MultimeterMode.Capacitance =>  _colorCapacitance,
            MultimeterMode.Diode =>  _colorDiode,
            MultimeterMode.Continuity => _colorContinuity,
            MultimeterMode.Frequency => _colorFrequency,
            MultimeterMode.Temperature => _colorTemperature,
            MultimeterMode.Unknown => _colorUnknown,
            _ =>  _colorUnknown
        };

        DataChanged();
    }
    
    public void Connect()
    {
        if (IsConnecting)
            return;
        
        IsConnecting =  true;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(2000);
                OnConnected();
            }
            finally
            {
                IsConnecting = false;
            }
        });
    }
        
    public void Disconnect()
    {
        OnDisconnected();
    }

    protected virtual void OnConnected()
    {
        IsConnected = true;
        Connected();

        // Fetch device name
        FetchDeviceName();
        
        // Set default mode
        ChangeMode(MultimeterMode.VoltageDC);
    }

    private void FetchDeviceName()
    {
        try
        {
            DeviceName = "Rigol DM3068";
            Status = "Device alive and well.";
            DataChanged();
        }
        catch (Exception e)
        {
            Status = "Error: " + e.StackTrace;
            DataChanged();
        }
    }

    protected virtual void OnDisconnected()
    {
        IsConnected = false;
        Disconnected();

        _liveValueTest = 0;
    }

    #endregion
}