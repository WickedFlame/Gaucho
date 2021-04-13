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
			var processor = new EventProcessor(_worker.Object, () => { }, new Logger());
		}

		[Test]
		public void EventProcessor_Start()
		{
			var called = false;
			_worker.Setup(exp => exp.Execute()).Callback(() => called = true);
			var processor = new EventProcessor(_worker.Object, () => { }, new Logger());
			processor.Start();

			processor.Task.Wait();

			Assert.IsTrue(called);
		}

		[Test]
		public void EventProcessor_Thread()
		{
			var processor = new EventProcessor(_worker.Object, () => { }, new Logger());
			processor.Start();

			Assert.IsNotNull(processor.Task);
		}

		[Test]
		public void EventProcessor_NoThread()
		{
			var processor = new EventProcessor(_worker.Object, () => { }, new Logger());
			Assert.IsNull(processor.Task);
		}

		[Test]
		public void EventProcessor_IsWorking()
		{
			var working = true;
			_worker.Setup(exp => exp.Execute()).Callback(() => { while (working) { } });
			var processor = new EventProcessor(_worker.Object, () => { }, new Logger());
			processor.Start();

			Assert.IsTrue(processor.IsWorking);

			working = false;
			processor.Task.Wait();

			Assert.IsFalse(processor.IsWorking);
		}

		[Test]
		public void EventProcessor_EndWork()
		{
			var processor = new EventProcessor( _worker.Object, () => { }, new Logger());
			processor.Start();

			processor.Task.Wait();

			Assert.IsFalse(processor.IsWorking);
		}

		[Test]
		public void EventProcessor_Continuation()
		{
			var called = false;
			var processor = new EventProcessor(_worker.Object, () => called = true, new Logger());
			processor.Start();

			processor.Task.Wait();

			Assert.IsTrue(called);
		}
	}
}
