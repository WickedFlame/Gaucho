using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;

namespace Gaucho.Diagnostics.MetricCounters
{
	/// <summary>
	/// Logs the count workers for a statistics graph
	/// </summary>
	public class WorkersLogMetricCounter : ILogWriter<StatisticEvent<int>>
	{
		private IList<WorkerCountMetric> _log;
		private readonly string _pipelineId;
		private IStorage _storage;

		/// <summary>
		/// Creates a workers log metric counter
		/// </summary>
		/// <param name="statistic"></param>
		public WorkersLogMetricCounter(StatisticsApi statistic)
		{
			statistic.AddMetricsCounter(new Metric(MetricType.WorkersLog, "Active Workers", () => _log));
			_pipelineId = statistic.PipelineId;
		}

		/// <summary>
		/// the category that is loged
		/// </summary>
		public Category Category => Category.EventStatistic;

		/// <summary>
		/// default write method
		/// </summary>
		/// <param name="event"></param>
		public void Write(ILogEvent @event)
		{
			if (@event is StatisticEvent<int> e)
			{
				Write(e);
			}
		}

		/// <summary>
		/// statistic event override
		/// </summary>
		/// <param name="event"></param>
		public void Write(StatisticEvent<int> @event)
		{
			if (@event.Metric != StatisticType.WorkersLog)
			{
				return;
			}

			lock (_pipelineId)
			{
				if (_storage == null)
				{
					_storage = GlobalConfiguration.Configuration.Resolve<IStorage>();
					_log = _storage.GetList<WorkerCountMetric>(_pipelineId, "WorkersLog")?.ToList() ?? new List<WorkerCountMetric>();
				}
			}

			lock (_pipelineId)
			{
				var metric = new WorkerCountMetric
				{
					Timestamp = DateTime.Now,
					PipelineId = _pipelineId,
					ActiveWorkers = @event.Value
				};
				_log.Add(metric);
				_storage.AddToList(_pipelineId, "WorkersLog", metric);
			}
		}
	}
}
