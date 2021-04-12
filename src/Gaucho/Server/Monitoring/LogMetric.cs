using System;

namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// Metric for logmessages
	/// </summary>
	public class LogMetric : IMetric
	{
		private readonly Func<object> _factory;

		/// <summary>
		/// Creates a new instance of a metric for logs
		/// </summary>
		/// <param name="key"></param>
		/// <param name="title"></param>
		/// <param name="factory"></param>
		public LogMetric(MetricType key, string title, Func<object> factory)
		{
			Key = key;
			Title = title;

			_factory = factory;
		}

		/// <summary>
		/// Gets or sets the MetricType
		/// </summary>
		public MetricType Key { get; }

		/// <summary>
		/// Gets or sets the title
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Gets or sets the value
		/// </summary>
		public object Value => _factory.Invoke();
	}
}
