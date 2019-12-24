using System;
using Gaucho.Configuration;

namespace Gaucho
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseProcessingServer(this IGlobalConfiguration config, Action<PipelineBuilder> setup)
        {
            var builder = new PipelineBuilder();
            setup.Invoke(builder);

            return config;
        }

        public static void Register<T>(this IGlobalConfiguration config, T item)
        {
            var key = typeof(T).Name;
            config.Context.Add(key, item);
        }

        public static T Resolve<T>(this IGlobalConfiguration config)
        {
            var key = typeof(T).Name;
            if (config.Context.ContainsKey(key))
            {
                return (T)config.Context[key];
            }

            return default(T);
        }
    }
}
