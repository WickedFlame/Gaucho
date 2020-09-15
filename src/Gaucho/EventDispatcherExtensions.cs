using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho
{
	public static class EventDispatcherExtensions
	{
		public static void Process(this EventDispatcher dispatcher, string pipelineId, Func<EventDataFactory, EventData> exp)
		{
			var factory = new EventDataFactory();
			var data = exp(factory);
			dispatcher.Process(pipelineId, data);
		}
	}
}
