
namespace Gaucho.Diagnostics
{
    public class TraceLogWriter : ILogWriter<LogEvent>
    {
        public Category Category => Category.Log;

        public void Write(ILogEvent @event)
        {
            if (@event is LogEvent e)
            {
                Write(e);
            }
        }

        public void Write(LogEvent @event)
        {
            System.Diagnostics.Trace.WriteLine($"[{@event.Timestamp}] [{@event.Source}] [{@event.Level}] {@event.Message}");
        }
    }
}
