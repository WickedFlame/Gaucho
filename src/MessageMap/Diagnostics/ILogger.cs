
namespace MessageMap.Diagnostics
{
    public interface ILogger
    {
        void Write(string message, Category category, LogLevel level = LogLevel.Info, string source = null);
    }
}
