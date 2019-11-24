﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMap.Test
{
    public class LogQueueHandler : IOutputHandler
    {
        private readonly List<string> _log = new List<string>();

        public IConverter Converter { get; set; } = new Converter();

        public void Handle(Event @event)
        {
            var sb = new StringBuilder();
            if(@event.Data is EventData data)
            {
                foreach (var item in data.Properties)
                {
                    sb.Append($"[{item.Key} -> {item.Value}] ");
                }
            }
            else if (@event.Data is SingleNode node)
            {
                sb.Append(node.Value);
            }

            _log.Add(sb.ToString());
        }

        public IEnumerable<string> Log => _log;
    }
}
