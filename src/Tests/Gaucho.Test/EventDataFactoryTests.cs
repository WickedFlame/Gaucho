using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventDataFactoryTests
	{
		[Test]
		public void EventDataFactory_BuildFrom()
		{
			var data = new MessageData { Message = "this is a test" };

			var factory = new EventDataFactory();
			var eventData = factory.BuildFrom(data);

			Assert.That(eventData["Message"].Equals("this is a test"));
		}

		[Test]
		public void EventDataFactory_Add()
		{
			var data = new MessageData {Message = "this is a test"};

			var factory = new EventDataFactory();
			var eventData = factory.BuildFrom(data)
				.Add("two", "this is 2");

			Assert.That(eventData["Message"].Equals("this is a test"));
			Assert.That(eventData["two"].Equals("this is 2"));
		}

		[Test]
		public void EventDataFactory_Add_Expression()
		{
			var data = new MessageData { Message = "this is a test" };
			var three = 3;

			var factory = new EventDataFactory();
			var eventData = factory.BuildFrom(data)
				.Add("two", "this is 2")
				.Add(() => three);

			Assert.That(eventData["Message"].Equals("this is a test"));
			Assert.That(eventData["two"].Equals("this is 2"));
			Assert.That(eventData["three"].Equals("3"));
		}

		[Test]
		public void EventDataFactory_Add_MultipleSame()
		{
			var data = new MessageData { Message = "this is a test" };
			var three = 3;

			var factory = new EventDataFactory();
			var eventData = factory.BuildFrom(data)
				.Add("two", "this is 2")
				.Add(() => three)
				.Add("two", "this is 4");

			Assert.That(eventData["Message"].Equals("this is a test"));
			Assert.That(eventData["two"].Equals("this is 2"));
			Assert.That(eventData["three"].Equals("3"));
		}

		public class MessageData
		{
			public string Message { get; set; }
		}
	}
}
