using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Server;
using Gaucho.Storage;
using NUnit.Framework;
using AwesomeAssertions;

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
			options.LogLevel.Should().Be(LogLevel.Info);
		}

		[Test]
		public void GlobalConfiguration_Default_Options_Servername()
		{
			var options = GlobalConfiguration.Configuration.Resolve<Options>();
			options.ServerName.Should().Be(Environment.MachineName);
		}
	}
}
