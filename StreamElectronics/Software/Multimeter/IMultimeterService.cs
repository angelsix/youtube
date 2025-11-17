using Avalonia.Media;
using System;

namespace StreamElectronics.Multimeter;

public interface IMultimeterService
{
    bool IsConnected { get; set; }
    bool IsEnabled { get; set; }
    bool IsConnecting { get; set; }
    string VisaId { get; set; }
    string DeviceName { get; set; }
    string Status { get; set; }
    string LiveValue { get; set; }
    string ModeTitle { get; set; }
    string SymbolTop { get; set; }
    string SymbolBottom { get; set; }
    MultimeterMode Mode { get; set; }
    IBrush Color { get; set; }
    event Action Connected;
    event Action Disconnected;
    event Action DataChanged;
    event Action ModeChanged;
    void FetchLiveValue();
    void IncrementMode();
    void ChangeMode(MultimeterMode mode);
    void Connect();
    void Disconnect();
    void OnConnected();
    void FetchDeviceName();
    void OnDisconnected();
}