using Avalonia;
using Avalonia.Media;
using Ivi.Visa;
using NationalInstruments.Visa;
using System;
using System.Text;
using System.Threading.Tasks;

namespace StreamElectronics.Multimeter;

public class RigolMultimeterService : IMultimeterService
{
    #region Private Members

    private ResourceManager? _resourceManager;
    private MessageBasedSession? _session;

    private byte[] _readBuffer = new byte[1024];
    
    private bool _errorConnecting = false;
    
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
    public event Action ModeChanged = () => { };

    #endregion
    
    #region Constructor
    
    public RigolMultimeterService()
    {
        Task.Run(MainLoop);
    }

    #endregion
    
    #region Methods
    
    private async Task? MainLoop()
    {
        while (true)
        {
            // Classic infinite loop

            // If we have error connecting...
            if (_errorConnecting)
            {
                // Wait 2 seconds
                await Task.Delay(2000);

                // Clear error flag
                _errorConnecting = false;
            }

            // Attempt to reconnect
            if (IsEnabled && !IsConnected) Connect();

            // Disconnect if disabled
            if (!IsEnabled && IsConnected) Disconnect();

            // If connected, get live value
            if (IsConnected && IsEnabled) FetchLiveValue();

            // Run 5 times a second
            await Task.Delay(200);
        }
    }
    
    // Helper method to write a string as a byte array to the raw IO interface
    private void WriteStringRaw(string command) => _session?.RawIO.Write(Encoding.ASCII.GetBytes(command));

    // Helper method to read a string from the raw IO interface
    private string ReadStringRaw()
    {
        // Read all available data into the buffer.
        var result = _session?.RawIO.Read(_readBuffer);

        // Convert the relevant portion of the buffer to a string and trim whitespace
        return Encoding.ASCII.GetString(_readBuffer, 0, (int)(result?.ActualCount ?? 0)).Trim();
    }

    public void FetchLiveValue()
    {
        var measureCommand = Mode switch
        {
            MultimeterMode.VoltageDC    => "MEAS:VOLT:DC?\n",
            MultimeterMode.VoltageAC    => "MEAS:VOLT:AC?\n",
            MultimeterMode.CurrentDC    => "MEAS:CURR:DC?\n",
            MultimeterMode.CurrentAC    => "MEAS:CURR:AC?\n",
            MultimeterMode.Resistance   => "MEAS:RES? AUTO,MAX\n",
            MultimeterMode.Capacitance  => "MEAS:CAP?\n",
            MultimeterMode.Frequency    => "MEAS:FREQ?\n",
            MultimeterMode.Diode        => "MEAS:DIODE?\n",
            MultimeterMode.Continuity   => "MEAS:CONT?\n",
            MultimeterMode.Temperature  => "MEAS:TEMP? RTD,100,OHM\n",
            // Volt DC if unknown
            MultimeterMode.Unknown => "MEAS:VOLT:DC?\n",
            _ => "MEAS:VOLT:DC?\n"
        };
        
        
        // Send the measure command
        WriteStringRaw(measureCommand);

        // Read the result
        var result = ReadStringRaw();

        if (!IsEnabled)
            return;
        
        // Try to convert to number
        if (double.TryParse(result, out var value))
        {
            LiveValue = $"{value:0.000}";

            // Do other things to react to the real value
            ProcessLiveValue(value);
        }
        // If it fails, just display the value
        else
            LiveValue =  $"{result}";

        DataChanged();
    }

    private void ProcessLiveValue(double value)
    {
        // Voltage DC color gradients
        if (Mode == MultimeterMode.VoltageDC)
        {
            if (value < 1)
                Color = _colorUnknown;
            else if (value < 3.5)
                Color = _colorVoltage;
            else if (value < 6)
                Color = _colorDiode;
            else
                Color = _colorTemperature;
        }
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

        ModeChanged();
        DataChanged();
    }
    
    public void Connect()
    {
        if (IsConnecting)
            return;
        
        IsConnecting =  true;
        _errorConnecting = false;

        Task.Run(() =>
        {
            try
            {
                _resourceManager = new ResourceManager();
                _session = (MessageBasedSession)_resourceManager.Open(VisaId);

                // Set a suitable timeout (e.g., 20 seconds)
                _session.TimeoutMilliseconds = 20000;

                // FIX FOR TIMEOUT: Explicitly enable and set the Termination Character (0x0A is ASCII for Line Feed \n)
                _session.TerminationCharacterEnabled = true;
                _session.TerminationCharacter = 0x0A; // Line Feed (\n)
                
                OnConnected();
            }
            catch (Exception e)
            {
                _resourceManager?.Dispose();
                _resourceManager = null;
                _session?.Dispose();
                _session = null;

                _errorConnecting = true;
                
                // Report problem
                DeviceName = $"Error {DateTime.Now:HH:mm:ss}";
                Status = "Error: " + e.Message;

                DataChanged();
            }
            finally
            {
                IsConnecting = false;
            }
        });
    }
        
    public void Disconnect()
    {
        _resourceManager?.Dispose();
        _resourceManager = null;
        _session?.Dispose();
        _session = null;
        
        OnDisconnected();
    }

    public virtual void OnConnected()
    {
        IsConnected = true;
        Connected();

        // Fetch device name
        FetchDeviceName();
        
        // Set default mode
        ChangeMode(MultimeterMode.VoltageDC);
    }

    public void FetchDeviceName()
    {
        try
        {
            // Use the RawIO methods explicitly to bypass missing high-level string methods
            WriteStringRaw("*IDN?\n");
            var idn = ReadStringRaw();
            
            var parts = idn.Split(',');
            DeviceName = parts.Length > 1 ? parts[1] : idn;
            Status = idn;
            
            // Write byte array for reset command
            WriteStringRaw("*RST\n");
            
            DataChanged();
        }
        catch (Exception e)
        {
            _errorConnecting = true;
            Status = "Error: " + e.StackTrace;
            DataChanged();
        }
    }

    public virtual void OnDisconnected()
    {
        IsConnected = false;
        Disconnected();
    }

    #endregion
}