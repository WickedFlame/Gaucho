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
			Assert.Throws<ArgumentException>(() => new EventPipelineWorker(new EventQueue(), () => null, null));
		}

		[Test]
		public void EventPipelineWorker_Null_Queue()
		{
			Assert.Throws<ArgumentException>(() => new EventPipelineWorker(null, () => null, new Logger()));
		}

		[Test]
		public void EventPipelineWorker_Execute()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event(string.Empty, new EventData()));
			queue.Enqueue(new Event(string.Empty, new EventData()));

			var worker = new EventPipelineWorker(queue, () => _pipeline.Object, new Logger());
			worker.Execute();

			_pipeline.Verify(exp => exp.Run(It.IsAny<Event>()), Times.Exactly(2));
		}

		[Test]
		public void EventPipelineWorker_Execute_Null_Pipeline()
		{
			var worker = new EventPipelineWorker(new EventQueue(), () => null, new Logger());
			Assert.Throws<ArgumentException>(() => worker.Execute());
		}

		[Test]
		public void EventPipelineWorker_Execute_EmptyQueue()
		{
			var queue = new EventQueue();

			var worker = new EventPipelineWorker(queue, () => _pipeline.Object, new Logger());
			worker.Execute();

			_pipeline.Verify(exp => exp.Run(It.IsAny<Event>()), Times.Never);
		}

		[Test]
		public void EventPipelineWorker_Execute_Logger()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event(string.Empty, new EventData()));
			queue.Enqueue(new Event(string.Empty, new EventData()));

			var logger = new Mock<ILogger>();

			var worker = new EventPipelineWorker(queue, () => _pipeline.Object, logger.Object);
			worker.Execute();

			logger.Verify(exp => exp.Write(It.IsAny<StatisticEvent<string>>(), Category.EventStatistic), Times.Exactly(2));
		}

		[Test]
		public void EventPipelineWorker_Execute_Error()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event(string.Empty, new EventData()));
			queue.Enqueue(new Event(string.Empty, new EventData()));

			var logger = new Mock<ILogger>();
			_pipeline.Setup(exp => exp.Run(It.IsAny<Event>())).Throws(new Exception());

			var worker = new EventPipelineWorker(queue, () => _pipeline.Object, logger.Object);
			worker.Execute();

			logger.Verify(exp => exp.Write(It.Is<LogEvent>(l => l.Level == LogLevel.Error && l.Source == "EventBus"), Category.Log), Times.Exactly(2));
		}
	}
}
