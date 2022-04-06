using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Polaroider;

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

		[Test]
		public void EventDataFactory_Dictionary()
		{
			var data = new Dictionary<string, object>
			{
				["Id"] = 1,
				["Message"]= "this is a test"
			};
			var factory = new EventDataFactory();
			var eventData = factory.BuildFrom(data);

			Assert.That(eventData["Id"].Equals(1));
			Assert.That(eventData["Message"].Equals("this is a test"));
		}

        [Test]
		public void EventDataFactory_Complex()
		{
			var data = new ComplexData
			{
				Index = 1,
				MessageData = new MessageData
				{
					Message = "test"
				},
				Inner = new ComplexData
				{
					Index = 2,
					MessageData = new MessageData
					{
						Message = "inner"
					}
				}
			};

			var factory = new EventDataFactory();
			var eventData = factory.BuildFrom(data);

			eventData.MatchSnapshot();
		}

        [Test]
        public void EventDataFactory_Array()
        {
            var data = new 
            {
                Index = 1,
                Data = new []
                {
                    new { test = "one"},
                    new { test = "two"}
				}
            };

            var factory = new EventDataFactory();
            var eventData = factory.BuildFrom(data);

            eventData.MatchSnapshot();
        }

		public class MessageData
		{
			public string Message { get; set; }
		}

		public class ComplexData
		{
			public int Index { get; set; }

			public MessageData MessageData { get; set; }

			public ComplexData Inner { get; set; }
		}
	}
}
