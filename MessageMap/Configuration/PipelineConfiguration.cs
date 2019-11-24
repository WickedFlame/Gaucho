﻿using System;
using System.Collections.Generic;

namespace MessageMap.Configuration
{
    public class PipelineConfiguration
    {
        public string Id { get; set; }

        public HandlerNode InputHandler { get; set; }

        public List<HandlerNode> OutputHandlers { get; set; }
    }
}
