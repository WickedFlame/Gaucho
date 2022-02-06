using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaucho
{
    public class EventProcessorList : IEnumerable<EventProcessor>
    {
        private readonly object _lock = new object();
        private readonly List<EventProcessor> _eventProcessors;

        public EventProcessorList()
        {
            _eventProcessors = new List<EventProcessor>();
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _eventProcessors.Count;
                }
            }
        }

        public EventProcessor this[int index]
        {
            get
            {
                lock (_lock)
                {
                    return _eventProcessors[index];
                }
            }
        }

        public void Add(EventProcessor processor)
        {
            lock(_lock)
            {
                _eventProcessors.Add(processor);
            }
        }

        public void Remove(EventProcessor processor)
        {
            lock (_lock)
            {
                _eventProcessors.Remove(processor);
            }
        }

        public Task[] GetTasks()
        {
            lock (_lock)
            {
                //return _eventProcessors.Select(t => t.Task)
                //    .Where(t => t != null && t.Status == TaskStatus.Running)
                //    .ToArray();
                return _eventProcessors.Where(p => p.IsWorking).Select(t => t.Task)
                    .Where(t => t != null)
                    .ToArray();
            }
        }

        public IEnumerator<EventProcessor> GetEnumerator()
        {
            lock(_lock)
            {
                return _eventProcessors.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
