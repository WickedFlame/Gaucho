using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Diagnostics;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventProcessorTests
	{
		[Test]
		public void EventProcessor_Ctor()
		{
			var processor = new EventProcessor(() => null, a => {}, new Logger());
		}

		[Test]
		public void EventProcessor_Start()
		{
			var called = false;
			var processor = new EventProcessor(() => null, a => called = true, new Logger());
			processor.Start();

			processor.Task.Wait();

			Assert.IsTrue(called);
		}

		[Test]
		public void EventProcessor_Thread()
		{
			var processor = new EventProcessor(() => null, a => { }, new Logger());
			processor.Start();

			Assert.IsNotNull(processor.Task);
		}

		[Test]
		public void EventProcessor_NoThread()
		{
			var processor = new EventProcessor(() => null, a => { }, new Logger());
			Assert.IsNull(processor.Task);
		}

		[Test]
		public void EventProcessor_IsWorking()
		{
			var working = true;
			var processor = new EventProcessor(() => null, a => { while (working) { } }, new Logger());
			processor.Start();

			Assert.IsTrue(processor.IsWorking);

			working = false;
		}

		[Test]
		public void EventProcessor_EndWork()
		{
			var processor = new EventProcessor(() => null, a => { }, new Logger());
			processor.Start();

			processor.Task.Wait();

			Assert.IsFalse(processor.IsWorking);
		}
	}
}
