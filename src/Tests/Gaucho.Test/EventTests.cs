using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Polaroider;

namespace Gaucho.Test
{
	public class EventTests
	{
		[Test]
		public void Event_StringToSimpleData()
		{
			var e = new Event("", "data");
			e.Data.Should().BeOfType<SimpleData>();
		}

		[Test]
		public void Event_DataFactory()
		{
			var item = new
			{
				Id =1,
				Value = "value"
			};
			var e = new Event("id", f => f.BuildFrom(item));

			var options = SnapshotOptions.Create(o =>
			{
				// ignore regex when comparing
				o.AddDirective(s => s.ReplaceGuid());
			});

			e.MatchSnapshot(options);
		}
	}
}
