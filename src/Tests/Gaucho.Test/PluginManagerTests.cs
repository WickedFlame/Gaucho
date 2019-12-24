using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gaucho.Configuration;
using Gaucho.Filters;
using Gaucho.Server;
using NUnit.Framework;

namespace Gaucho.Test
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
            var obj = plugin.Type.CreateInstance<IInputHandler>();

            Assert.That(obj is LogInputHandler);
        }

        [Test]
        public void PluginManagerTests_GetPlugin()
        {
            var plugin = _pluginMgr.GetPlugin(typeof(IInputHandler), "LogInput");
            var obj = plugin.Type.CreateInstance<IInputHandler>();

            Assert.That(obj is LogInputHandler);
        }

        [Test]
        public void PluginManagerTests_BuildConfig()
        {
            var inputPlugin = _pluginMgr.GetPlugin(typeof(IInputHandler), "CustomInput");
            var input = inputPlugin.Type.CreateInstance<IInputHandler>();

            var outputPlugin = _pluginMgr.GetPlugin(typeof(IOutputHandler), "ConsoleOutput");
            var output = outputPlugin.Type.CreateInstance<IOutputHandler>();

            var pipelineId = Guid.NewGuid().ToString();

            var loghandler = new LogQueueHandler();

            var cnt = 0;

            var server = new ProcessingServer();
            server.SetupPipeline(pipelineId, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    pipeline.AddHandler(output);
                    pipeline.AddHandler(loghandler);

                    cnt = cnt + 1;

                    return pipeline;
                });

                s.Register(input);
            });


            var client = new EventDispatcher(server);
            client.Process(pipelineId, new LogMessage { Message = "Event1" });
            client.Process(pipelineId, new LogMessage { Message = "Event2" });

            server.WaitAll(pipelineId);

            //Assert.That(cnt == 1, () => $"Count is {cnt} but is expected to be 1");
            Assert.That(loghandler.Log.Count() == 2, () => $"Logcount is {loghandler.Log.Count()} but is expected to be 2");
        }

        [Test]
        public void PluginManagerTests_InvalidPlugin()
        {
            var mgr = new PluginManager();
            var plugin = mgr.GetPlugin(typeof(IInputHandler), new HandlerNode());

            Assert.IsNull(plugin);
        }

        //[Test]
        //public void PluginManagerTests_ReadConfigFromFile()
        //{
        //    var reader = new YamlReader();
        //    var config = reader.Read("Config1.yml");

        //    var pipelineId = Guid.NewGuid().ToString();

        //    var server = new ProcessingServer();

        //    ProcessingServer.SetupPipeline(pipelineId, server, s =>
        //    {
        //        s.Register(() =>
        //        {
        //            var pipeline = new EventPipeline();
        //            foreach (var handler in _pluginMgr.GetOutputHandlers(config))
        //            {
        //                pipeline.AddHandler(handler);
        //            }

        //            return pipeline;
        //        });

        //        s.Register(_pluginMgr.GetInputHandler(config));
        //    });

        //    var bus = server.EventBusFactory.GetEventBus(pipelineId);
        //    var p = bus.PipelineSetup.Setup();

        //    var inputhandler = server.GetHandler<LogMessage>(pipelineId);
        //    Assert.IsNotNull(inputhandler);
        //    Assert.That(inputhandler.Converter.Filters.Any());

        //    Assert.That(p.Handlers.Count() == 2);
        //    Assert.That(p.Handlers.All(h => h.Converter.Filters.Any()));
        //}
    }
}
