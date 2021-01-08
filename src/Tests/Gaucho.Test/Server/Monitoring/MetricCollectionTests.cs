using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server.Monitoring;
using NUnit.Framework;

namespace Gaucho.Test.Server.Monitoring
{
	public class MetricCollectionTests
	{
		[Test]
		public void MetricCollection_Ctor()
		{
			var col = new MetricCollection();
		}

		[Test]
		public void MetricCollection_Ctor_Initializer()
		{
			var col = new MetricCollection
			{
				new Metric(MetricType.EventLog, "m", () => true)
			};

			Assert.IsTrue(col.Get(MetricType.EventLog).Title == "m");
		}

		[Test]
		public void MetricCollection_Add()
		{
			var col = new MetricCollection();
			col.Add(new Metric(MetricType.EventLog, "m", () => true));
		}

		[Test]
		public void MetricCollection_Get()
		{
			var metric = new Metric(MetricType.EventLog, "m", () => true);

			var col = new MetricCollection
			{
				metric
			};

			Assert.AreSame(metric, col.Get(MetricType.EventLog));
		}
	}
}
