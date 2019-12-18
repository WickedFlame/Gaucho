using System;
using System.Collections.Generic;
using System.Text;
using MeasureMap;
using Gaucho.Configuration;
using NUnit.Framework;

namespace Gaucho.Test.LoadTests
{
    [TestFixture]
    [Explicit]
    [Ignore("Loadtesting")]
    public class EventBusLoadTests
    {
        [Test]
        public void LoadTesting()
        {
            var config = new PipelineConfiguration
            {
                InputHandler = new HandlerNode("CustomInput"),
                OutputHandlers = new List<HandlerNode>
                {
                    new HandlerNode("ConsoleOutput")
                    {
                        Filters = new List<string>
                        {
                            "Message -> msg",
                            "Level -> lvl"
                        }
                    },
                    new HandlerNode(typeof(ApiTests.ThreadWaitHandler))
                }
            };

            var pipelineId = Guid.NewGuid().ToString();

            ProcessingServer.SetupPipeline(pipelineId, config);

            var result = ProfilerSession.StartSession()
                .Task(ctx =>
                {
                    var client = new EventDispatcher();
                    client.Process(pipelineId, new LogMessage {Level = "Info", Message = $"Loadtesting {ctx.Get<int>(ContextKeys.Iteration)} on Thread {ctx.Get<int>(ContextKeys.ThreadId)}", Title = "Loadtest" });
                })
                .SetIterations(100)
                .SetThreads(10)
                .RunSession();

            ProcessingServer.Server.WaitAll(pipelineId);

            result.Trace("### LoadTesting");
        }
    }
}
