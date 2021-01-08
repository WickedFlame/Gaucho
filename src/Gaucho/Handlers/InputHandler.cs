namespace Gaucho.Handlers
{
	/// <summary>
	/// a generic inputhandler that simply translates the generic item to eventdata
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class InputHandler<T> : IInputHandler<T>
	{
		/// <summary>
		/// the id of the pipeline
		/// </summary>
		public string PipelineId { get; set; }

		/// <summary>
		/// process the input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public Event ProcessInput(T input)
		{
			return new Event(PipelineId, f => f.BuildFrom(input));
		}
	}
}
