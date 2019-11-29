using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Configuration;
using NUnit.Framework;

namespace Gaucho.Test.Configuration
{
    [TestFixture]
    public class ConfigurationReaderTests
    {
        [Test]
        public void ConfigurationReader_ReadFromFile()
        {
            var reader = new ConfigurationReader();
            var config = reader.Read("Config1.cnf");

            Assert.That(config.Id == "pipeline1");

            config = reader.Read("Config2.cnf");

            Assert.That(config.Id == "pipeline2");
        }

        [Test]
        public void ConfigurationReader_ReadFromFile_InputHandlerByType()
        {
            var reader = new ConfigurationReader();
            var config = reader.Read("Config1.cnf");

            Assert.That(config.InputHandler.Type == typeof(GenericInputHandler<LogMessage>));
            Assert.That(config.InputHandler.Name == null);
        }

        [Test]
        public void ConfigurationReader_ReadFromFile_InputHandlerByName()
        {
            var reader = new ConfigurationReader();
            var config = reader.Read("Config2.cnf");

            Assert.That(config.InputHandler.Type == null);
            Assert.That(config.InputHandler.Name == "GenericLogMessage");
        }

        [Test]
        public void ConfigurationReader_ReadFromFile_OutputHandlerByType()
        {
            var reader = new ConfigurationReader();
            var config = reader.Read("Config1.cnf");

            Assert.That(config.OutputHandlers.First().Type == typeof(ConsoleOutputHandler));
            Assert.That(config.OutputHandlers.First().Name == null);

            Assert.That(config.OutputHandlers.Last().Type == typeof(LogQueueHandler));
            Assert.That(config.OutputHandlers.Last().Name == null);
        }

        [Test]
        public void ConfigurationReader_ReadFromFile_OutputHandlerByName()
        {
            var reader = new ConfigurationReader();
            var config = reader.Read("Config2.cnf");

            Assert.That(config.OutputHandlers.First().Type == null);
            Assert.That(config.OutputHandlers.First().Name == "ConsoleOutput");

            Assert.That(config.OutputHandlers.Last().Type == null);
            Assert.That(config.OutputHandlers.Last().Name == "LogOutput");
        }

        [Test]
        public void ConfigurationReader_ReadFromFile_OutputHandlers_Filters()
        {
            var reader = new ConfigurationReader();
            var config = reader.Read("Config1.cnf");

            Assert.That(config.OutputHandlers.First().Filters.Count() == 1);
            Assert.That(config.OutputHandlers.First().Filters[0].Source == "Message");
            Assert.That(config.OutputHandlers.First().Filters[0].Destination == "msg");

            Assert.That(config.OutputHandlers.Last().Filters.Count() == 2);
            Assert.That(config.OutputHandlers.Last().Filters[0].Source == "Message");
            Assert.That(config.OutputHandlers.Last().Filters[0].Destination == "msg");
            Assert.That(config.OutputHandlers.Last().Filters[1].Source == "Level");
            Assert.That(config.OutputHandlers.Last().Filters[1].Destination == "lvl");
        }

        [Test]
        public void ConfigurationReader_ReadFromFile_InputHandlers_Filters()
        {
            var reader = new ConfigurationReader();
            var config = reader.Read("Config1.cnf");

            Assert.That(config.InputHandler.Filters.Count() == 1);
            Assert.That(config.InputHandler.Filters[0].Source == "Message");
            Assert.That(config.InputHandler.Filters[0].Destination == "msg");
        }
    }
}
