using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test.Server.Monitoring
{
	public class MetricServiceTests
	{
		private Mock<IStorage> _storage;

		[SetUp]
		public void Setup()
		{
			_storage = new Mock<IStorage>();
		}

		[Test]
		public void MetricService_SetMetric()
		{
			var metrics = new MetricService(_storage.Object, "metric_SetMetric");
			var metric = new Metric(MetricType.ProcessedEvents, "stats", true);
			metrics.SetMetric(metric);

			_storage.Verify(exp => exp.Set(It.Is<StorageKey>(k => k.Key == $"metric:{MetricType.ProcessedEvents}"), It.Is<IMetric>(m => (bool) m.Value == true)), Times.Once);
		}

		[Test]
		public void MetricService_GetMetric()
		{
			_storage.Setup(exp => exp.Get<Metric>(It.Is<StorageKey>(k => k.Key == $"metric:{MetricType.ProcessedEvents}"))).Returns(() => new Metric(MetricType.ProcessedEvents, "stats",1));
			var metrics = new MetricService(_storage.Object, "stats_GetMetric");
			var metric = metrics.GetMetric(MetricType.ProcessedEvents);

			metric.Value.Should().Be(1);
		}

		[Test]
		public void MetricService_GetMetricValue()
		{
			_storage.Setup(exp => exp.Get<Metric>(It.Is<StorageKey>(k => k.Key == $"metric:{MetricType.ProcessedEvents}"))).Returns(() => new Metric(MetricType.ProcessedEvents, "stats",1));
			var metrics = new MetricService(_storage.Object, "stats_GetMetric");
			var metric = metrics.GetMetricValue<int>(MetricType.ProcessedEvents);

			metric.Should().Be(1);
		}
	}
}
