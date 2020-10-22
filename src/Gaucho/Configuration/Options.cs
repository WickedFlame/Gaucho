using Gaucho.Diagnostics;

namespace Gaucho.Configuration
{
	public class Options
	{
		public LogLevel LogLevel { get; set; } = LogLevel.Info;
	}

	public static class OptionsExtensions
	{
		public static void Merge(this Options orig, Options merge)
		{
			if (orig.LogLevel != merge.LogLevel)
			{
				orig.LogLevel = merge.LogLevel;
			}
		}
	}
}
