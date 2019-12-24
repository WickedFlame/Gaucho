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
            config.Context.Add(item.GetType().Name, item);
        }
    }
}
