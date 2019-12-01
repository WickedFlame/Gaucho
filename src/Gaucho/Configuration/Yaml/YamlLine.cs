
namespace WickedFlame.Yaml
{
    public class YamlLine
    {
        public YamlLine(string line)
        {
            Original = line;
            Line = line.TrimStart();
            Indentation = line.Length - Line.Length;

            if (line.TrimStart().StartsWith("- "))
            {
                IsListItem = true;
                line = line.TrimStart().Substring(2);
                Value = line;
            }

            var index = line.IndexOf(':');
            if (index > 0)
            {
                Property = line.Substring(0, index).TrimStart();
                Value = line.Substring(index + 1);
            }
        }

        public string Original { get; }

        public int Indentation { get; set; }

        public string Line { get; }

        public string Property { get; }

        public string Value { get; }

        public bool IsListItem { get; }
    }
}
