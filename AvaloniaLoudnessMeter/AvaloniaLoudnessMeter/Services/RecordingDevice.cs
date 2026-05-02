using ManagedBass;
using System;
using System.Collections.Generic;

namespace AvaloniaLoudnessMeter.Services;

public class RecordingDevice : IDisposable
{
    public int Index { get; }
    public string Name { get; }

    RecordingDevice(int index, string name)
    {
        Index = index;

        Name = name;
    }

    public static IEnumerable<RecordingDevice> Enumerate()
    {
        for (var i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            yield return new RecordingDevice(i, info.Name);
    }

    public void Dispose()
    {
        Bass.CurrentRecordingDevice = Index;
        Bass.RecordFree();
    }

    public override string ToString() => Name;
}
