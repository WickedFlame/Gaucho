
namespace Gaucho.Filters
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

		string Key { get; }

		FilterType FilterType { get; }
			
		Property Filter(EventData data);
    }
}
