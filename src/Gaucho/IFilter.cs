
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
}
