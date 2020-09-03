using System;
using System.Collections.Generic;

namespace Gaucho
{
    public class ConfiguredArguments
    {
        private readonly Dictionary<string, string> _parameters;

        public ConfiguredArguments()
        {
            _parameters = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            _parameters.Add(key.ToLower(), value);
        }

        public string GetRaw(string key)
        {
            key = key.ToLower();

            if (!_parameters.ContainsKey(key))
            {
                return null;
            }

            var value = _parameters[key];
            return value;
        }

        public T GetValue<T>(string key)
        {
            var value = GetRaw(key);
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            try
            {
                return Parse<T>(value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);

                return default(T);
            }
        }

        private static T Parse<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            if (typeof(T) == typeof(bool))
            {
                if (value == "1")
                {
                    return (T)(object)true;
                }

                if (value == "0")
                {
                    return (T)(object)false;
                }

                bool.TryParse(value, out bool boolean);
                return (T)(object)boolean;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}