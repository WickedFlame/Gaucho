using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Handlers
{
	/// <summary>
	/// Decorator class for outputhandlers that automaticaly converts the data before passing the event to the outputhandler
	/// </summary>
	public class DataConversionDecorator : IOutputHandler
	{
		private readonly IEventDataConverter _converter;
		private readonly IOutputHandler _decorate;

		public DataConversionDecorator(IEventDataConverter converter, IOutputHandler decorate)
		{
			_converter = converter;
			_decorate = decorate;
		}
		
		public void Handle(Event @event)
		{
			var data = _converter.Convert(@event.Data);

			_decorate.Handle(new Event(@event.PipelineId, data));
		}
	}
}
