using System.Collections.Generic;

namespace Gaucho
{
    public class ConfiguredArgumentsCollection
    {
        private readonly Dictionary<string, string> _parameters;

        public ConfiguredArgumentsCollection()
        {
            _parameters = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            _parameters.Add(key, value);
        }
    }
}