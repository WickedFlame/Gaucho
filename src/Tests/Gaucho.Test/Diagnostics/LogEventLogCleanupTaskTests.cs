using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.BackgroundTasks;
using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test.Diagnostics
{
    public class LogEventLogCleanupTaskTests
    {
        [Test]
        public void LogEventLogCleanupTask_ctor()
        {
            Assert.DoesNotThrow(() => new LogEventLogCleanupTask(new List<ILogMessage>(), new StorageKey("test"), 1));
        }

        [Test]
        public void LogEventLogCleanupTask_Execute_Unlock()
        {
            var locker = new DispatcherLock();
            locker.Lock();
            var task = new LogEventLogCleanupTask(new List<ILogMessage>(), new StorageKey("test"), 0);
            task.Execute(new StorageContext(new Mock<IStorage>().Object, locker));

            Assert.IsFalse(locker.IsLocked());
        }

        [Test]
        public void LogEventLogCleanupTask_Execute_ShrinkSizeTooLarge()
        {
            var locker = new DispatcherLock();
            locker.Lock();
            var task = new LogEventLogCleanupTask(new List<ILogMessage>(), new StorageKey("test"), 100);
            task.Execute(new StorageContext(new Mock<IStorage>().Object, locker));

            Assert.IsFalse(locker.IsLocked());
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

            Assert.AreEqual(5, lst.Count);
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
            //storage.Setup(x => x.GetList<ActiveWorkersLogMessage>(It.IsAny<StorageKey>())).Returns(() => lst);

            var task = new LogEventLogCleanupTask(lst, new StorageKey("test"), 20);
            task.Execute(new StorageContext(storage.Object, new DispatcherLock()));

            storage.Verify(x => x.RemoveRangeFromList(It.IsAny<StorageKey>(), 20), Times.Once);
        }
    }
}
