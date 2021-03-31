using Gaucho.Diagnostics;

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
		}
	}
}
