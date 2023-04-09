using Avalonia.Threading;
using AvaloniaLoudnessMeter.DataModels;
using ManagedBass;
using NWaves.Signals;
using NWaves.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AvaloniaLoudnessMeter.Services;

public class BassAudioCaptureService : IDisposable, IAudioCaptureService
{
    #region Private Members
    
    /// <summary>
    /// The buffer for a short capture of microphone audio
    /// </summary>
    private byte[] mBuffer;

    /// <summary>
    /// The device ID we want to capture
    /// </summary>
    private int mDevice;
    
    /// <summary>
    /// The handle to the device we want to capture
    /// </summary>
    private int mHandle;
    
    /// <summary>
    /// The last few sets of captured audio bytes, converted to LUFS
    /// </summary>
    private Queue<double> mLufs = new Queue<double>();

    /// <summary>
    /// The frequency to capture at
    /// </summary>
    private int mCaptureFrequency = 44100;

    #endregion

    #region Public Events

    /// <inheritdoc />
    public event Action<AudioChunkData> AudioChunkAvailable;

    #endregion

    #region Default Constructor
    
    /// <summary>
    /// Initializes the audio capture service, and starts capturing the specified device ID
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="frequency"></param>
    public BassAudioCaptureService(int deviceId = 1, int frequency = 44100)
    {
        // Store device ID
        mDevice = deviceId;

        // Initialise and start 
        Bass.Init();
        Bass.RecordInit(mDevice);

        // Start recording (but in a paused state)
        mHandle = Bass.RecordStart(frequency, 2, BassFlags.RecordPause, AudioChunkCaptured);
        
        // Output all devices, then select one
        // foreach (var device in RecordingDevice.Enumerate())
        //     Console.WriteLine($"{device?.Index}: {device?.Name}");
        //
        // var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MBass");
        // Directory.CreateDirectory(outputPath);
        // var filePath = Path.Combine(outputPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".wav");
        // using var writer = new WaveFileWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read), new WaveFormat());
    } 
    
    #endregion
    
    #region Channel Configuration Methods
    
    public Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync() =>
        Task.FromResult(new List<ChannelConfigurationItem>(new []
        {
            new ChannelConfigurationItem("Mono Stereo Configuration", "Mono", "Mono"),
            new ChannelConfigurationItem("Mono Stereo Configuration", "Stereo", "Stereo"),
            new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, R, Ls, Rs, C, LFE)", "5.1 DTS"),
            new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, R, C, LFE, Ls, Rs)", "5.1 ITU"),
            new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, C, R, Ls, Rs, LFE)", "5.1 FILM"),
        }));
    
    #endregion
    
    #region Capture Audio Methods
    
    /// <summary>
    /// Call back from the audio recording, to process each chunk of audio data
    /// </summary>
    /// <param name="handle">The device handle</param>
    /// <param name="buffer">The chunk of audio data</param>
    /// <param name="length">The length of the audio data chunk</param>
    /// <param name="user"></param>
    /// <returns></returns>
    private bool AudioChunkCaptured(int handle, IntPtr buffer, int length, IntPtr user)
    {
        if (mBuffer == null || mBuffer.Length < length)
            mBuffer = new byte[length];

        Marshal.Copy(buffer, mBuffer, 0, length);

        // Calculate useful information from this chunk
        CalculateValues(mBuffer);

        return true;
    }
    
    /// <summary>
    /// Calculates usable information from an audio chunk
    /// </summary>
    /// <param name="buffer">The audio buffer</param>
    private void CalculateValues(byte[] buffer)
    {
        // Get total PCM16 samples in this buffer (16 bits per sample)
        var sampleCount = buffer.Length / 2;

        // Create our Discrete Signal ready to be filled with information
        var signal = new DiscreteSignal(mCaptureFrequency, sampleCount);

        // Loop all bytes and extract the 16 bits, into signal floats
        using var reader = new BinaryReader(new MemoryStream(buffer));

        for (var i = 0; i < sampleCount; i++)
            signal[i] = reader.ReadInt16() / 32768f;
            
        // Calculate the LUFS
        var lufs = Scale.ToDecibel(signal.Rms() * 1.2);
        mLufs.Enqueue(lufs);
            
        // Keep list to 10 samples
        if (mLufs.Count > 10)
            mLufs.Dequeue();

        // Calculate the average
        var averageLufs = mLufs.Average();

        // Fire off this chunk of information to listeners
        AudioChunkAvailable?.Invoke(new AudioChunkData
        (
            // TODO: Make these calculations correct
            ShortTermLUFS: averageLufs,
            Loudness: lufs,
            LoudnessRange: lufs * 0.9,
            RealtimeDynamics:  lufs * 0.8,
            AverageRealtimeDynamics:  lufs * 0.7,
            TruePeakMax:  lufs * 0.6,
            IntegratedLUFS:  lufs * 0.5,
            MomentaryMaxLUFS:  lufs * 0.4,
            ShortTermMaxLUFS:  lufs * 0.3
        ));
    }
    
    #endregion
    
    #region Public Control Methods
    
    /// <inheritdoc />
    public void Start() => Bass.ChannelPlay(mHandle);

    /// <inheritdoc />
    public void Stop() => Bass.ChannelStop(mHandle);

    #endregion

    #region Dispose
    
    /// <summary>
    /// Dipose of all handles
    /// </summary>
    public void Dispose()
    {
        Bass.CurrentRecordingDevice = mDevice;

        Bass.RecordFree();
    }
    
    #endregion
}