using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server.Monitoring;

namespace Gaucho.Dashboard.Pages
{
    public partial class PipelinePartial
    {
        public PipelinePartial(PipelineMonitor pipeline)
        {
            Pipeline = pipeline;
        }

        public PipelineMonitor Pipeline { get; set; }
    }
}
