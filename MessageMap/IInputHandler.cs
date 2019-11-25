
namespace MessageMap
{
    public interface IInputHandler
    {
        IConverter Converter { get; set; }

        string PipelineId { get; set; }
    }

    public interface IInputHandler<T> : IInputHandler
    {
        Event ProcessInput(T input);
    }
}
