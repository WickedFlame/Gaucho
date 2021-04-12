namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// A metric item
	/// </summary>
	public class Metric : IMetric
    {
		/// <summary>
		/// Creates a new instance of a metric
		/// </summary>
		/// <param name="key"></param>
		/// <param name="title"></param>
		/// <param name="value"></param>
        public Metric(MetricType key, string title, object value)
        {
            Key = key;
            Title = title;

            Value = value;
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
        public object Value { get; }
    }
}
