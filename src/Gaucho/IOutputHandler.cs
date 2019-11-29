
namespace Gaucho
{
    public interface IOutputHandler
    {
        IConverter Converter { get; set; }

        void Handle(Event @event);
    }
}
