using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using AwesomeAssertions;

namespace Gaucho.Test.Configuration
{
	public class OptionsTests
	{
		[Test]
		public void Options_LogLevel_Default()
		{
			var def = new Options();
			def.LogLevel.Should().Be(LogLevel.Info);
		}

		[Test]
		public void Options_Merge_LogLevel_Default()
		{
			var def = new Options();
			def.Merge(new Options());
			def.LogLevel.Should().Be(LogLevel.Info);
		}

		[Test]
		public void Options_Merge_LogLevel_Overwrite()
		{
			var def = new Options();
			def.Merge(new Options {LogLevel = LogLevel.Warning});
			def.LogLevel.Should().Be(LogLevel.Warning);
		}

		[Test]
		public void Options_Merge_LogLevel_DefaultNotDefault()
		{
			var def = new Options { LogLevel = LogLevel.Warning };
			def.Merge(new Options());
			def.LogLevel.Should().Be(LogLevel.Warning);
		}

		[Test]
		public void Options_Merge_ServerName_Default()
		{
			var def = new Options { ServerName = "original" };
			def.Merge(new Options());
			def.ServerName.Should().Be("original");
		}

		[Test]
		public void Options_Merge_ServerName_Overwrite()
		{
			var def = new Options {ServerName = "original"};
			def.Merge(new Options { ServerName = "overwrite" });
			def.ServerName.Should().Be("overwrite");
		}

		[Test]
		public void Options_Merge_HeartbeatInterval_Default()
		{
			var def = new Options { HeartbeatInterval =20};
			def.Merge(new Options());
			def.HeartbeatInterval.Should().Be(20);
		}

		[Test]
		public void Options_Merge_HeartbeatInterval_Overwrite()
		{
			var def = new Options();
			def.Merge(new Options { HeartbeatInterval =22 });
			def.HeartbeatInterval.Should().Be(22);
		}
	}
}
