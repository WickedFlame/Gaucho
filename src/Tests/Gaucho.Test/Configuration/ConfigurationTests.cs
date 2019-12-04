using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gaucho.Configuration;
using NUnit.Framework;

namespace Gaucho.Test.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        private PluginManager _pluginMgr;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _pluginMgr = new PluginManager();
        }
        
        [Test]
        public void ConfigurationTests_Configuration()
        {
            var config = new PipelineConfiguration
            {
                InputHandler = new HandlerNode("CustomInput"),
                OutputHandlers = new List<HandlerNode>
                {
                    new HandlerNode("ConsoleOutput")
                }
            };

            var pipelineId = Guid.NewGuid().ToString();

            ProcessingServer.SetupPipeline(pipelineId, config);

            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent2" });
        }

        [Test]
        public void ConfigurationTests_Configuration_HandlerPerType()
        {
            var config = new PipelineConfiguration
            {
                InputHandler = new HandlerNode(typeof(GenericInputHandler<LogMessage>)),
                OutputHandlers = new List<HandlerNode>
                {
                    new HandlerNode("ConsoleOutput")
                }
            };

            var pipelineId = Guid.NewGuid().ToString();

            ProcessingServer.SetupPipeline(pipelineId, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    foreach (var handler in _pluginMgr.GetOutputHandlers(config))
                    {
                        pipeline.AddHandler(handler);
                    }

                    return pipeline;
                });

                s.Register(_pluginMgr.GetInputHandler(config));
            });

            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent2" });
        }

        [Test]
        public void ConfigurationTests_Configuration_Filters()
        {
            var config = new PipelineConfiguration
            {
                InputHandler = new HandlerNode(typeof(GenericInputHandler<LogMessage>)),
                OutputHandlers = new List<HandlerNode>
                {
                    new HandlerNode("ConsoleOutput")
                    {
                        Filters = new List<string>
                        {
                            "Message -> msg"
                        }
                    }
                }
            };

            var pipelineId = Guid.NewGuid().ToString();

            ProcessingServer.SetupPipeline(pipelineId, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    foreach (var handler in _pluginMgr.GetOutputHandlers(config))
                    {
                        pipeline.AddHandler(handler);
                    }

                    return pipeline;
                });

                s.Register(_pluginMgr.GetInputHandler(config));
            });

            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent2" });

            ProcessingServer.Server.WaitAll(pipelineId);
        }

    }
}
