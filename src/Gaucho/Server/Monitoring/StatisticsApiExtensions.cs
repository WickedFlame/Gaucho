namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// Extensions for <seealso cref="StatisticsApi"/>
	/// </summary>
	public static class StatisticsApiExtensions
	{
		/// <summary>
		/// Get the value of a MetricType
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="metrics"></param>
		/// <param name="type"></param>
		/// <returns></returns>
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
