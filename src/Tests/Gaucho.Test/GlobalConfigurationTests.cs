using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Handlers;
using Gaucho.Server;
using Gaucho.Storage;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class GlobalConfigurationTests
	{
		[TearDown]
		public void Teardown()
		{
			GlobalConfiguration.Setup(s => { });
		}

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
		public void GlobalConfiguration_UseProcessingServer_BuildPipeline_OutputHandlers()
		{
			var server = new ProcessingServer();
			GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
				{
					var config = new PipelineConfiguration
					{
						Id = "pipeline1",
						OutputHandlers = new List<HandlerNode>
						{
							new HandlerNode(typeof(ConsoleOutputHandler))
						},
						InputHandler = new HandlerNode(typeof(CustomInputHandler))
					};
					p.BuildPipeline(server, config);
				}));

			var handlers = GetOutputHandlers(server, "pipeline1");

			Assert.IsInstanceOf<ConsoleOutputHandler>(handlers.Single());
		}

		[Test]
		public void GlobalConfiguration_UseProcessingServer_BuildPipeline_InputHandlers()
		{
			var server = new ProcessingServer();
			GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
			{
				var config = new PipelineConfiguration
				{
					Id = "pipeline1",
					OutputHandlers = new List<HandlerNode>
					{
						new HandlerNode(typeof(ConsoleOutputHandler))
					},
					InputHandler = new HandlerNode(typeof(CustomInputHandler))
				};
				p.BuildPipeline(server, config);
			}));

			var handler = server.InputHandlers.Single();

			Assert.IsInstanceOf<CustomInputHandler>(handler);
			Assert.AreEqual("pipeline1", handler.PipelineId);
		}

		[Test]
		public void GlobalConfiguration_UseProcessingServer_BuildPipeline_Filters()
		{
			var server = new ProcessingServer();
			GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
			{
				var config = new PipelineConfiguration
				{
					Id = "pipeline1",
					OutputHandlers = new List<HandlerNode>
					{
						new HandlerNode(typeof(ConsoleOutputHandler))
						{
							Filters = new List<string>
							{
								"Level -> lvl",
								"Message",
								"Id <- ${lvl}_error_${Message}"
							}
						}
					},
					
					InputHandler = new HandlerNode(typeof(CustomInputHandler))
				};
				p.BuildPipeline(server, config);
			}));

			var handler = GetOutputHandlers(server, "pipeline1").Single();
			Assert.IsAssignableFrom<DataFilterDecorator>(handler);

			var converter = ((DataFilterDecorator) handler).Converter;
			Assert.That(converter.Filters.Any(f => f.Key == "Level" && f.FilterType == Gaucho.Filters.FilterType.Property));
			Assert.That(converter.Filters.Any(f => f.Key == "Message" && f.FilterType == Gaucho.Filters.FilterType.Property));
			Assert.That(converter.Filters.Any(f => f.Key == "Id" && f.FilterType == Gaucho.Filters.FilterType.Formatter));
		}

		[Test]
		public void GlobalConfiguration_AddService_Func()
		{
			GlobalConfiguration.Setup(s => { });

			var dependency = new Dependency();
			var server = new ProcessingServer();
			GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
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
				}))
				.AddService<IDependency>(() => dependency);

			Assert.AreSame(dependency, ((DependencyHandler)GetOutputHandlers(server, "dependency_handler").Single()).Instance);
		}

		[Test]
		public void GlobalConfiguration_AddService_Func_InSetup()
		{
			GlobalConfiguration.Setup(s => { });

			var dependency = new Dependency();
			var server = new ProcessingServer();
			GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
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
				}).AddService<IDependency>(() => dependency)
			);

			Assert.AreSame(dependency, ((DependencyHandler)GetOutputHandlers(server, "dependency_handler").Single()).Instance);
		}

		[Test]
		public void GlobalConfiguration_AddService_ByType()
		{
			GlobalConfiguration.Setup(s => { });

			var server = new ProcessingServer();
			GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
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
			).AddService<IDependency, Dependency>();

			Assert.IsNotNull(((DependencyHandler)GetOutputHandlers(server, "dependency_handler").Single()).Instance);
		}

		[Test]
		public void GlobalConfiguration_AddService_ByType_InSetup()
		{
			GlobalConfiguration.Setup(s => { });

			var server = new ProcessingServer();
			GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
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
				}).AddService<IDependency, Dependency>()
			);

			Assert.IsNotNull(((DependencyHandler)GetOutputHandlers(server, "dependency_handler").Single()).Instance);
		}

		[Test]
		public void GlobalConfiguration_Override_ActivationContext()
		{
			GlobalConfiguration.Setup(s => { });
			var context = new CustomActivationContext();
			GlobalConfiguration.Configuration
				.Register<IActivationContext>(context);

			Assert.AreSame(context, GlobalConfiguration.Configuration.Resolve<IActivationContext>());
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

		public class CustomActivationContext : IActivationContext
		{
			public void Register<TService, TImpl>() where TImpl : TService
			{
				throw new NotImplementedException();
			}

			public void Register<TService>(Func<TService> instanceCreator)
			{
				throw new NotImplementedException();
			}

			public T Resolve<T>()
			{
				throw new NotImplementedException();
			}

			public T Resolve<T>(Type serviceType)
			{
				throw new NotImplementedException();
			}

			public object Resolve(Type serviceType)
			{
				throw new NotImplementedException();
			}

			public IActivationContext ChildContext()
			{
				throw new NotImplementedException();
			}
		}
	}
}
