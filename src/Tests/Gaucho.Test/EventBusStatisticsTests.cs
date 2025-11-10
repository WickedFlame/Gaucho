using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using NUnit.Framework;
using AwesomeAssertions;

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
			stats.Count().Should().Be(5);

			stats.GetMetricValue(MetricType.ThreadCount).Should().Be(1);
			stats.GetMetricValue(MetricType.QueueSize).Should().Be(0);
			stats.GetMetricValue(MetricType.ProcessedEvents).Should().Be(0);

			stats.GetMetricValue(MetricType.EventLog).Should().NotBeNull();
			stats.GetMetricValue(MetricType.WorkersLog).Should().NotBeNull();

			GlobalConfiguration.Setup(c => c.Register<IStorage>(new InmemoryStorage()));
		}

		[Test]
		public void EventBus_StatisticsApi_Registered_Metrics()
		{
			GlobalConfiguration.Setup(c => c.Register<IStorage>(new InmemoryStorage()));

			var stats = new StatisticsApi("EventBus_2");
			stats.GetMetricValue(MetricType.ThreadCount).Should().BeNull();
			stats.GetMetricValue(MetricType.QueueSize).Should().BeNull();
			stats.GetMetricValue(MetricType.ProcessedEvents).Should().BeNull();

			var eventBus = new EventBus(() => null, "EventBus_2");
			eventBus.Publish(new Event("EventBus_2", new EventData()));

			stats = new StatisticsApi("EventBus_2");
			stats.GetMetricValue(MetricType.ThreadCount).Should().NotBeNull();
			stats.GetMetricValue(MetricType.QueueSize).Should().NotBeNull();
			stats.GetMetricValue(MetricType.ProcessedEvents).Should().NotBeNull();

			GlobalConfiguration.Setup(c => c.Register<IStorage>(new InmemoryStorage()));
		}
	}
}
