using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Diagnostics;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventProcessorTests
	{
		private Mock<IWorker> _worker;

		[SetUp]
		public void Setup()
		{
			_worker = new Mock<IWorker>();
		}

		[Test]
		public void EventProcessor_Ctor()
		{
			var processor = new EventProcessor(_worker.Object, () => { }, p => { }, new Logger());
		}

		[Test]
		public void EventProcessor_Start()
		{
			var called = false;
			_worker.Setup(exp => exp.Execute()).Callback(() => called = true);
			var processor = new EventProcessor(_worker.Object, () => { }, p => { }, new Logger());
			processor.Start();

			System.Threading.Tasks.Task.Delay(1000).Wait();

            processor.Stop();

			processor.Task.Wait();

			Assert.IsTrue(called);
		}

		[Test]
		public void EventProcessor_Thread()
		{
			var processor = new EventProcessor(_worker.Object, () => { }, p => { }, new Logger());
			processor.Start();

			Assert.IsNotNull(processor.Task);
		}

		[Test]
		public void EventProcessor_NoThread()
		{
			var processor = new EventProcessor(_worker.Object, () => { }, p => { }, new Logger());
			Assert.IsNull(processor.Task);
		}

		[Test]
		public void EventProcessor_IsWorking()
		{
			_worker.Setup(exp => exp.Execute()).Callback(() => { });
			var processor = new EventProcessor(_worker.Object, () => { }, p => { }, new Logger());
			processor.Start();

			Assert.IsTrue(processor.IsWorking);

            processor.Stop();

			processor.Task.Wait();

			Assert.IsFalse(processor.IsWorking);
		}

		[Test]
		public void EventProcessor_Stop()
		{
			var processor = new EventProcessor( _worker.Object, () => { }, p => { }, new Logger());
			processor.Start();
            processor.Stop();

			processor.Task.Wait();

			Assert.IsFalse(processor.IsWorking);
		}

        [Test]
        public void EventProcessor_OnEndProcessing()
        {
            var processor = new EventProcessor(_worker.Object, () => { }, p => p.Stop(), new Logger());
            processor.Start();

            processor.Task.Wait();

            Assert.IsFalse(processor.IsWorking);
        }

		[Test]
		public void EventProcessor_Continuation()
		{
			var called = false;
			var processor = new EventProcessor(_worker.Object, () => called = true, p => { }, new Logger());
			processor.Start();
            processor.Stop();

			processor.Task.Wait();

			Assert.IsTrue(called);
		}

        [Test]
        public void EventProcessor_OnEndTask()
        {
			var ended = false;

            var processor = new EventProcessor(_worker.Object, () => ended = true, p => p.Stop(), new Logger());
            processor.Start();

            processor.Task.Wait();

            Assert.IsTrue(ended);
        }
	}
}
