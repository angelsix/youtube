using System;

namespace DependencyExample
{
    public class Logger : ILogger
    {
        public void LogMessage(string message)
        {
            Console.WriteLine("LOG: " + message);
        }
    }
}
