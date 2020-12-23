using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class SetupTests
	{
		[Test]
		public void Setup_Register_Factory_Register_OutputHandler()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var handler = new SetupOutputHandler();


			var server = new ProcessingServer();
			server.Register(pipelineId, () =>
			{
				var pipeline = new EventPipeline();
				pipeline.AddHandler(handler);

				return pipeline;
			});

			Assert.AreSame(handler, server.EventBusFactory.GetEventBus(pipelineId).PipelineFactory.Setup().Handlers.Single());
		}

		[Test]
		public void Setup_Register_Factory_Register_InputHandler()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var handler = new SetupInputHandler();


			var server = new ProcessingServer();
			server.Register(pipelineId, handler);
			
			Assert.AreSame(handler, server.InputHandlers.Single(h => h.PipelineId == pipelineId));
		}

		[Test]
		public void Setup_Register_Factory_SetupPipeline()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var handler = new SetupOutputHandler();

			var server = new ProcessingServer();
			server.SetupPipeline(pipelineId, c =>
			{
				c.Register(() =>
				{
					var pipeline = new EventPipeline();
					pipeline.AddHandler(handler);

					return pipeline;
				});
			});

			Assert.AreSame(handler, server.EventBusFactory.GetEventBus(pipelineId).PipelineFactory.Setup().Handlers.Single());
		}

		//[Test]
		//public void Setup_Register_Factory_SetupPipeline_AddFilters()
		//{
		//	var pipelineId = Guid.NewGuid().ToString();

		//	var handler = new SetupOutputHandler();

		//	var server = new ProcessingServer();
		//	server.SetupPipeline(pipelineId, c =>
		//	{
		//		c.Register(() =>
		//		{
		//			var pipeline = new EventPipeline();
		//			pipeline.AddHandler(handler);

		//			return pipeline;
		//		});
		//	});

		//	Assert.AreSame(handler, server.EventBusFactory.GetEventBus(pipelineId).PipelineFactory.Setup().Handlers.Single());
		//}

		public class SetupOutputHandler : IOutputHandler
		{
			public void Handle(Event @event)
			{
				throw new NotImplementedException();
			}
		}

		public class SetupInputHandler : IInputHandler
		{
			public string PipelineId { get; set; }
		}
	}
}
