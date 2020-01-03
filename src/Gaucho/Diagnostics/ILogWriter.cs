
namespace Gaucho.Diagnostics
{
    public interface ILogWriter
    {
        Category Category { get; }

        void Write(ILogEvent @event);
    }

    public interface ILogWriter<T> : ILogWriter where T : ILogEvent
    {
        void Write(T @event);
    }
}
