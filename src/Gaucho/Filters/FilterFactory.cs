
using System.Collections.Generic;

namespace Gaucho.Filters
{
	public delegate IFilter FilterBuilder(string input);

	public class FilterFactory
    {
	    static FilterFactory()
	    {
		    BuildFilter = DefaultBuilder;
	    }

		public static FilterBuilder BuildFilter { get; set; }

        private static IFilter DefaultBuilder(string input)
        {
	        if (string.IsNullOrEmpty(input))
	        {
				return null;
	        }

	        if (input.StartsWith("'") && input.EndsWith("'"))
	        {
		        input = input.Substring(1, input.Length - 2);
	        }

	        var formatterIndex = input?.IndexOf("<-") ?? -1;
	        if (formatterIndex > 0)
	        {
		        var name = input.Substring(0, formatterIndex)
			        .Trim();
		        switch (name.ToLower())
		        {
					case "json":
						input = input.Substring(formatterIndex + 2)
							.Trim()
							.Trim('[', ']');
						var filters = new List<PropertyFilter>();
						foreach (var argument in input.Split(','))
						{
							if (BuildFilter(argument) is PropertyFilter filter)
							{
								filters.Add(filter);
							}
						}

						return new JsonFilter(filters);

					default:
						input = input.Substring(formatterIndex + 2)
							.Trim();

						return new FormatFilter(name, input);
		        }
				
	        }

	        var propertyIndex = input?.IndexOf("->") ?? -1;
            if (propertyIndex > 0)
            {
                var source = input.Substring(0, propertyIndex).Trim();
                var destination = input.Substring(propertyIndex + 2).Trim();
                return new PropertyFilter(source, destination);
            }

            return new PropertyFilter(input);
        }
    }
}
