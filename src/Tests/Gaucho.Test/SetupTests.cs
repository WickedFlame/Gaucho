using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Handlers;
using NUnit.Framework;
using AwesomeAssertions;

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

			GetOutputHandlers(server, pipelineId).Single().Should().BeSameAs(handler);
		}

		[Test]
		public void Setup_Register_Factory_Register_InputHandler()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var handler = new SetupInputHandler();


			var server = new ProcessingServer();
			server.Register(pipelineId, handler);
			
			server.InputHandlers.Single(h => h.PipelineId == pipelineId).Should().BeSameAs(handler);
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

			GetOutputHandlers(server, pipelineId).Single().Should().BeSameAs(handler);
		}

		[Test]
		public void Setup_Register_Factory_SetupPipeline_FilterDecorator()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var handler = new SetupOutputHandler();

			var server = new ProcessingServer();
			server.SetupPipeline(pipelineId, c =>
			{
				c.Register(() =>
				{
					var pipeline = new EventPipeline();
					pipeline.AddHandler(handler, new[] { "Level -> dst_lvl", "Message" });

					return pipeline;
				});
			});

			GetOutputHandlers(server, pipelineId).Single().Should().BeAssignableTo<DataFilterDecorator>();
		}

		[Test]
		public void Setup_Register_Factory_SetupPipeline_AddFilters()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var handler = new SetupOutputHandler();

			var server = new ProcessingServer();
			server.SetupPipeline(pipelineId, c =>
			{
				c.Register(() =>
				{
					var pipeline = new EventPipeline();
					pipeline.AddHandler(handler, new[] {"Level -> dst_lvl", "Message"});

					return pipeline;
				});
			});

			((DataFilterDecorator)GetOutputHandlers(server, pipelineId).Single()).Converter.Filters.Count().Should().Be(2);
		}

		[Test]
		public void Setup_Register_Factory_SetupPipeline_AddFilters_Convert()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var handler = new SetupOutputHandler();

			var server = new ProcessingServer();
			server.SetupPipeline(pipelineId, c =>
			{
				c.Register(() =>
				{
					var pipeline = new EventPipeline();
					pipeline.AddHandler(handler, new[] { "Level -> dst_lvl", "Message" });

					return pipeline;
				});
			});

			((DataFilterDecorator)GetOutputHandlers(server, pipelineId).Single()).Converter.Filters.Count().Should().Be(2);
		}


		[Test]
		public void Setup_Register_Factory_SetupPipeline_AddFilters_VerifyHandlers()
		{
			var pipelineId = Guid.NewGuid().ToString();

			var handler = new SetupOutputHandler();

			var server = new ProcessingServer();
			server.SetupPipeline(pipelineId, c =>
			{
				c.Register(() =>
				{
					var pipeline = new EventPipeline();
					pipeline.AddHandler(handler, new[] { "Level -> dst_lvl", "Message" });

					return pipeline;
				});
			});

			GetOutputHandlers(server, pipelineId).Single().Should().NotBeSameAs(handler);
			((DataFilterDecorator) GetOutputHandlers(server, pipelineId).Single()).InnerHandler.Should().BeSameAs(handler);
		}

		private IEnumerable<IOutputHandler> GetOutputHandlers(IProcessingServer server, string pipelineId)
		{
			return server.EventBusFactory.GetEventBus(pipelineId).PipelineFactory.Setup().Handlers;
		}

		public class SetupOutputHandler : IOutputHandler
		{
			public void Handle(Event @event)
			{
				throw new NotImplementedException();
			}
		}

		public class SetupInputHandler : IInputHandler //<SetupMessage>
		{
			public string PipelineId { get; set; }
		}
	}
}
