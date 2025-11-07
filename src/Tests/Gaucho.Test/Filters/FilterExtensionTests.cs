using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Filters;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test.Filters
{
	public class FilterExtensionTests
	{
		[Test]
		public void FilterExtensions_BuildDataFilter()
		{
			var converter = new[] { "Level -> dst_lvl", "Message" }.BuildDataFilter();

			converter.Filters.Count().Should().Be(2);
		}

		[Test]
		public void FilterExtensions_BuildDataFilter_TypeCheck()
		{
			var converter = new[] {"Level -> dst_lvl", "Message"}.BuildDataFilter();

			converter.Filters.All(f => f.FilterType == FilterType.Property).Should().BeTrue();
		}
	}
}
