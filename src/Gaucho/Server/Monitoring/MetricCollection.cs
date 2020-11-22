using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Server.Monitoring
{
	public class MetricCollection : IEnumerable<IMetric>
	{
		private readonly List<IMetric> _metrics = new List<IMetric>();

		public void Add(IMetric metric)
		{
			_metrics.Add(metric);
		}

		public IMetric Get(MetricType type)
		{
			return _metrics.FirstOrDefault(m => m.Key == type);
		}

		public IEnumerator<IMetric> GetEnumerator()
		{
			return _metrics.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
