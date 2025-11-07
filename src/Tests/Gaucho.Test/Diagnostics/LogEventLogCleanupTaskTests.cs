using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.BackgroundTasks;
using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test.Diagnostics
{
    public class LogEventLogCleanupTaskTests
    {
        [Test]
        public void LogEventLogCleanupTask_ctor()
        {
            Action act = () => new LogEventLogCleanupTask(new List<ILogMessage>(), new StorageKey("test"), 1);
            act.Should().NotThrow();
        }

        [Test]
        public void LogEventLogCleanupTask_Execute_Unlock()
        {
            var locker = new DispatcherLock();
            locker.Lock();
            var task = new LogEventLogCleanupTask(new List<ILogMessage>(), new StorageKey("test"), 0);
            task.Execute(new StorageContext(new Mock<IStorage>().Object, locker));

            locker.IsLocked().Should().BeFalse();
        }

        [Test]
        public void LogEventLogCleanupTask_Execute_ShrinkSizeTooLarge()
        {
            var locker = new DispatcherLock();
            locker.Lock();
            var task = new LogEventLogCleanupTask(new List<ILogMessage>(), new StorageKey("test"), 100);
            task.Execute(new StorageContext(new Mock<IStorage>().Object, locker));

            locker.IsLocked().Should().BeFalse();
        }

        [Test]
        public void LogEventLogCleanupTask_Execute_ShrinkQueue()
        {
            var lst = new List<ILogMessage>();
            for (var i = 0; i < 25; i++)
            {
                lst.Add(new LogEvent());
            }
            var storage = new Mock<IStorage>();

            var task = new LogEventLogCleanupTask(lst, new StorageKey("test"), 20);
            task.Execute(new StorageContext(storage.Object, new DispatcherLock()));

            lst.Count.Should().Be(5);
        }

        [Test]
        public void LogEventLogCleanupTask_Execute_ShrinkStore()
        {
            var lst = new List<ILogMessage>();
            for (var i = 0; i < 25; i++)
            {
                lst.Add(new LogEvent());
            }
            var storage = new Mock<IStorage>();

            var task = new LogEventLogCleanupTask(lst, new StorageKey("test"), 20);
            task.Execute(new StorageContext(storage.Object, new DispatcherLock()));

            storage.Verify(x => x.RemoveRangeFromList(It.IsAny<StorageKey>(), 20), Times.Once);
        }
    }
}
