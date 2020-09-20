
namespace Gaucho.Handlers
{
	/// <summary>
	/// Decorator class for outputhandlers that automaticaly converts the data before passing the event to the outputhandler
	/// </summary>
	public class DataConversionDecorator : IOutputHandler
	{
		private readonly IEventDataConverter _converter;
		private readonly IOutputHandler _decorate;

		/// <summary>
		/// creates a new instance of the outputhandler
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="decorate"></param>
		public DataConversionDecorator(IEventDataConverter converter, IOutputHandler decorate)
		{
			_converter = converter;
			_decorate = decorate;
		}
		
		/// <summary>
		/// handle the event
		/// </summary>
		/// <param name="event"></param>
		public void Handle(Event @event)
		{
			var data = _converter.Convert(@event.Data);

			_decorate.Handle(new Event(@event.PipelineId, data));
		}
	}
}
