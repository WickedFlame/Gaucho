using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Filters;
using NUnit.Framework;

namespace Gaucho.Test.Filters
{
	public class FormatFilterTests
	{
		[Test]
		public void FormatFilter()
		{
			var filter = new FormatFilter("tmp", "${lvl}_error_${Message}");

			var data = new EventData()
				.Add("lvl", "Debug")
				.Add("Message", "theMessage");

			var result = filter.Filter(data);

			Assert.AreEqual(result.Value, "Debug_error_theMessage");
			Assert.AreEqual(result.Key, "tmp");
		}

		[Test]
		public void FormatFilter_InvalidProperty()
		{
			var filter = new FormatFilter("tmp", "${lvl}_error_${message}");

			var data = new EventData()
				.Add("lvl", "Debug")
				.Add("Message", "theMessage");

			var result = filter.Filter(data);

			Assert.AreEqual(result.Value, "Debug_error_${message}");
			Assert.AreEqual(result.Key, "tmp");
		}
	}
}
