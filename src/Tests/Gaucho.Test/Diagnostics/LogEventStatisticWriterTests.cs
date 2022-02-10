using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test.Diagnostics
{
    [SingleThreaded]
    public class LogEventStatisticWriterTests
    {
        [Test]
        public void LogEventStatisticWriter_ctor()
        {
            Assert.DoesNotThrow(() => new LogEventStatisticWriter("pipeline"));
        }

        [Test]
        public void LogEventStatisticWriter_Write()
        {
            var config = GlobalConfiguration.Setup(s => s.Register<IStorage>(new Mock<IStorage>().Object));
            var writer = new LogEventStatisticWriter("pipeline", config);
            var log = new LogEvent { Level = LogLevel.Info };
            writer.Write(log);

            Assert.AreSame(log, writer.Logs.Single());
        }

        [Test]
        public void LogEventStatisticWriter_Write_Storage()
        {
            var storage = new Mock<IStorage>();

            var config = GlobalConfiguration.Setup(s => s.Register<IStorage>(storage.Object));
            var writer = new LogEventStatisticWriter("pipeline", config);
            var log = new LogEvent { Level = LogLevel.Info };
            writer.Write(log);

            storage.Verify(x => x.AddToList<ILogMessage>(It.Is<StorageKey>(k => k.ToString().EndsWith(":pipeline:logs")), It.Is<LogEvent>(e => e == log)), Times.Once);
        }

        [Test]
        public void LogEventStatisticWriter_Write_InvalidLogLevel()
        {
            var config = GlobalConfiguration.Setup(s => s.Register<IStorage>(new Mock<IStorage>().Object));
            var writer = new LogEventStatisticWriter("pipeline", config);
            var log = new LogEvent();
            writer.Write(log);

            Assert.IsEmpty(writer.Logs);
        }

        [Test]
        public void LogEventStatisticWriter_Write_Storage_InvalidLogLevel()
        {
            var storage = new Mock<IStorage>();

            var config = GlobalConfiguration.Setup(s => s.Register<IStorage>(storage.Object));
            var writer = new LogEventStatisticWriter("pipeline", config);
            var log = new LogEvent();
            writer.Write(log);

            storage.Verify(x => x.AddToList(It.IsAny<StorageKey>(), It.IsAny<LogEvent>()), Times.Never);
        }

        [Test]
        public void LogEventStatisticWriter_Shrink()
        {
            var config = GlobalConfiguration.Setup(s => s.Register<IStorage>(new Mock<IStorage>().Object)
                .UseOptions(new Options
                {
                    MaxLogSize = 10,
                    LogShrinkSize = 5
                }));

            var writer = new LogEventStatisticWriter("pipeline", config);
            for(var i = 0; i < 10; i++)
            {
                writer.Write(new LogEvent { Level = LogLevel.Info });
            }

            Assert.AreEqual(10, writer.Logs.Count());

            writer.Write(new LogEvent { Level = LogLevel.Info });

            Assert.AreEqual(5, writer.Logs.Count());
        }

        [Test]
        public void LogEventStatisticWriter_Shrink_Storage()
        {
            var storage = new Mock<IStorage>();
            var config = GlobalConfiguration.Setup(s => s.Register<IStorage>(storage.Object)
                .UseOptions(new Options
                {
                    MaxLogSize = 10,
                    LogShrinkSize = 5
                }));

            var writer = new LogEventStatisticWriter("pipeline", config);
            for (var i = 0; i < 11; i++)
            {
                writer.Write(new LogEvent { Level = LogLevel.Info });
            }

            storage.Verify(x => x.RemoveRangeFromList(It.Is<StorageKey>(k => k.ToString().EndsWith(":pipeline:logs")), 6), Times.Once);
        }
    }
}
