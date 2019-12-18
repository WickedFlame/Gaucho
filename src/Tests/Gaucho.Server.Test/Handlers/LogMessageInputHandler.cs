using Gaucho.Server.Test.Controllers;

namespace Gaucho.Server.Test.Handlers
{
    public class LogMessageInputHandler : IInputHandler<LogMessage>
    {
        public string PipelineId { get; set; }

        public Event ProcessInput(LogMessage input)
        {
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            return new Event(PipelineId, data);
        }
    }
}
