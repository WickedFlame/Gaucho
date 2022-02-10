using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Gaucho.Configuration;
using Gaucho.Handlers;
using Gaucho.Server.Monitoring;
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

        [TestCase("default", 20, 20, 1, 560)]
        [TestCase("faster", 10, 30, 1, 560)]
        [TestCase("slow", 30, 1, 1, 530)]
        [TestCase("tmp", 30, 20, 10, 530)]
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
                    client.Process(id, new InputItem
                    {
                        Value = "StaticServer",
                        Name = "test",
                        Number = 0
                    });
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

        [Test]
        public void PerformanceMeasures_Measure_ProcessTime()
        {
            var times = new List<PerfItem>();

            var pipelineId = Guid.NewGuid().ToString();

            var server = new ProcessingServer();
            server.Register(pipelineId, () =>
            {
                var pipeline = new EventPipeline();
                pipeline.AddHandler(new LoadTestOuptuHandler(e =>
                {
                    var start = (DateTime)((EventData)e.Data)["Start"];
                    var time = DateTime.Now.Subtract(start);
                    var itm = new PerfItem
                    {
                        Id = (int)((EventData)e.Data)["Id"],
                        Start = start,
                        Duration = time
                    };
                    times.Add(itm);
                    // do some heavy work
                    Trace.WriteLine($"[{itm.Id}] {time}");
                }));

                return pipeline;
            }, new PipelineOptions { MinProcessors = 10 });
            server.Register(pipelineId, new InputHandler<PerfItem>());



            var client = new EventDispatcher(server);

            var i = 1;
            var profiler = ProfilerSession.StartSession()
                .SetIterations(100)
                .Task(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"[{i}] Start task");
                    client.Process(pipelineId, new PerfItem
                    {
                        Id = i,
                        Start = DateTime.Now
                    });
                    i++;
                }).RunSession();

            server.WaitAll(pipelineId);
            profiler.Trace();


            foreach (var itm in times.OrderBy(t => t.Duration))
            {
                Debug.WriteLine($"[{itm.Id}] {itm.Duration}");
            }
        }

        public class PerfItem
        {
            public DateTime Start { get; set; }
            public int Id { get; set; }
            public TimeSpan Duration { get; set; }
        }
    }
}
