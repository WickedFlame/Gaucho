using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Storage.Inmemory;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test.Storage
{
	public class ValueItemTests
	{
		[Test]
		public void ValueItem_Ctor()
		{
			new ValueItem().Should().NotBeNull();
		}

		[Test]
		public void ValueItem_SetValue()
		{
			var item = new ValueItem();
			item.SetValue("value");
			item.Value.Should().Be("value");
		}

		[Test]
		public void ValueItem_GetValue()
		{
			var item = new ValueItem();
			item.SetValue("value");
			item.GetValue().Should().Be("value");
		}
	}
}
