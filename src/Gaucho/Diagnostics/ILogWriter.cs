
namespace Gaucho.Diagnostics
{
    public interface ILogWriter
    {
        Category Category { get; }

        void Write(string message, LogLevel level, string source = null);
    }
}
