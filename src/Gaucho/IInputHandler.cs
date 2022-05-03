
namespace Gaucho
{
    /// <summary>
    /// InputHandler that processes the events by itself.
    /// Import the <see cref="IProcessingServer"/> through the Constructor to pass the Events to the server
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// Id of the Pipeline that the Handler belongs to
        /// </summary>
        string PipelineId { get; set; }
    }

    /// <summary>
    /// InputHandler that returns a <see cref="Event"/> that will be sent to the server for processing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInputHandler<in T> : IInputHandler
    {
        /// <summary>
        /// Process the data to generate a <see cref="Event"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Event ProcessInput(T input);
    }
}
