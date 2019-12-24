using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Diagnostics;
using Gaucho.Filters;

namespace Gaucho.Configuration
{
    public class HandlerNode
    {
        public HandlerNode() { }

        public HandlerNode(Type type)
        {
            Type = type;
        }

        public HandlerNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public List<string> Filters { get; set; }

        public Dictionary<string, string> Arguments { get; set; }
    }

    public static class HandlerNodeExtensions
    {
        public static IEventDataConverter BuildEventData(this HandlerNode node)
        {
            var converter = new EventDataConverter();
            if (node.Filters == null)
            {
                return converter;
            }

            foreach (var filterString in node.Filters)
            {
                var filter = FilterFactory.CreateFilter(filterString);
                if (filter == null)
                {
                    var logger = LoggerConfiguration.Setup();
                    logger.Write($"Could not convert '{filterString}' to a Filter", Category.Log, LogLevel.Warning, source: "FilterFactory");
                    continue;
                }

                converter.Add(filter);
            }

            return converter;
        }

        public static ConfiguredArgumentsCollection BuildArguments(this HandlerNode node)
        {
            var collection = new ConfiguredArgumentsCollection();
            if (node.Arguments == null)
            {
                return collection;
            }

            foreach (var item in node.Arguments)
            {
                collection.Add(item.Key, item.Value);
            }

            return collection;
        }
    }
}
