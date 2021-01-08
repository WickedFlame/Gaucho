using Gaucho.Filters;
using Gaucho.Handlers;
using System.Collections.Generic;

namespace Gaucho
{
	public static class EventPipelineExtensions
	{
		/// <summary>
		/// add a handler with a set of filters to the pipeline
		/// </summary>
		/// <param name="pipeline"></param>
		/// <param name="outputHandler"></param>
		/// <param name="filters"></param>
		public static void AddHandler(this IEventPipeline pipeline, IOutputHandler outputHandler, IEnumerable<string> filters)
		{
			var converter = filters.BuildDataFilter();
			outputHandler = new DataFilterDecorator(converter, outputHandler);
			pipeline.AddHandler(outputHandler);
		}
	}
}
