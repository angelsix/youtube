using AvaloniaLoudnessMeter.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaLoudnessMeter.Services;

public interface IAudioCaptureService
{
    /// <summary>
    /// Fetch the channel configurations
    /// </summary>
    /// <returns></returns>
    Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync();

    /// <summary>
    /// Initialises capturing of audio from a specific device 
    /// </summary>
    /// <param name="deviceId">The device ID</param>
    /// <param name="frequency">The frequency</param>
    void InitCapture(int deviceId = 1, int frequency = 44100);

    /// <summary>
    /// Start capturing audio
    /// </summary>
    void Start();

    /// <summary>
    /// Stop capturing audio
    /// </summary>
    void Stop();

    /// <summary>
    /// A callback for when the next chunk of audio data is available
    /// </summary>
    event Action<AudioChunkData> AudioChunkAvailable;
}