using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Server.Monitoring;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventBusStatisticsTests
	{
		[Test]
		public void EventBus_StatisticsApi_DefautlMetrics()
		{
			var stats = new StatisticsApi("EventBus_1");

			var eventBus = new EventBus(() => null, "EventBus_1");

			Assert.AreEqual(3, stats.Count());
		}

		[Test]
		public void EventBus_StatisticsApi_RegisteredMetrics()
		{
			var stats = new StatisticsApi("EventBus_1");
			Assert.IsNull(stats.GetMetricValue(MetricType.ThreadCount));
			Assert.IsNull(stats.GetMetricValue(MetricType.QueueSize));
			Assert.IsNull(stats.GetMetricValue(MetricType.WorkersLog));

			var eventBus = new EventBus(() => null, "EventBus_1");

			Assert.IsNotNull(stats.GetMetricValue(MetricType.ThreadCount));
			Assert.IsNotNull(stats.GetMetricValue(MetricType.QueueSize));
			Assert.IsNotNull(stats.GetMetricValue(MetricType.WorkersLog));
		}

		
	}
}
