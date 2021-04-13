using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Server;
using Gaucho.Storage;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class GlobalConfigurationDefaultOptionsTests
	{
		[TearDown]
		public void Teardown()
		{
			GlobalConfiguration.Setup(s => { });
		}

		[Test]
		public void GlobalConfiguration_Default_Options_LogLevel()
		{
			var options = GlobalConfiguration.Configuration.Resolve<Options>();
			Assert.AreEqual(options.LogLevel, LogLevel.Info);
		}

		[Test]
		public void GlobalConfiguration_Default_Options_Servername()
		{
			var options = GlobalConfiguration.Configuration.Resolve<Options>();
			Assert.AreEqual(options.ServerName, Environment.MachineName);
		}
	}
}
