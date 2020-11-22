using Gaucho.Configuration;
using Gaucho.Server.Monitoring;
using System.Collections.Generic;

namespace Gaucho.Diagnostics
{
	/// <summary>
	/// Writes LogEvents to the Statistics API
	/// </summary>
	public class LogEventStatisticWriter : ILogWriter<ILogMessage>
	{
		private readonly List<ILogMessage> _logQueue = new List<ILogMessage>();
		private readonly LogLevel _minLogLevel;

		public LogEventStatisticWriter(StatisticsApi statistic)
		{
			statistic.AddMetricsCounter(new Metric(MetricType.EventLog, "Logs", () => _logQueue));
			_minLogLevel = GlobalConfiguration.Configuration.Resolve<Options>().LogLevel;
		}

		public Category Category => Category.Log;

		public void Write(ILogEvent @event)
		{
			if (@event is ILogMessage e)
			{
				Write(e);
			}
		}

		public void Write(ILogMessage @event)
		{
			if (@event.Level >= _minLogLevel)
			{
				_logQueue.Add(@event);

				if (_logQueue.Count > 1000)
				{
					ShrinkLog();
				}
			}
		}

		private void ShrinkLog()
		{
			_logQueue.RemoveRange(0, 500);
		}
	}
}
