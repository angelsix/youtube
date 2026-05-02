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
    /// The last few sets of captured audio bytes, converted to LUFS
    /// </summary>
    private Queue<double> mLufsLonger = new Queue<double>();

    /// <summary>
    /// The frequency to capture at
    /// </summary>
    private int mCaptureFrequency = 44100;
    
    /// <summary>
    /// A counter for how long a silence (less than 5db) of previous audio has elapsed
    /// </summary>
    private int mSilenceCount = 0;

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
    public BassAudioCaptureService()
    {
        // Initialise and start 
        Bass.Init();
    } 
    
    #endregion

    /// <inheritdoc />
    public void InitCapture(int deviceId = 1, int frequency = 44100)
    {       
        // Store device ID
        mDevice = deviceId;

        try
        {
            // Attempt to free previous resources
            Bass.RecordFree();
        }
        catch
        {
            // ignored
        }

        // Initialize new device
        Bass.RecordInit(mDevice);

        // Start recording (but in a paused state)
        mHandle = Bass.RecordStart(frequency, 2, BassFlags.RecordPause, 20, AudioChunkCaptured);
        
        // Output all devices, then select one
        // foreach (var device in RecordingDevice.Enumerate())
        //     Console.WriteLine($"{device?.Index}: {device?.Name}");
        //
        // var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MBass");
        // Directory.CreateDirectory(outputPath);
        // var filePath = Path.Combine(outputPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".wav");
        // using var writer = new WaveFileWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read), new WaveFormat());
    }
    
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
        
        // Limit queue sizes
        if (mLufs.Count > 10)
            mLufs.Dequeue();

        // Calculate the average
        var averageLufs = Math.Max(-60, mLufs.Count > 0 ? mLufs.Average() : -60);
        var averageLongLufs = Math.Max(-60, mLufsLonger.Count > 0 ? mLufsLonger.Average() : -60);

        // If current sound is lower than average by a lot...
        if (mLufsLonger.Count > 0 && averageLufs + 5 < averageLongLufs)
        {
            // Count how long a silence has gone
            mSilenceCount++;
            
            // If its been lower now for 1000 or more counts...
            if (mSilenceCount > 50)
            {
                // Start dequeuing 
                if (mLufsLonger.Count > 0)
                {
                    // Add silence in front while removing noise from behind
                    // If we dont add silence and only remove, there average
                    // will increase not decrease
                    mLufsLonger.Enqueue(-60);
                    mLufsLonger.Dequeue();
                    mLufsLonger.Dequeue();
                }
            }
        }
        // Otherwise, normal sounds to add to overall longer average...
        else
        {
            // Reset silence count
            mSilenceCount = 0;
            
            // If we are not silent...
            if (averageLufs > -60)
                // Add current audio to average
                 mLufsLonger.Enqueue(averageLufs);
            // Otherwise, start dequeuing the average while it is silent
            else if (mLufsLonger.Count > 0)
                mLufsLonger.Dequeue();
            
            // Max average
            if (mLufsLonger.Count > 60)
                mLufsLonger.Dequeue();
        }

        // Re-calculate long average
        averageLongLufs = Math.Max(-60, mLufsLonger.Count > 0 ? mLufsLonger.Average() : -60);

        // Fire off this chunk of information to listeners
        AudioChunkAvailable?.Invoke(new AudioChunkData
        (
            // TODO: Make these calculations correct
            ShortTermLUFS: averageLongLufs,
            Loudness: averageLufs,
            LoudnessRange: averageLufs + (averageLufs * 0.9),
            RealtimeDynamics:  averageLufs + (averageLufs * 0.8),
            AverageRealtimeDynamics:  averageLufs + (averageLufs * 0.7),
            TruePeakMax:  averageLufs + (averageLufs * 0.6),
            IntegratedLUFS: averageLufs + (averageLufs * 0.5),
            MomentaryMaxLUFS:  averageLufs + (averageLufs * 0.4),
            ShortTermMaxLUFS:  averageLufs + (averageLufs * 0.3)
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