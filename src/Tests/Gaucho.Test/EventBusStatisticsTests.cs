using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventBusStatisticsTests
	{
		[Test]
		public void EventBus_StatisticsApi_DefautlMetrics()
		{
			GlobalConfiguration.Setup(c => c.Register<IStorage>(new InmemoryStorage()));

			var eventBus = new EventBus(() => null, "EventBus_1");

			var stats = new StatisticsApi("EventBus_1");
			Assert.AreEqual(5, stats.Count());

			Assert.AreEqual(1, stats.GetMetricValue(MetricType.ThreadCount));
			Assert.AreEqual(0, stats.GetMetricValue(MetricType.QueueSize));
			Assert.AreEqual(0, stats.GetMetricValue(MetricType.ProcessedEvents));

			Assert.IsNotNull(stats.GetMetricValue(MetricType.EventLog));
			Assert.IsNotNull(stats.GetMetricValue(MetricType.WorkersLog));

			GlobalConfiguration.Setup(c => c.Register<IStorage>(new InmemoryStorage()));
		}

		[Test]
		public void EventBus_StatisticsApi_Registered_Metrics()
		{
			GlobalConfiguration.Setup(c => c.Register<IStorage>(new InmemoryStorage()));

			var stats = new StatisticsApi("EventBus_2");
			Assert.IsNull(stats.GetMetricValue(MetricType.ThreadCount));
			Assert.IsNull(stats.GetMetricValue(MetricType.QueueSize));
			Assert.IsNull(stats.GetMetricValue(MetricType.ProcessedEvents));

			var eventBus = new EventBus(() => null, "EventBus_2");
			eventBus.Publish(new Event("EventBus_2", new EventData()));

			stats = new StatisticsApi("EventBus_2");
			Assert.IsNotNull(stats.GetMetricValue(MetricType.ThreadCount));
			Assert.IsNotNull(stats.GetMetricValue(MetricType.QueueSize));
			Assert.IsNotNull(stats.GetMetricValue(MetricType.ProcessedEvents));

			GlobalConfiguration.Setup(c => c.Register<IStorage>(new InmemoryStorage()));
		}
	}
}
