using Avalonia.Controls.Shapes;
using BatchProcess3.Crash;
using HarfBuzzSharp;
using System;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace BatchProcess3.Services;

public static class CrashService
{
    private static readonly string _parentFolder = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "BatchProcess3");

    private static readonly string _crashFilePath = System.IO.Path.Combine(_parentFolder, "lastcrash.json");

    public static void SetCrashData(Exception ex)
    {
        try
        {
            Directory.CreateDirectory(_parentFolder);
            
            File.WriteAllText(_crashFilePath, JsonSerializer.Serialize(new CrashData
            {
                CrashDate = DateTimeOffset.UtcNow,
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty,
                Source = ex.TargetSite?.ToString() ?? string.Empty
            }));
        }
        catch (Exception)
        {
            // TODO: Handle system message box or other way to inform user of crash
        }
    }

    public static void ClearCrashData()
    {
        try
        {
            if (File.Exists(_crashFilePath))
                File.Delete(_crashFilePath);
        }
        catch (Exception ex)
        {
            // Ignored    
        }
    }

    public static CrashData? GetCrashData()
    {
        try
        {
            if (File.Exists(_crashFilePath))
                return JsonSerializer.Deserialize<CrashData>(File.ReadAllText(_crashFilePath));
        }
        catch (Exception ex)
        {
            ClearCrashData();
        }

        return null;
    }
}