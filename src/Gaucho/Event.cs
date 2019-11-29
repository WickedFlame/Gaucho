using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public class EventData : IEventData, IEnumerable
    {
        private readonly List<Property> _properties;

        public EventData()
        {
            _properties = new List<Property>();
        }

        public IEnumerable<Property> Properties => _properties;

        public void Add(string key, object value)
        {
            _properties.Add(new Property(key, value));
        }

        public IEnumerator GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            foreach (var node in Properties)
            {
                sb.AppendLine(node.ToString());
            }

            sb.Append("}");

            return sb.ToString();
        }

        public object this[string source]
        {
            get
            {
                var property = _properties.FirstOrDefault(p => p.Key == source);
                if (property == null)
                {
                    return null;
                }

                return property.Value;
            }
        }
    }

    public class Property : IConvertible
    {
        public Property(string key, object value)
        {
            Key = key;
            Value = value;
            ValueType = value?.GetType();
        }

        public string Key { get; }

        public object Value { get; }

        public Type ValueType { get; }

        public override string ToString()
        {
            return $"   {Key} => {Value}";
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
