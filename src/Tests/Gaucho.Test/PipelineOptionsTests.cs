using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Configuration;
using NUnit.Framework;
using Polaroider;

namespace Gaucho.Test
{
    public class PipelineOptionsTests
    {
        [Test]
        public void PipelineOptions_Merge()
        {
            var opt = new Options
            {
                MaxProcessors = 10,
                MinProcessors = 10,
                MaxItemsInQueue = 10,
                ServerName = "localhost"
            };

            var po = new PipelineOptions();
            po.Merge(opt).MatchSnapshot();
        }

        [Test]
        public void PipelineOptions_Merge_None()
        {
            var opt = new Options
            {
                MaxProcessors = 10,
                MinProcessors = 10,
                MaxItemsInQueue = 10,
                ServerName = "localhost"
            };

            var po = new PipelineOptions
            {
                MaxProcessors = 1,
                MinProcessors = 1,
                MaxItemsInQueue = 1,
                ServerName = "tmp",
                MetricsPollingInterval = 1
            };

            po.Merge(opt).MatchSnapshot();
        }
    }
}
