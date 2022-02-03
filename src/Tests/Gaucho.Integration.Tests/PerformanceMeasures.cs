using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Handlers;
using MeasureMap;
using NUnit.Framework;

namespace Gaucho.Integration.Tests
{
    public class PerformanceMeasures
    {
        [OneTimeTearDown]
        public void TearDown()
        {
            GlobalConfiguration.Setup(s => s.UseOptions(new Options()));
        }

        [Test]
        public void PerformanceMeasures_FullProcess()
        {
            GlobalConfiguration.Setup(s => s.UseOptions(new Options
            {
                MaxItemsInQueue = 10,
                MaxProcessors = 30
            }));

            // start with the warmup...
            var cnt = 0;

            var pipelineId = Guid.NewGuid().ToString();

            var server = new ProcessingServer();
            server.Register(pipelineId, () =>
            {
                var pipeline = new EventPipeline();
                pipeline.AddHandler(new LoadTestOuptuHandler(e => { }));

                return pipeline;
            });
            server.Register(pipelineId, new InputHandler<InputItem>());


            var client = new EventDispatcher(server);

            var sw = new Stopwatch();
            sw.Start();
            
            for(var i = 0 ; i < 10 ; i++)
            {
                client.Process(pipelineId, new InputItem
                {
                    Value = "StaticServer",
                    Name = "test",
                    Number = i
                });
            }
            
            //sw.Stop();
            var time1 = sw.Elapsed;

            server.WaitAll(pipelineId);

            sw.Stop();
            var time2 = sw.Elapsed;

            Assert.Less(time1, TimeSpan.FromMilliseconds(30));
            Assert.Less(time2, TimeSpan.FromMilliseconds(600));
        }

        [TestCase("default", 20, 20, 1, 540)]
        [TestCase("faster", 10, 30, 1, 560)]
        [TestCase("slow", 30, 1, 1, 530)]
        [TestCase("tmp", 30, 20, 10, 520)]
        public void PerformanceMeasures_WorkersCount(string id, int maxitems, int maxprocessors, int workers, int ticks)
        {
            var server = new ProcessingServer();
            server.Register(id, () =>
            {
                var pipeline = new EventPipeline();
                pipeline.AddHandler(new LoadTestOuptuHandler(e => { }));

                return pipeline;
            });
            server.Register(id, new InputHandler<InputItem>());

            var client = new EventDispatcher(server);
            GlobalConfiguration.Setup(s => s.UseOptions(new Options
            {
                MinProcessors = workers,
                MaxItemsInQueue = maxitems,
                MaxProcessors = maxprocessors
            }));

            var result = ProfilerSession.StartSession()
                .Setup(() =>
                {
                    
                })
                .Task(() =>
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        client.Process(id, new InputItem
                        {
                            Value = "StaticServer",
                            Name = "test",
                            Number = i
                        });
                    }

                    server.WaitAll(id);
                })
                .RunSession();
            result.Trace();

            Assert.Less(result.AverageMilliseconds, ticks);
        }
    }
}
