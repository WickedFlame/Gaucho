using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MessageMap.Test
{
    [TestFixture]
    public class FilterTests
    {
        [Test]
        public void MessageMap_Filter()
        {
            var handler = new FilterOutputHandler
            {
                Converter = new Converter
                {
                    new PropertyFilter("Level", "dst_lvl"),
                    new PropertyFilter("Message")
                }
            };

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
    }

    public class FilterOutputHandler : IOutputHandler
    {
        private readonly List<string> _log = new List<string>();

        public IEnumerable<string> Log => _log;

        public IConverter Converter { get; set; } = new Converter();

        public void Handle(Event @event)
        {
            var sb = new StringBuilder();
            var data = Converter.Convert(@event.Data as EventData);
            foreach (var item in data.Properties)
            {
                sb.Append($"[{item.Key} -> {item.Value}] ");
            }

            _log.Add(sb.ToString());
        }
    }
}
