using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Dashboard.Monitoring;
using Gaucho.Server.Monitoring;

namespace Gaucho.Dashboard.Pages
{
    public partial class PipelinePartial
    {
        public PipelinePartial(PipelineMetric pipeline)
        {
            Pipeline = pipeline;
        }

        public PipelineMetric Pipeline { get; set; }
    }
}
