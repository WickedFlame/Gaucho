﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMap.Configuration
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

        public List<FilterNode> Filters { get; set; }
    }
}