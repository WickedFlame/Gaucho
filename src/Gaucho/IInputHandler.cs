
namespace Gaucho
{
    public interface IInputHandler
    {
        string PipelineId { get; set; }
    }

    public interface IInputHandler<in T> : IInputHandler
    {
        Event ProcessInput(T input);
    }
}
