namespace Gaucho.Server.Monitoring
{
	public static class StatisticsApiExtensions
	{
		public static T GetMetricValue<T>(this StatisticsApi metrics, MetricType type)
		{
			var metric = metrics.GetMetricValue(type);
			if (metric == null)
			{
				return default(T);
			}

			return (T)metric;
		}
	}
}
