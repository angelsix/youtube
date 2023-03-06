using ManagedBass;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AvaloniaLoudnessMeter.Services;

public delegate void DataAvailableHandler(byte[] buffer, int length);

public class AudioCaptureService : IDisposable
{
    #region Private Members
    
    private byte[] mBuffer;

    private int mDevice, mHandle;
    
    #endregion

    #region Public Events
    
    public event DataAvailableHandler DataAvailable;
    
    #endregion

    #region Default Constructor
    
    public AudioCaptureService(int deviceId)
    {
        mDevice = deviceId;

        Bass.Init();
        Bass.RecordInit(mDevice);

        mHandle = Bass.RecordStart(44100, 2, BassFlags.RecordPause, Procedure);
    } 
    
    #endregion
    
    #region Capture Audio Methods
    
    private bool Procedure(int handle, IntPtr buffer, int length, IntPtr user)
    {
        if (mBuffer == null || mBuffer.Length < length)
            mBuffer = new byte[length];

        Marshal.Copy(buffer, mBuffer, 0, length);

        DataAvailable?.Invoke(mBuffer, length);

        return true;
    }
    
    #endregion
    
    #region Public Control Methods
    
    public void Start() => Bass.ChannelPlay(mHandle);

    public void Stop() => Bass.ChannelStop(mHandle);

    #endregion

    #region Dispose
    
    public void Dispose()
    {
        Bass.CurrentRecordingDevice = mDevice;

        Bass.RecordFree();
    }
    
    #endregion
}