using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test
{
	public class EventQueueTests
	{
		[Test]
		public void EventQueue_Ctor()
		{
			var queue = new EventQueue();
		}

		[Test]
		public void EventQueue_Enqueue()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event("id", "value"));

			queue.Count.Should().Be(1);
		}

		[Test]
		public void EventQueue_Dequeue()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event("id", "value"));

			queue.TryDequeue(out var dequeued).Should().BeTrue();
		}

		[Test]
		public void EventQueue_Dequeue_Same()
		{
			var queue = new EventQueue();
			var @event = new Event("id", "value");
			queue.Enqueue(@event);
			queue.TryDequeue(out var dequeued);

			dequeued.Should().BeSameAs(@event);
		}

		[Test]
		public void EventQueue_Dequeue_All()
		{
			var queue = new EventQueue();
			queue.Enqueue(new Event("id", "value"));
			queue.TryDequeue(out var dequeued);

			queue.Count.Should().Be(0);
		}

		[Test]
		public void EventQueue_Dequeue_Empty()
		{
			var queue = new EventQueue();
			queue.TryDequeue(out var dequeued).Should().BeFalse();
		}

		[Test]
		public void EventQueue_Dequeue_Null()
		{
			var queue = new EventQueue();
			queue.TryDequeue(out var dequeued);
			dequeued.Should().BeNull();
		}
	}
}
