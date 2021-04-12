using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server.Monitoring;
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
		public void StatisticsAPi_CheckMetricValue()
		{
			var statistics = new StatisticsApi("stats_checkmetricvalue");
			var metric = new Metric(MetricType.ProcessedEvents, "stats", true);
			statistics.SetMetric(metric);

			Assert.AreEqual(true, statistics.GetMetricValue(MetricType.ProcessedEvents));
		}

		[Test]
		public void StatisticsAPi_CheckMetric()
		{
			var statistics = new StatisticsApi("stats_checkmetric");
			var metric = new Metric(MetricType.ProcessedEvents, "stats", true);
			statistics.SetMetric(metric);

			Assert.AreSame(metric, statistics.GetMetric(MetricType.ProcessedEvents));
		}

		[Test]
		public void StatisticsAPi_Singleton()
		{
			var statistics = new StatisticsApi("stats_singleton");
			var metric = new Metric(MetricType.ProcessedEvents, "stats", true);
			statistics.SetMetric(metric);

			statistics = new StatisticsApi("stats_singleton");

			Assert.AreSame(metric, statistics.GetMetric(MetricType.ProcessedEvents));
		}
	}
}
