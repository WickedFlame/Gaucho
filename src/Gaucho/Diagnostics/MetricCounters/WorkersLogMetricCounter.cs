using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;

namespace Gaucho.Diagnostics.MetricCounters
{
	public class WorkersLogMetricCounter : ILogWriter<StatisticEvent<int>>
	{
		private IList<EventWorkerMetric> _log;
		private readonly string _pipelineId;
		private IStorage _storage;

		public WorkersLogMetricCounter(StatisticsApi statistic)
		{
			statistic.AddMetricsCounter(new Metric(MetricType.WorkersLog, "Active Workers", () => _log));
			_pipelineId = statistic.PipelineId;
		}

		public Category Category => Category.EventStatistic;


		public void Write(ILogEvent @event)
		{
			if (@event is StatisticEvent<int> e)
			{
				Write(e);
			}
		}

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
					_log = _storage.GetList<EventWorkerMetric>(_pipelineId, "WorkersLog")?.ToList() ?? new List<EventWorkerMetric>();
				}
			}

			lock (_pipelineId)
			{
				var metric = new EventWorkerMetric
				{
					TimeStamp = DateTime.Now,
					PipelineId = _pipelineId,
					ActiveWorkers = @event.Value
				};
				_log.Add(metric);
				_storage.Add(_pipelineId, "WorkersLog", metric);
			}
		}
	}

	public class EventWorkerMetric
	{
		public string PipelineId { get; set; }

		public DateTime TimeStamp { get; set; }

		public int ActiveWorkers { get; set; }
	}
}
