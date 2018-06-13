using System;
using System.IO;

namespace DependencyExample
{
    public class WindowsFileManager : IFileManager
    {
        public string GetFileData(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteFileData(string path, string data)
        {
            DependencyProvider.Logger.LogMessage($"About to write `{data}` to `{path}`");

            File.WriteAllText(path, data);
        }
    }
}
