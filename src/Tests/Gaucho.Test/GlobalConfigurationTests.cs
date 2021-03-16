using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Storage;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class GlobalConfigurationTests
	{
		[Test]
		public void GlobalConfiguration_Default_Storage()
		{
			var storage = GlobalConfiguration.Configuration.Resolve<IStorage>();
			Assert.IsAssignableFrom<InmemoryStorage>(storage);
		}

		[Test]
		public void GlobalConfiguration_Default_Options()
		{
			var options = GlobalConfiguration.Configuration.Resolve<Options>();
			Assert.IsAssignableFrom<Options>(options);
		}

		[Test]
		public void GlobalConfiguration_AddService_Func()
		{
			GlobalConfiguration.Setup(s => { });

			var dependency = new Dependency();
			var server = new ProcessingServer();
			GlobalConfiguration.Configuration
				.UseProcessingServer(p =>
				{
					var config = new PipelineConfiguration
					{
						Id = "dependency_handler",
						OutputHandlers = new List<HandlerNode>
						{
							new HandlerNode(typeof(DependencyHandler))
						},
						InputHandler = new HandlerNode("CustomInput")
					};
					p.BuildPipeline(server, config);
				})
				.AddService<IDependency>(() => dependency);

			Assert.AreSame(dependency, ((DependencyHandler)GetOutputHandlers(server, "dependency_handler").Single()).Instance);
		}

		[Test]
		public void GlobalConfiguration_AddService_ByType()
		{
			GlobalConfiguration.Setup(s => { });

			var server = new ProcessingServer();
			GlobalConfiguration.Configuration
				.UseProcessingServer(p =>
				{
					var config = new PipelineConfiguration
					{
						Id = "dependency_handler",
						OutputHandlers = new List<HandlerNode>
						{
							new HandlerNode(typeof(DependencyHandler))
						},
						InputHandler = new HandlerNode("CustomInput")
					};
					p.BuildPipeline(server, config);
				})
				.AddService<IDependency, Dependency>();

			Assert.IsNotNull(((DependencyHandler)GetOutputHandlers(server, "dependency_handler").Single()).Instance);
		}

		public interface IDependency{}
		public class Dependency: IDependency
		{

		}

		public class DependencyHandler : IOutputHandler
		{
			public DependencyHandler(IDependency dependency)
			{
				Instance = dependency;
			}

			public IDependency Instance { get; }

			public void Handle(Event @event)
			{
				throw new NotImplementedException();
			}
		}

		private IEnumerable<IOutputHandler> GetOutputHandlers(IProcessingServer server, string pipelineId)
		{
			return server.EventBusFactory.GetEventBus(pipelineId).PipelineFactory.Setup().Handlers;
		}
	}
}
