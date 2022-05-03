using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;

namespace Gaucho.Configuration
{
	/// <summary>
	/// The options
	/// </summary>
	public class Options
	{
		/// <summary>
		/// Gets or sets the LogLevel. Defaults to Info
		/// </summary>
		public LogLevel LogLevel { get; set; } = LogLevel.Info;

		/// <summary>
		/// Gets or sets the name of the Serverinstance. Uses Machinename if none is provided
		/// </summary>
		public string ServerName { get; set; }

		/// <summary>
		/// Gets or sets the interval for publishing the Hearbeat to the storage in milliseconds. Defaults to 2 minuts or 120000 milliseconds
		/// </summary>
		public int HeartbeatInterval { get; set; } = 120000;

		/// <summary>
		/// Gets or sets the maximum log size. The log will be shrinked to the size of <see cref="LogShrinkSize"/>
		/// </summary>
        public int MaxLogSize { get; set; } = 100;

		/// <summary>
		/// The size that the log gets shrinked to. This should be about half of <see cref="MaxLogSize"/>
		/// </summary>
        public int LogShrinkSize { get; set; } = 50;

        /// <summary>
        /// Gets the maximum items that are allowed per thread in the queue
        /// </summary>
        public int MaxItemsInQueue { get; set; } = 20;

        /// <summary>
        /// Gets the maximum amount of <see cref="EventProcessor"/> that are created to work on the queue
        /// </summary>
        public int MaxProcessors { get; set; } = 20;

        private int _minProcessors = 1;

        /// <summary>
        /// Gets the minimal amount of default workers. Workers are added as the queue size gets bigger.
        /// </summary>
        public int MinProcessors
        {
            get => _minProcessors;
            set => _minProcessors = value < 1 ? 1 : value;
        }

        private int _metricsPollingInterval = 5;

        /// <summary>
        /// Gets the interval in seconds that the <see cref="EventBusMetricCounterTask"/> uses to write the metrics to the storage. 
        /// </summary>
		public int MetricsPollingInterval
        {
            get => _metricsPollingInterval;
            set => _metricsPollingInterval = value < 1 ? 1 : value;
        }
	}
	
	/// <summary>
	/// Extensions and Logic for the options
	/// </summary>
	public static class OptionsExtensions
	{
		/// <summary>
		/// Merge the options objects
		/// </summary>
		/// <param name="defaultOptions"></param>
		/// <param name="merge"></param>
		public static void Merge(this Options defaultOptions, Options merge)
		{
			if (defaultOptions.LogLevel != merge.LogLevel && merge.LogLevel != LogLevel.Info)
			{
				defaultOptions.LogLevel = merge.LogLevel;
			}

			if (!string.IsNullOrEmpty(merge.ServerName))
			{
				defaultOptions.ServerName = merge.ServerName;
			}

			if(defaultOptions.HeartbeatInterval == 120000)
			{
				defaultOptions.HeartbeatInterval = merge.HeartbeatInterval;
			}

            if (defaultOptions.MaxLogSize == 100)
            {
				defaultOptions.MaxLogSize = merge.MaxLogSize;
            }

            if (defaultOptions.LogShrinkSize == 50)
            {
                defaultOptions.LogShrinkSize = merge.LogShrinkSize;
            }

            if (defaultOptions.MaxItemsInQueue == 20)
            {
				defaultOptions.MaxItemsInQueue = merge.MaxItemsInQueue;
            }

            if (defaultOptions.MaxProcessors == 10)
            {
				defaultOptions.MaxProcessors = merge.MaxProcessors;
            }

            if (defaultOptions.MinProcessors == 1)
            {
                defaultOptions.MinProcessors = merge.MinProcessors;
            }

            if (defaultOptions.MetricsPollingInterval == 5)
            {
                defaultOptions.MetricsPollingInterval = merge.MetricsPollingInterval;
            }
		}
	}
}
