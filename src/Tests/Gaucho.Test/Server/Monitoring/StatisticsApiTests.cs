using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test.Server.Monitoring
{
	public class StatisticsApiTests
	{
		[Test]
		public void StatisticsAPi_Ctor()
		{
			var statistics = new StatisticsApi("stats_ctor");
		}

		[Test]
		public void StatisticsApi_Registered_WorkersLog()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<ActiveWorkersLogMessage>(It.Is<StorageKey>(k => k.Key == $"logs:{MetricType.WorkersLog}"))).Returns(() => new[] { new ActiveWorkersLogMessage() });

			var stats = new StatisticsApi("EventBus_2", storage.Object);

			Assert.IsNotNull(stats.GetMetricValue(MetricType.WorkersLog));
		}

		[Test]
		public void StatisticsApi_Registered_EventLog()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.GetList<LogEvent>(It.Is<StorageKey>(k => k.Key == $"logs"))).Returns(() => new[] {new LogEvent()});

			var stats = new StatisticsApi("EventBus_2", storage.Object);

			Assert.IsNotNull(stats.GetMetricValue(MetricType.EventLog));
		}

		[Test]
		public void StatisticsAPi_GetMetricValue_ThreadCount()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<Metric>(It.Is<StorageKey>(k => k.Key == $"metric:{MetricType.ThreadCount}"))).Returns(() => new Metric(MetricType.ThreadCount, "stats", true));
			storage.Setup(exp => exp.GetKeys(It.Is<StorageKey>(k => k.Key == $"metric:"))).Returns(() => new[] {$"metric:{MetricType.ThreadCount}"});
			var statistics = new StatisticsApi("stats_checkmetricvalue", storage.Object);

			Assert.AreEqual(true, statistics.GetMetricValue(MetricType.ThreadCount));
		}

		[Test]
		public void StatisticsAPi_GetMetricValue_QueueSize()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<Metric>(It.Is<StorageKey>(k => k.Key == $"metric:{MetricType.QueueSize}"))).Returns(() => new Metric(MetricType.QueueSize, "stats", 1));
			storage.Setup(exp => exp.GetKeys(It.Is<StorageKey>(k => k.Key == $"metric:"))).Returns(() => new[] { $"metric:{MetricType.QueueSize}" });
			var statistics = new StatisticsApi("stats_checkmetricvalue", storage.Object);

			Assert.AreEqual(1, statistics.GetMetricValue(MetricType.QueueSize));
		}

		[Test]
		public void StatisticsAPi_GetMetricValue_ProcessedEvents()
		{
			var storage = new Mock<IStorage>();
			storage.Setup(exp => exp.Get<Metric>(It.Is<StorageKey>(k => k.Key == $"metric:{MetricType.ProcessedEvents}"))).Returns(() => new Metric(MetricType.ProcessedEvents, "stats", 5));
			storage.Setup(exp => exp.GetKeys(It.Is<StorageKey>(k => k.Key == $"metric:"))).Returns(() => new[] { $"metric:{MetricType.ProcessedEvents}" });
			var statistics = new StatisticsApi("stats_checkmetricvalue", storage.Object);

			Assert.AreEqual(5, statistics.GetMetricValue(MetricType.ProcessedEvents));
		}
	}
}
