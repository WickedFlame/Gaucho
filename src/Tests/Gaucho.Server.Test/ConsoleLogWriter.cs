using Gaucho.Diagnostics;

namespace Gaucho.Server.Test
{
    public class ConsoleLogWriter : ILogWriter<LogEvent>
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
            System.Console.WriteLine($"[{@event.Source}] [{@event.Level}] {@event.Message}");
        }
    }
}
