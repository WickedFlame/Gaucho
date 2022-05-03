
using System;
using System.Text;

namespace Gaucho.Diagnostics
{
    /// <summary>
    /// LogWriter that writes to the debug console
    /// </summary>
    public class DebugLogWriter : ILogWriter<LogEvent>
    {
        public Category Category => Category.Log;

        /// <summary>
        /// Write the log to the debug console
        /// </summary>
        /// <param name="event"></param>
        public void Write(ILogEvent @event)
        {
            if (@event is LogEvent e)
            {
                Write(e);
            }
        }

        /// <summary>
        /// Write the log to the debug console
        /// </summary>
        /// <param name="event"></param>
        public void Write(LogEvent @event)
        {
            var meta = Serialize(@event.MetaData);
            System.Diagnostics.Debug.WriteLine($"[{@event.Timestamp}] [{@event.Source}] [{@event.Level}] {@event.Message} {meta}");
        }

        private string Serialize(object input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder().AppendLine().AppendLine("{");

            foreach (var property in input.GetType().GetProperties())
            {
                var name = $"{property.Name}";
                var value = property.GetValue(input);

                sb.AppendLine($"   {name}: {value}");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
