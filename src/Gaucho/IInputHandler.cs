
namespace Gaucho
{
    public interface IInputHandler
    {
        string PipelineId { get; set; }
    }

    public interface IInputHandler<T> : IInputHandler
    {
        Event ProcessInput(T input);
    }
}
