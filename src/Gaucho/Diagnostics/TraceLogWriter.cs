
namespace Gaucho.Diagnostics
{
    public class TraceLogWriter : ILogWriter
    {
        public Category Category => Category.Log;

        public void Write(string message, LogLevel level, string source)
        {
            System.Diagnostics.Trace.WriteLine($"[{source}] [{level}] {message}");
        }
    }
}
