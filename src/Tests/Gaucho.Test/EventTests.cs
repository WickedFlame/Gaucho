using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventTests
	{
		[Test]
		public void Event_StringToSimpleData()
		{
			var e = new Event("", "data");
			Assert.IsAssignableFrom<SimpleData>(e.Data);
		}
	}
}
