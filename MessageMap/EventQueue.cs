using System.Collections.Generic;

namespace MessageMap
{
    public class EventQueue
    {
        private readonly object _syncRoot = new object();
        private readonly Queue<Event> _queue;

        public EventQueue()
        {
            _queue = new Queue<Event>();
        }

        public int Count => _queue.Count;
        
        public void Enqueue(Event @event)
        {
            lock (_syncRoot)
            {
                _queue.Enqueue(@event);
            }
        }

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
