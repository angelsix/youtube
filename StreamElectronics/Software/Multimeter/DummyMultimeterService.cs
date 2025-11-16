using System;
using System.Threading.Tasks;

namespace StreamElectronics.Multimeter;

public class DummyMultimeterService
{
    private string _visaId = "";

    public bool IsConnected { get; set; } = false;
    public bool IsEnabled { get; set; } = false;
    
    public bool IsConnecting  { get; set; } = false;

    public string VisaId { get; set; } = "";
    
    public event Action Connected = () => { };
    public event Action Disconnected = () => { };

    public DummyMultimeterService()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                // Classic infinite loop
                // TODO: Connect to service if enabled and not connected
                if (IsEnabled && !IsConnected)
                    Connect();
                
                // Disconnect if disabled
                if (!IsEnabled && IsConnected)
                    Disconnect();
                
                // Run 5 times a second
                await Task.Delay(200);
            }
        });
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
    }

    protected virtual void OnDisconnected()
    {
        IsConnected = false;
        Disconnected();
    }
}