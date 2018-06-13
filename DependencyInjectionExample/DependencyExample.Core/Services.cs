using System;

namespace DependencyExample
{
    public static class DependencyProvider
    {
        // This would be from the DI service
        public static IFileManager FileManager { get; set; }
        public static ILogger Logger { get; set; }
    }

    public interface IFileManager
    {
        void WriteFileData(string path, string data);

        string GetFileData(string path);
    }

    public interface ILogger
    {
        void LogMessage(string message);
    }
}
