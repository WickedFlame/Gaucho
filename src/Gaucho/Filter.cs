
namespace Gaucho
{
    public interface IFilter
    {
        //https://www.elastic.co/guide/en/logstash/current/filter-plugins.html
        /*
         filter {
          json {
            source => "message"
          }
        }
         *
         */
        Property Filter(EventData data);
    }

    public class PropertyFilter : IFilter
    {
        private readonly string _source;
        private readonly string _destination;

        public PropertyFilter(string property)
        {
            _source = property;
            _destination = property;
        }

        public PropertyFilter(string source, string destination)
        {
            _source = source;
            _destination = destination;
        }

        public Property Filter(EventData data)
        {
            var value = data[_source];
            return new Property(_destination, value);
        }
    }
}
