namespace AvaloniaLoudnessMeter.Services;

/// <summary>
/// Holds all the information about a single chunk of audio for display in the UI
/// </summary>
/// <param name="Loudness"></param>
/// <param name="ShortTermLUFS"></param>
/// <param name="IntegratedLUFS"></param>
/// <param name="LoudnessRange"></param>
/// <param name="RealtimeDynamics"></param>
/// <param name="AverageRealtimeDynamics"></param>
/// <param name="MomentaryMaxLUFS"></param>
/// <param name="ShortTermMaxLUFS"></param>
/// <param name="TruePeakMax"></param>
public record AudioChunkData (
    double Loudness,
    double ShortTermLUFS, 
    double IntegratedLUFS,
    double LoudnessRange,
    double RealtimeDynamics,
    double AverageRealtimeDynamics,
    double MomentaryMaxLUFS,
    double ShortTermMaxLUFS,
    double TruePeakMax
    );