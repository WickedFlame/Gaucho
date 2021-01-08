using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Storage;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class GlobalConfigurationTests
	{
		[Test]
		public void GlobalConfiguration_Default_Storage()
		{
			var storage = GlobalConfiguration.Configuration.Resolve<IStorage>();
			Assert.IsAssignableFrom<InmemoryStorage>(storage);
		}

		[Test]
		public void GlobalConfiguration_Default_Options()
		{
			var options = GlobalConfiguration.Configuration.Resolve<Options>();
			Assert.IsAssignableFrom<Options>(options);
		}
	}
}
