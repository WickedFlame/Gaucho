
namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// A metric item
	/// </summary>
	public interface IMetric
    {
	    /// <summary>
	    /// Gets or sets the MetricType
	    /// </summary>
		MetricType Key { get; }

	    /// <summary>
	    /// Gets or sets the title
	    /// </summary>
		string Title { get; }

	    /// <summary>
	    /// Gets or sets the value
	    /// </summary>
		object Value { get; }
    }
}
