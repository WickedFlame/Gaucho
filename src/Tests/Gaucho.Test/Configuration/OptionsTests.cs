using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using NUnit.Framework;

namespace Gaucho.Test.Configuration
{
	public class OptionsTests
	{
		[Test]
		public void Options_LogLevel_Default()
		{
			var def = new Options();
			Assert.AreEqual(LogLevel.Info, def.LogLevel);
		}

		[Test]
		public void Options_Merge_LogLevel_Default()
		{
			var def = new Options();
			def.Merge(new Options());
			Assert.AreEqual(LogLevel.Info, def.LogLevel);
		}

		[Test]
		public void Options_Merge_LogLevel_Overwrite()
		{
			var def = new Options();
			def.Merge(new Options {LogLevel = LogLevel.Warning});
			Assert.AreEqual(LogLevel.Warning, def.LogLevel);
		}

		[Test]
		public void Options_Merge_LogLevel_DefaultNotDefault()
		{
			var def = new Options { LogLevel = LogLevel.Warning };
			def.Merge(new Options());
			Assert.AreEqual(LogLevel.Warning, def.LogLevel);
		}

		[Test]
		public void Options_Merge_ServerName_Default()
		{
			var def = new Options { ServerName = "original" };
			def.Merge(new Options());
			Assert.AreEqual("original", def.ServerName);
		}

		[Test]
		public void Options_Merge_ServerName_Overwrite()
		{
			var def = new Options {ServerName = "original"};
			def.Merge(new Options { ServerName = "overwrite" });
			Assert.AreEqual("overwrite", def.ServerName);
		}
	}
}
