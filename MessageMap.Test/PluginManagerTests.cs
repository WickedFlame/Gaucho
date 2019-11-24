using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MessageMap.Configuration;
using NUnit.Framework;

namespace MessageMap.Test
{
    [TestFixture]
    public class PluginManagerTests
    {
        private PluginManager _pluginMgr;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _pluginMgr = new PluginManager();
        }

        [Test]
        public void PluginManagerTests_Simple()
        {
            var plugins = _pluginMgr.GetPlugins(typeof(IInputHandler));

            Assert.That(plugins.Count() > 2);
        }

        [Test]
        public void PluginManagerTests_CreateInstance()
        {
            var plugins = _pluginMgr.GetPlugins(typeof(IInputHandler));

            var plugin = plugins.First(p => p.Name == "LogInput");
            var obj = plugin.CreateInstance<IInputHandler>();

            Assert.That(obj is LogInputHandler);
        }

        [Test]
        public void PluginManagerTests_GetPlugin()
        {
            var plugin = _pluginMgr.GetPlugin(typeof(IInputHandler), "LogInput");
            var obj = plugin.CreateInstance<IInputHandler>();

            Assert.That(obj is LogInputHandler);
        }

        [Test]
        public void PluginManagerTests_BuildConfig()
        {
            var inputPlugin = _pluginMgr.GetPlugin(typeof(IInputHandler), "CustomInput");
            var input = inputPlugin.CreateInstance<IInputHandler>();

            var outputPlugin = _pluginMgr.GetPlugin(typeof(IOutputHandler), "ConsoleOutput");
            var output = outputPlugin.CreateInstance<IOutputHandler>();

            var pipelineId = Guid.NewGuid().ToString();

            var cnt = 0;

            ProcessingServer.SetupPipeline(pipelineId, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    pipeline.AddHandler(output);
                    cnt = cnt + 1;

                    return pipeline;
                });

                s.Register(input);
            });


            var client = new Client();
            client.Process(pipelineId, new LogMessage { Message = "PluginManagerTests_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "PluginManagerTests_NewPipelinePerEvent2" });

            Assert.That(cnt == 0);

            ProcessingServer.Server.WaitAll(pipelineId);

            Assert.That(cnt == 1);
        }

        [Test]
        public void PluginManagerTests_ReadConfigFromFile()
        {
            var reader = new ConfigurationReader();
            var config = reader.Read("Config1.cnf");

            var pipelineId = Guid.NewGuid().ToString();

            var server = new ProcessingServer();
            
            ProcessingServer.SetupPipeline(pipelineId, server, s =>
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

                s.Register(_pluginMgr.GetHandler(config));
            });

            var bus = server.EventBusFactory.GetEventBus(pipelineId);
            var pipeline = bus.PipelineSetup.Setup();

            Assert.IsNotNull(server.GetHandler<LogMessage>(pipelineId));
            Assert.That(pipeline.Handlers.Count() == 2);
            Assert.That(pipeline.Handlers.All(h => h.Converter.Filters.Any()));
        }
    }
}
