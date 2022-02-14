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
    [SingleThreaded]
    public class EventQueueMetricCounterTaskTests
    {
        [Test]
        public void EventBusMetricCounterTask_Execute_EventQueue_Log()
        {
            var logger = new Mock<ILogger>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            logger.Verify(x => x.Write<LogEvent>(It.Is<LogEvent>(l => l.Source == "EventQueue"), It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_EventQueue_Log_MultipleTimes()
        {
            var logger = new Mock<ILogger>();
            var queue = new EventQueue();
            var context = new EventBusContext(queue, new EventProcessorList(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(1000).Wait();

            queue.Enqueue(new Event("1", SimpleData.From("1")));

            context.WaitHandle.Set();

            logger.Verify(x => x.Write<LogEvent>(It.Is<LogEvent>(l => l.Source == "EventQueue"), It.IsAny<Category>()), Times.Exactly(2));
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_Log_MultipleTimes_NotChanged()
        {
            var logger = new Mock<ILogger>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            context.WaitHandle.Set();

            logger.Verify(x => x.Write<LogEvent>(It.Is<LogEvent>(l => l.Source == "EventQueue"), It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_EventQueue_StopThread()
        {
            var logger = new Mock<ILogger>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object)
            {
                IsRunning = false
            };
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            context.WaitHandle.Set();

            logger.Verify(x => x.Write<LogEvent>(It.Is<LogEvent>(l => l.Source == "EventQueue"), It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_EventQueue_SetMetric()
        {
            var storage = new Mock<IStorage>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(storage.Object, "1"), new Mock<ILogger>().Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            storage.Verify(x => x.Set(It.IsAny<StorageKey>(), It.Is<IMetric>(m => m.Key == MetricType.QueueSize)), Times.Once);
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_EventQueue_SetMetric_MultipleTimes()
        {
            var storage = new Mock<IStorage>();
            var queue = new EventQueue();
            var context = new EventBusContext(queue, new EventProcessorList(), new MetricService(storage.Object, "1"), new Mock<ILogger>().Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(1000).Wait();

            queue.Enqueue(new Event("1", SimpleData.From("1")));

            context.WaitHandle.Set();

            storage.Verify(x => x.Set(It.IsAny<StorageKey>(), It.Is<IMetric>(m => m.Key == MetricType.QueueSize)), Times.Exactly(2));
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_EventQueue_SetMetric_MultipleTimes_QueueNotChanged()
        {
            var storage = new Mock<IStorage>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(storage.Object, "1"), new Mock<ILogger>().Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            context.WaitHandle.Set();

            storage.Verify(x => x.Set(It.IsAny<StorageKey>(), It.Is<IMetric>(m => m.Key == MetricType.QueueSize)), Times.Once);
        }





        [Test]
        public void EventBusMetricCounterTask_Execute_Processors_Log()
        {
            var logger = new Mock<ILogger>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            logger.Verify(x => x.Write<LogEvent>(It.Is<LogEvent>(l => l.Source == "EventProcessor"), It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_Processors_Log_MultipleTimes()
        {
            var logger = new Mock<ILogger>();
            var processors = new EventProcessorList();
            var context = new EventBusContext(new EventQueue(), processors, new MetricService(new Mock<IStorage>().Object, "1"), logger.Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(1000).Wait();

            processors.Add(new EventProcessor(new Mock<IWorker>().Object, () => { }, new Mock<ILogger>().Object));

            context.WaitHandle.Set();

            logger.Verify(x => x.Write<LogEvent>(It.Is<LogEvent>(l => l.Source == "EventProcessor"), It.IsAny<Category>()), Times.Exactly(2));
        }

        [Test]
        public void EventBusMetricCounterTask_Processors_Log_MultipleTimes_NotChanged()
        {
            var logger = new Mock<ILogger>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            context.WaitHandle.Set();

            logger.Verify(x => x.Write<LogEvent>(It.Is<LogEvent>(l => l.Source == "EventProcessor"), It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_Processors_StopThread()
        {
            var logger = new Mock<ILogger>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(new Mock<IStorage>().Object, "1"), logger.Object)
            {
                IsRunning = false
            };
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            context.WaitHandle.Set();

            logger.Verify(x => x.Write<LogEvent>(It.Is<LogEvent>(l => l.Source == "EventProcessor"), It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_Processors_SetMetric()
        {
            var storage = new Mock<IStorage>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(storage.Object, "1"), new Mock<ILogger>().Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            storage.Verify(x => x.Set(It.IsAny<StorageKey>(), It.Is<IMetric>(m => m.Key == MetricType.ThreadCount)), Times.Once);
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_Processors_SetMetric_MultipleTimes()
        {
            var storage = new Mock<IStorage>();
            var processors = new EventProcessorList();
            var context = new EventBusContext(new EventQueue(), processors, new MetricService(storage.Object, "1"), new Mock<ILogger>().Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            processors.Add(new EventProcessor(new Mock<IWorker>().Object, () => { }, new Mock<ILogger>().Object));

            context.WaitHandle.Set();

            storage.Verify(x => x.Set(It.IsAny<StorageKey>(), It.Is<IMetric>(m => m.Key == MetricType.ThreadCount)), Times.Exactly(2));
        }

        [Test]
        public void EventBusMetricCounterTask_Execute_Processors_SetMetric_MultipleTimes_QueueNotChanged()
        {
            var storage = new Mock<IStorage>();
            var context = new EventBusContext(new EventQueue(), new EventProcessorList(), new MetricService(storage.Object, "1"), new Mock<ILogger>().Object);
            var task = new EventBusMetricCounterTask("server", "1");
            Task.Factory.StartNew(() => task.Execute(context));

            Task.Delay(100).Wait();

            context.WaitHandle.Set();

            storage.Verify(x => x.Set(It.IsAny<StorageKey>(), It.Is<IMetric>(m => m.Key == MetricType.ThreadCount)), Times.Once);
        }
    }
}
