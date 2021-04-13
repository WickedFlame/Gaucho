using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Diagnostics;
using Gaucho.Server.Monitoring;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventPipelineWorkerTests
	{
		private Mock<IEventPipeline> _pipeline;

		[SetUp]
		public void Setup()
		{
			_pipeline = new Mock<IEventPipeline>();
		}

		[Test]
		public void EventPipelineWorker_Null_Logger()
		{
			Assert.Throws<ArgumentException>(() => new EventPipelineWorker(null, new EventQueue(), new Mock<IMetricService>().Object));
		}

		[Test]
		public void EventPipelineWorker_Null_Queue()
		{
			Assert.Throws<ArgumentException>(() => new EventPipelineWorker(new Logger(), null, new Mock<IMetricService>().Object));
		}

		[Test]
		public void EventPipelineWorker_Null_Metrics()
		{
			Assert.Throws<ArgumentException>(() => new EventPipelineWorker(new Logger(), new EventQueue(), null));
		}

		[Test]
		public void EventPipelineWorker_Execute()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event(string.Empty, new EventData()));
			queue.Enqueue(new Event(string.Empty, new EventData()));

			var worker = new EventPipelineWorker(new Logger(), queue, new Mock<IMetricService>().Object);
			worker.Execute(_pipeline.Object);

			_pipeline.Verify(exp => exp.Run(It.IsAny<Event>()), Times.Exactly(2));
		}

		[Test]
		public void EventPipelineWorker_Execute_Null_Pipeline()
		{
			var worker = new EventPipelineWorker(new Logger(), new EventQueue(), new Mock<IMetricService>().Object);
			Assert.Throws<ArgumentException>(() => worker.Execute(null));
		}

		[Test]
		public void EventPipelineWorker_Execute_EmptyQueue()
		{
			var queue = new EventQueue();

			var worker = new EventPipelineWorker(new Logger(), queue, new Mock<IMetricService>().Object);
			worker.Execute(_pipeline.Object);

			_pipeline.Verify(exp => exp.Run(It.IsAny<Event>()), Times.Never);
		}

		[Test]
		public void EventPipelineWorker_Execute_Logger()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event(string.Empty, new EventData()));
			queue.Enqueue(new Event(string.Empty, new EventData()));

			var logger = new Mock<ILogger>();

			var worker = new EventPipelineWorker(logger.Object, queue, new Mock<IMetricService>().Object);
			worker.Execute(_pipeline.Object);

			logger.Verify(exp => exp.Write(It.IsAny<StatisticEvent<string>>(), Category.EventStatistic), Times.Exactly(2));
		}

		[Test]
		public void EventPipelineWorker_Execute_Metrics()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event(string.Empty, new EventData()));
			queue.Enqueue(new Event(string.Empty, new EventData()));

			var metrics = new Mock<IMetricService>();

			var worker = new EventPipelineWorker(new Logger(), queue, metrics.Object);
			worker.Execute(_pipeline.Object);

			metrics.Verify(exp => exp.SetMetric(It.Is<Metric>(m => (int)m.Value == 1)), Times.Exactly(1));
			metrics.Verify(exp => exp.SetMetric(It.Is<Metric>(m => (int)m.Value == 0)), Times.Exactly(1));
		}

		[Test]
		public void EventPipelineWorker_Execute_Error()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event(string.Empty, new EventData()));
			queue.Enqueue(new Event(string.Empty, new EventData()));

			var logger = new Mock<ILogger>();
			_pipeline.Setup(exp => exp.Run(It.IsAny<Event>())).Throws(new Exception());

			var worker = new EventPipelineWorker(logger.Object, queue, new Mock<IMetricService>().Object);
			worker.Execute(_pipeline.Object);

			logger.Verify(exp => exp.Write(It.Is<LogEvent>(l => l.Level == LogLevel.Error && l.Source == "EventBus"), Category.Log), Times.Exactly(2));
		}
	}
}
