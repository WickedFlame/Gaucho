using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Gaucho.Test.Handlers
{
	public class EventDataInputHandlerTests
	{
		[Test]
		public void EventDataInputHandler_Workflow()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var logHandler = new LogQueueHandler();

			ProcessingServer.Server.SetupPipeline(pipelineId, s =>
			{
				s.Register(() =>
				{
					var pipeline = new EventPipeline();
					pipeline.AddHandler(new ConsoleOutputHandler());
					pipeline.AddHandler(logHandler);

					return pipeline;
				});

				s.Register(new EventDataInputHandler());
			});


			var portalId = "portal";


			var factory = new EventDataFactory();
			var eventData = factory.BuildFrom(new LogMessage {Message = "StaticServer_NewPipelinePerEvent2"})
				.Add(() => portalId);

			var client = new EventDispatcher();
			client.Process(pipelineId, eventData);

			ProcessingServer.Server.WaitAll(pipelineId);

			Assert.AreEqual(logHandler.Log.First(), "[Title -> ] [Message -> StaticServer_NewPipelinePerEvent2] [Level -> ] [portalId -> portal] ");
		}

		[Test]
		public void EventDataInputHandler_EventDataFactoryExtension()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var logHandler = new LogQueueHandler();

			ProcessingServer.Server.SetupPipeline(pipelineId, s =>
			{
				s.Register(() =>
				{
					var pipeline = new EventPipeline();
					pipeline.AddHandler(new ConsoleOutputHandler());
					pipeline.AddHandler(logHandler);

					return pipeline;
				});

				s.Register(new EventDataInputHandler());
			});


			var portalId = "portal"; 
			var message = new LogMessage {Message = "StaticServer_NewPipelinePerEvent2"};

			var client = new EventDispatcher();
			client.Process(pipelineId, f => f.BuildFrom(message)
				.Add(() => portalId));

			ProcessingServer.Server.WaitAll(pipelineId);

			Assert.AreEqual(logHandler.Log.First(), "[Title -> ] [Message -> StaticServer_NewPipelinePerEvent2] [Level -> ] [portalId -> portal] ");
		}
	}
}
