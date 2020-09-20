using System.Collections.Generic;

namespace Gaucho
{
	/// <summary>
	/// container for queueing events.
	/// events are fetched and processed by the EventBus
	/// </summary>
    public class EventQueue
    {
        private readonly object _syncRoot = new object();
        private readonly Queue<Event> _queue;

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
				lock(_syncRoot)
				{
					return _queue.Count;
				}
	        }
        }

		/// <summary>
		/// enqueue a new event
		/// </summary>
		/// <param name="event"></param>
        public void Enqueue(Event @event)
        {
            lock (_syncRoot)
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
            lock (_syncRoot)
            {
                if (_queue.Count > 0)
                {
                    value = _queue.Dequeue();

                    return true;
                }

                value = default(Event);

                return false;
            }
        }
    }
}
