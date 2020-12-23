using Gaucho.Diagnostics;
using Gaucho.Filters;

namespace Gaucho.Configuration
{
	public static class HandlerNodeExtensions
	{
		/// <summary>
		/// build a converter based on the filters in the node
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static IEventDataConverter BuildDataFilter(this HandlerNode node)
		{
			if (node.Filters == null)
			{
				return new EventDataConverter();
			}

			return node.Filters.BuildDataFilter();
		}

		/// <summary>
		/// build the collection of given arguments
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static ConfiguredArguments BuildArguments(this HandlerNode node)
		{
			var collection = new ConfiguredArguments();
			if (node.Arguments == null)
			{
				return collection;
			}

			foreach (var item in node.Arguments)
			{
				collection.Add(item.Key, item.Value);
			}

			return collection;
		}
	}
}
