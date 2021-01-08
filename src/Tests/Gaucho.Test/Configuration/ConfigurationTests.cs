using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gaucho.Configuration;
using Gaucho.Server;
using NUnit.Framework;

namespace Gaucho.Test.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        private HandlerPluginManager _pluginMgr;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _pluginMgr = new HandlerPluginManager();
        }
        
        [Test]
        public void ConfigurationTests_Configuration()
        {
            var pipelineId = Guid.NewGuid().ToString();
            var config = new PipelineConfiguration
            {
                Id = pipelineId,
                InputHandler = new HandlerNode("CustomInput"),
                OutputHandlers = new List<HandlerNode>
                {
                    new HandlerNode("ConsoleOutput")
                }
            };


            var builder = new PipelineBuilder();
            builder.BuildPipeline(config);

            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent2" });
        }

        [Test]
        public void ConfigurationTests_Configuration_HandlerPerType()
        {
            var pipelineId = Guid.NewGuid().ToString();
            var config = new PipelineConfiguration
            {
                Id = pipelineId,
                InputHandler = new HandlerNode(typeof(GenericInputHandler<LogMessage>)),
                OutputHandlers = new List<HandlerNode>
                {
                    new HandlerNode("ConsoleOutput")
                }
            };

            var builder = new PipelineBuilder();
            builder.BuildPipeline(ProcessingServer.Server, config);
            
            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent2" });
        }

        [Test]
        public void ConfigurationTests_Configuration_Filters()
        {
            var pipelineId = Guid.NewGuid().ToString();
            var config = new PipelineConfiguration
            {
                Id = pipelineId,
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


            var builder = new PipelineBuilder();
            builder.BuildPipeline(ProcessingServer.Server, config);

            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "ConfigurationTests_NewPipelinePerEvent2" });

            ProcessingServer.Server.WaitAll(pipelineId);
        }

    }
}
