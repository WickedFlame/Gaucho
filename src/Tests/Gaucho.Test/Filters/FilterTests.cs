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

        [Test]
        public void Gaucho_Filter_ConvertDataMultipleTimes()
        {
	        var converter = new EventDataConverter();
	        var filters = new List<string>
	        {
		        "Level -> dst_lvl",
		        "Message -> msg"
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
	        
	        Assert.That(data.All(p => p.Key != "Message"));

			// the property Message -> msg cannot be converted because only msg is in the datacollection
			// still has to return the msg property if present
	        data = converter.Convert(data);

			Assert.AreEqual(data.ToString(), "{\r\n   dst_lvl -> Info\r\n   msg -> The message\r\n}");
        }

        [Test]
        public void Gaucho_Filter_MapComplexToSimple()
        {
            var converter = new EventDataConverter();
            var filters = new List<string>
            {
                "Level -> dst_lvl",
                "Sub.Message -> msg"
            };

            foreach (var filter in filters)
            {
                converter.Add(FilterFactory.BuildFilter(filter));
            }

            var input = new
            {
                Level = "Info",
                Sub = new
                {
                    Message = "The message",
                }
            };
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            data = converter.Convert(data);

            Assert.IsNotNull(data.FirstOrDefault(p => p.Key == "msg"));
            Assert.AreEqual(input.Sub.Message, data["msg"]);
        }

        [Test]
        public void Gaucho_Filter_MapComplexToSimple_NoObject()
        {
            var converter = new EventDataConverter();
            var filters = new List<string>
            {
                "Level -> dst_lvl",
                "Sub.Message -> msg"
            };

            foreach (var filter in filters)
            {
                converter.Add(FilterFactory.BuildFilter(filter));
            }

            var input = new
            {
                Level = "Info"
            };
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            Assert.IsNull(data["msg"]);
        }

        [Test]
        public void Gaucho_Filter_MapComplexToSimple_Ignore()
        {
            var converter = new EventDataConverter();
            var filters = new List<string>
            {
                "Level -> dst_lvl",
                "Sub.Message -> msg"
            };

            foreach (var filter in filters)
            {
                converter.Add(FilterFactory.BuildFilter(filter));
            }

            var input = new
            {
                Level = "Info"
            };
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            Assert.IsNull(data.FirstOrDefault(d => d.Key == "msg"));
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
            foreach (var node in data)
            {
                sb.Append($"[{node.Key} -> {node.Value}] ");
            }

            _log.Add(sb.ToString());
        }
    }
}
