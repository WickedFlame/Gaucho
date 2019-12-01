
namespace WickedFlame.Yaml
{
    public interface INodeReader
    {
        object Node { get; }

        void ReadLine(YamlLine line);
    }
}
