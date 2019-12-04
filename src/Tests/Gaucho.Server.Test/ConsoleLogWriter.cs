using Gaucho.Diagnostics;

namespace Gaucho.Server.Test
{
    public class ConsoleLogWriter : ILogWriter
    {
        public Category Category => Category.Log;

        public void Write(string message, LogLevel level, string source)
        {
            System.Console.WriteLine($"[{source}] [{level}] {message}");
        }
    }
}
