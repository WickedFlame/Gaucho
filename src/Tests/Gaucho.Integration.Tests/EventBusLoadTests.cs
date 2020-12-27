﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MeasureMap;
using Gaucho.Configuration;
using Gaucho.Integration.Tests.Handlers;
using Gaucho.Server;
using NUnit.Framework;

namespace Gaucho.Test.LoadTests
{
    [Explicit]
	//[Ignore("")]
	[Category("Loadtesting")]
	public class EventBusLoadTests
    {
        [Test]
        public void LoadTesting()
        {
            var pipelineId = Guid.NewGuid().ToString();
            var config = new PipelineConfiguration
            {
                Id = pipelineId,
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
                    new HandlerNode(typeof(ThreadWaitHandler))
                }
            };

            var builder = new PipelineBuilder();
            builder.BuildPipeline(config);

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

        public class ThreadWaitHandler : IOutputHandler
        {
	        public void Handle(Event @event)
	        {
		        Thread.Sleep(5000);
	        }
        }
	}
}