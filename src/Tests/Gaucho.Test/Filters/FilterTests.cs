using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Filters;
using NUnit.Framework;

namespace Gaucho.Test
{
    [TestFixture]
    public class FilterTests
    {
        [Test]
        public void Gaucho_Filter()
        {
            var handler = new FilterOutputHandler(new EventDataConverter
            {
                new PropertyFilter("Level", "dst_lvl"),
                new PropertyFilter("Message")
            });

            var input = new LogMessage
            {
                Level = "Info",
                Message = "The message",
                Title = "title"
            };
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            var @event = new Event(Guid.NewGuid().ToString(), data);

            handler.Handle(@event);

            Assert.That(handler.Log.First() == "[dst_lvl -> Info] [Message -> The message] ");
        }

        [Test]
        public void Gaucho_Filter_FromFilterFactory()
        {
	        var converter = new EventDataConverter();
	        var filters = new List<string>
	        {
		        "Level -> dst_lvl",
		        "Message"
	        };

	        foreach (var filter in filters)
	        {
		        converter.Add(FilterFactory.BuildFilter(filter));
	        }

	        var input = new LogMessage
	        {
		        Level = "Info",
		        Message = "The message",
		        Title = "title"
	        };
	        var factory = new EventDataFactory();
	        var data = factory.BuildFrom(input);

	        data = converter.Convert(data);

	        Assert.AreEqual(data.ToString(), "{\r\n   dst_lvl -> Info\r\n   Message -> The message\r\n}");
        }

		
	}

    public class FilterOutputHandler : IOutputHandler
    {
        private readonly List<string> _log = new List<string>();

        public FilterOutputHandler(IEventDataConverter converter)
        {
            Converter = converter;
        }

        public IEnumerable<string> Log => _log;

        public IEventDataConverter Converter { get; }

        public void Handle(Event @event)
        {
            var sb = new StringBuilder();
            var data = Converter.Convert(@event.Data);
            foreach (var item in data.Properties)
            {
                sb.Append($"[{item.Key} -> {item.Value}] ");
            }

            _log.Add(sb.ToString());
        }
    }
}
