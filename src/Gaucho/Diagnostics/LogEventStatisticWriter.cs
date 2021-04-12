using System;
using Gaucho.Configuration;
using Gaucho.Server.Monitoring;
using System.Collections.Generic;
using Gaucho.Storage;

namespace Gaucho.Diagnostics
{
	/// <summary>
	/// Writes LogEvents to the Statistics API
	/// </summary>
	public class LogEventStatisticWriter : ILogWriter<ILogMessage>
	{
		private List<ILogMessage> _logQueue;
		private readonly LogLevel _minLogLevel;
		private readonly Lazy<IStorage> _storage;
		private readonly string _pipelineId;

		/// <summary>
		/// Creates a new instance of LogEventStatisticWriter
		/// </summary>
		/// <param name="statistic"></param>
		public LogEventStatisticWriter(StatisticsApi statistic)
		{
			//statistic.AddMetricsCounter(new Metric(MetricType.EventLog, "Logs", () =>
			//{
			//	if (_logQueue == null)
			//	{
			//		InitLogQueue();
			//	}

			//	return _logQueue;
			//}));
			_pipelineId = statistic.PipelineId;

			_minLogLevel = GlobalConfiguration.Configuration.Resolve<Options>().LogLevel;
			_storage = new Lazy<IStorage>(() => GlobalConfiguration.Configuration.Resolve<IStorage>());
		}

		/// <summary>
		/// The loggercategory
		/// </summary>
		public Category Category => Category.Log;

		/// <summary>
		/// Write the ILogEvent to the Logs
		/// </summary>
		/// <param name="event"></param>
		public void Write(ILogEvent @event)
		{
			if (@event is ILogMessage e)
			{
				Write(e);
			}
		}

		/// <summary>
		/// Write the ILogMessage to the Logs
		/// </summary>
		/// <param name="event"></param>
		public void Write(ILogMessage @event)
		{
			if (_logQueue == null)
			{
				InitLogQueue();
			}

			if (@event.Level >= _minLogLevel)
			{
				_logQueue.Add(@event);
				_storage.Value.AddToList(new StorageKey(_pipelineId, "logs"), @event);

				if (_logQueue.Count > 500)
				{
					ShrinkLog();
				}
			}
		}

		private void InitLogQueue()
		{
			if (_logQueue != null)
			{
				return;
			}

			_logQueue = new List<ILogMessage>();
			var logs = _storage.Value.GetList<LogEvent>(new StorageKey(_pipelineId, "logs"));
			if (logs != null)
			{
				_logQueue.AddRange(logs);
			}
		}

		private void ShrinkLog()
		{
			_logQueue.RemoveRange(0, 500);
			_storage.Value.RemoveRangeFromList(new StorageKey(_pipelineId, "logs"), 250);
		}
	}
}
