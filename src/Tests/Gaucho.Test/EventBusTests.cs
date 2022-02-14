using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gaucho.Server.Monitoring;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventBusTests
	{
		[Test]
		public void EventBus_Ctor()
		{
			var eventBus = new EventBus(() => new EventPipeline(), "pipelineId");
		}

		[Test]
		public void EventBus_Process()
		{
			var eventBus = new EventBus(() => new EventPipeline(), "EventBus_2");
			eventBus.Publish(new Event("EventBus_2", "data"));

			eventBus.WaitAll();

            Task.Delay(1000).Wait();

			// event was not processed
			var stats = new StatisticsApi("EventBus_2");
			Assert.AreEqual(0, (int)stats.GetMetricValue(MetricType.QueueSize));
		}

		[Test]
		public void EventBus_Process_WithouPipeline()
		{
			var eventBus = new EventBus(() => null, "EventBus_3");
			eventBus.Publish(new Event("EventBus_3", "data"));

			eventBus.WaitAll();
            
            Task.Delay(1000).Wait();

			// event was not processed
			var stats = new StatisticsApi("EventBus_3");
			Assert.AreEqual(1, (int)stats.GetMetricValue(MetricType.QueueSize));
		}

		[Test]
		public void EventBus_DefaultThread()
		{
			new EventBus(() => null, "EventBus_4");

			var stats = new StatisticsApi("EventBus_4");
			Assert.AreEqual(stats.GetMetricValue(MetricType.ThreadCount), 1);
		}

		[Test]
		public void EventBus_Close()
		{
			var eventBus = new EventBus(() => null, "EventBus_5");
			eventBus.Close();
			
			var stats = new StatisticsApi("EventBus_5");
			Assert.IsTrue((int)stats.GetMetricValue(MetricType.QueueSize) == 0);
		}
	}
}
