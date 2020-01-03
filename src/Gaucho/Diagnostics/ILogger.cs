
namespace Gaucho.Diagnostics
{
    public interface ILogger
    {
        void Write<T>(T @event, Category category) where T : ILogEvent;
    }
}
