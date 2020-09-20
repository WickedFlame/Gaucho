using Gaucho.Server.Test.Controllers;

namespace Gaucho.Server.Test.Handlers
{
    public class GenericInputHandler<T> : IInputHandler<T>
    {
        public string PipelineId { get; set; }

        public Event ProcessInput(T input)
        {
	        return new Event(PipelineId, f => f.BuildFrom(input));
        }
    }
}
