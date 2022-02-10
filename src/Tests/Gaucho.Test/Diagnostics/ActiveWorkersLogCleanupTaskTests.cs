using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.BackgroundTasks;
using Gaucho.Diagnostics.MetricCounters;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test.Diagnostics
{
    public class ActiveWorkersLogCleanupTaskTests
    {
        [Test]
        public void ActiveWorkersLogCleanupTask_ctor()
        {
            Assert.DoesNotThrow(() => new ActiveWorkersLogCleanupTask(new DispatcherLock(), new StorageKey("test")));
        }

        [Test]
        public void ActiveWorkersLogCleanupTask_ctor_NoLock()
        {
            Assert.Throws<ArgumentNullException>(() => new ActiveWorkersLogCleanupTask(null, new StorageKey("test")));
        }

        [Test]
        public void ActiveWorkersLogCleanupTask_Execute_Unlock()
        {
            var locker = new DispatcherLock();
            locker.Lock();
            var task = new ActiveWorkersLogCleanupTask(locker, new StorageKey("test"));
            task.Execute(new StorageContext(new Mock<IStorage>().Object));

            Assert.IsFalse(locker.IsLocked());
        }

        [Test]
        public void ActiveWorkersLogCleanupTask_Execute()
        {
            var lst = new List<ActiveWorkersLogMessage>();
            for (var i = 0; i < 25; i++)
            {
                lst.Add(new ActiveWorkersLogMessage());
            }
            var storage = new Mock<IStorage>();
            storage.Setup(x => x.GetList<ActiveWorkersLogMessage>(It.IsAny<StorageKey>())).Returns(() => lst);

            var task = new ActiveWorkersLogCleanupTask(new DispatcherLock(), new StorageKey("test"));
            task.Execute(new StorageContext(storage.Object));

            storage.Verify(x => x.RemoveRangeFromList(It.IsAny<StorageKey>(), 15), Times.Once);
        }
    }
}
