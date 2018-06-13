using System;

namespace DependencyExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Inject specific services
            DependencyProvider.FileManager = new MockFileManager { FailOnPurpose = true };
            DependencyProvider.Logger = new Logger();

            // Write some file, and read it back
            var data = "this is my test";
            var path = @"C:\Users\Luke\Desktop\test.txt";

            // Log
            DependencyProvider.Logger.LogMessage($"Writing `{data}` to `{path}`");

            // Write
            DependencyProvider.FileManager.WriteFileData(path, data);

            // Read back
            var readBack = DependencyProvider.FileManager.GetFileData(path);

            // Log
            DependencyProvider.Logger.LogMessage($"Read back `{readBack}`");

            Console.Read();
        }
    }
}
