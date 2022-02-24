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
        private readonly Queue<Event> _queue;
        private readonly object _lock = new object();

		/// <summary>
		/// creates a new queue
		/// </summary>
        public EventQueue()
        {
            _queue = new Queue<Event>();
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
            lock(_lock)
            {
                _queue.Enqueue(@event);
            }
        }

		/// <summary>
		/// try to dequeue a event.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
        public bool TryDequeue(out Event value)
        {
            //if (_queue.TryDequeue(out value))
            //{
            //    return true;
            //}

            lock(_lock)
            {
                if (_queue.Count == 0)
                {
                    value = null;
                    return false;
                }

                var item = _queue.Dequeue();
                if (item != null)
                {
                    value = item;
                    return true;
                }

                value = default(Event);

                return false;
            }
        }
    }
}
