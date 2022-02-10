using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test.Diagnostics
{
    public class EventQueueMetricCounterTaskTests
    {
        [Test]
        public void EventQueueMetricCounterTask_Execute_Log()
        {
            var logger = new Mock<ILogger>();
            var context = new EventQueueContext(new EventQueue(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object);
            var task = new EventQueueMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            logger.Verify(x => x.Write<LogEvent>(It.IsAny<LogEvent>(), It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public void EventQueueMetricCounterTask_Execute_Log_MultipleTimes()
        {
            var logger = new Mock<ILogger>();
            var queue = new EventQueue();
            var context = new EventQueueContext(queue, new MetricService(new Mock<IStorage>().Object, "1"), logger.Object);
            var task = new EventQueueMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(1000).Wait();

            queue.Enqueue(new Event("1", SimpleData.From("1")));

            context.WaitHandle.Set();

            logger.Verify(x => x.Write<LogEvent>(It.IsAny<LogEvent>(), It.IsAny<Category>()), Times.Exactly(2));
        }

        [Test]
        public void EventQueueMetricCounterTask_Execute_Log_MultipleTimes_QueNotChanged()
        {
            var logger = new Mock<ILogger>();
            var context = new EventQueueContext(new EventQueue(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object);
            var task = new EventQueueMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            context.WaitHandle.Set();

            logger.Verify(x => x.Write<LogEvent>(It.IsAny<LogEvent>(), It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public void EventQueueMetricCounterTask_Execute_StopThread()
        {
            var logger = new Mock<ILogger>();
            var context = new EventQueueContext(new EventQueue(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object)
            {
                IsRunning = false
            };
            var task = new EventQueueMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            context.WaitHandle.Set();

            logger.Verify(x => x.Write<LogEvent>(It.IsAny<LogEvent>(), It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public void EventQueueMetricCounterTask_Execute_SetMetric()
        {
            var storage = new Mock<IStorage>();
            var context = new EventQueueContext(new EventQueue(), new MetricService(storage.Object, "1"), new Mock<ILogger>().Object);
            var task = new EventQueueMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            storage.Verify(x => x.Set(It.IsAny<StorageKey>(), It.IsAny<IMetric>()), Times.Once);
        }

        [Test]
        public void EventQueueMetricCounterTask_Execute_SetMetric_MultipleTimes()
        {
            var storage = new Mock<IStorage>();
            var queue = new EventQueue();
            var context = new EventQueueContext(queue, new MetricService(storage.Object, "1"), new Mock<ILogger>().Object);
            var task = new EventQueueMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            queue.Enqueue(new Event("1", SimpleData.From("1")));

            context.WaitHandle.Set();

            storage.Verify(x => x.Set(It.IsAny<StorageKey>(), It.IsAny<IMetric>()), Times.Exactly(2));
        }

        [Test]
        public void EventQueueMetricCounterTask_Execute_SetMetric_MultipleTimes_QueNotChanged()
        {
            var storage = new Mock<IStorage>();
            var context = new EventQueueContext(new EventQueue(), new MetricService(storage.Object, "1"), new Mock<ILogger>().Object);
            var task = new EventQueueMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            context.WaitHandle.Set();

            storage.Verify(x => x.Set(It.IsAny<StorageKey>(), It.IsAny<IMetric>()), Times.Once);
        }
    }
}
