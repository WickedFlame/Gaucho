using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Gaucho
{
	/// <summary>
	/// container for queueing events.
	/// events are fetched and processed by the EventBus
	/// </summary>
    public class EventQueue
    {
        private readonly ConcurrentQueue<Event> _queue;

		/// <summary>
		/// creates a new queue
		/// </summary>
        public EventQueue()
        {
            _queue = new ConcurrentQueue<Event>();
        }

		/// <summary>
		/// gets the amount of items in the queue
		/// </summary>
        public int Count
        {
	        get
	        {
                return _queue.Count;
            }
        }

		/// <summary>
		/// enqueue a new event
		/// </summary>
		/// <param name="event"></param>
        public void Enqueue(Event @event)
        {
            _queue.Enqueue(@event);
        }

		/// <summary>
		/// try to dequeue a event.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
        public bool TryDequeue(out Event value)
        {
            if (_queue.TryDequeue(out value))
            {
                return true;
            }

            value = default(Event);

            return false;
        }
    }
}
