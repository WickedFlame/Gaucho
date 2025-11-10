using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test.Diagnostics
{
    [SingleThreaded]
    public class LogEventStatisticWriterTests
    {
        [Test]
        public void LogEventStatisticWriter_ctor()
        {
            Action act = () => new LogEventStatisticWriter("pipeline");
            act.Should().NotThrow();
        }

        [Test]
        public void LogEventStatisticWriter_Write()
        {
            var config = GlobalConfiguration.Setup(s => s.Register<IStorage>(new Mock<IStorage>().Object));
            var writer = new LogEventStatisticWriter("pipeline", config);
            var log = new LogEvent { Level = LogLevel.Info };
            writer.Write(log);

            writer.Logs.Single().Should().BeSameAs(log);
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

            writer.Logs.Should().BeEmpty();
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
                    MaxLogSize =10,
                    LogShrinkSize =5
                }));

            var writer = new LogEventStatisticWriter("pipeline", config);
            for(var i =0; i <10; i++)
            {
                writer.Write(new LogEvent { Level = LogLevel.Info });
            }

            writer.Logs.Count().Should().Be(10);

            writer.Write(new LogEvent { Level = LogLevel.Info });

            // wait for the async task to complete
            writer.WaitAll();

            writer.Logs.Count().Should().Be(5);
        }

        [Test]
        public void LogEventStatisticWriter_Shrink_Storage()
        {
            var storage = new Mock<IStorage>();
            var config = GlobalConfiguration.Setup(s => s.Register<IStorage>(storage.Object)
                .UseOptions(new Options
                {
                    MaxLogSize =10,
                    LogShrinkSize =5
                }));

            var writer = new LogEventStatisticWriter("pipeline", config);
            for (var i =0; i <11; i++)
            {
                writer.Write(new LogEvent { Level = LogLevel.Info });
            }

            writer.WaitAll();

            storage.Verify(x => x.RemoveRangeFromList(It.Is<StorageKey>(k => k.ToString().EndsWith(":pipeline:logs")),6), Times.Once);
        }
    }
}
