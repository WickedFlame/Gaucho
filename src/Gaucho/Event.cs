using Gaucho.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Gaucho
{
    public class Event
    {
        public Event(string pipelineId, IEventData data)
        {
            PipelineId = pipelineId;
            Data = data;

            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; }

        public string PipelineId { get; }

        public IEventData Data { get; }
    }

    public interface IEventData
    {

    }

    public class SingleNode : IEventData
    {
        public SingleNode(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
