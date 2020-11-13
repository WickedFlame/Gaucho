using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Server;
using MeasureMap;
using NUnit.Framework;

namespace Gaucho.Test.LoadTests
{
	[Explicit]
	[Category("Performancetesting")]
	public class PipelineBuilderPerformanceTests
	{
		[Test]
		public void PipelineBuilder_Resolver_Perfomance()
		{
			var server = new ProcessingServer();
			var builder = new PipelineBuilder();

			builder.BuildPipeline(server, new PipelineConfiguration
			{
				Id = "1",
				InputHandler = new HandlerNode(typeof(PlainInputHandler)),
				OutputHandlers = new List<HandlerNode>
				{
					new HandlerNode(typeof(PlainOutputHandler))
				}
			});

			builder.BuildPipeline(server, new PipelineConfiguration
			{
				Id = "2",
				InputHandler = new HandlerNode(typeof(CustomInputHandler)),
				OutputHandlers = new List<HandlerNode>
				{
					new HandlerNode(typeof(PlainOutputHandler))
				}
			});

			//server.EventBusFactory.GetEventBus("1").Publish(new Event("1", new EventData()));
			//server.EventBusFactory.GetEventBus("2").Publish(new Event("2", new EventData()));

			var runner = new BenchmarkRunner();
			runner.SetIterations(100);

			runner.AddSession("Simple resolver",
				ProfilerSession.StartSession()
					.Task(() => server.Publish(new Event("1", new EventData())))
					.SetThreads(5)
			);
			runner.AddSession("Complex resolver",
				ProfilerSession.StartSession()
					.Task(() => server.Publish(new Event("2", new EventData())))
					.SetThreads(5)
			);

			var result = runner.RunSessions();
			result.Trace();
		}

		public class PlainInputHandler : IInputHandler
		{
			public string PipelineId { get; set; }
		}

		public class CustomInputHandler : IInputHandler
		{
			public CustomInputHandler(IEventDataConverter converter, ConfiguredArguments arguments){}
			public string PipelineId { get; set; }
		}

		public class PlainOutputHandler : IOutputHandler
		{
			public void Handle(Event @event)
			{
				
			}
		}

		public class CustomOutputHandler : IOutputHandler
		{
			public CustomOutputHandler(IEventDataConverter converter, ConfiguredArguments arguments) { }
			public void Handle(Event @event)
			{
				
			}
		}
	}
}
