using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Filters;
using Gaucho.Server;

namespace Gaucho
{
    public class Plugin
    {
        public string Name { get; set; }

        public Type Type { get; set; }
    }

    public abstract class HandlerRegistration
    {
        public abstract void RegisterHandlers(HandlerRegistrationContext context);

        internal IEnumerable<Plugin> GetPlugins()
        {
            var ctx = new HandlerRegistrationContext();
            this.RegisterHandlers(ctx);

            return ctx.Plugins;
        }
    }

    public class HandlerRegistrationContext
    {
        private readonly List<Plugin> _plugins = new List<Plugin>();

        internal IEnumerable<Plugin> Plugins => _plugins;

        public void Register<T>(string name) //where T : IInputHandler
        {
            _plugins.Add(new Plugin
            {
                Name = name,
                Type = typeof(T)
            });
        }
    }

    public class PluginManager
    {
        internal static IEnumerable<Plugin> _plugins;

        internal static IEnumerable<Plugin> GetPlugins(IEnumerable<Assembly> assemblies)
        {
            if (_plugins != null)
            {
                return _plugins;
            }

            var plugins = new List<Plugin>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes().Where(x => typeof(HandlerRegistration).IsAssignableFrom(x));
                    foreach(var type in types)
                    {
                        var registration = (HandlerRegistration)Activator.CreateInstance(type);
                        plugins.AddRange(registration.GetPlugins());
                    }
                }
                catch
                {
                    // just continue
                }
            }

            _plugins = plugins;

            var logger = LoggerConfiguration.Setup();
            foreach (var plugin in _plugins)
            {
                logger.Write($"Loaded Plugin: {plugin.Name}", Category.Log, LogLevel.Info, "PluginManager");
            }

            return _plugins;
        }

        private IEnumerable<Assembly> _assemblies;

        private IEnumerable<Assembly> GetAssemblies()
        {
            if (_assemblies == null)
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                path = Path.GetDirectoryName(path);

                var files = Directory.EnumerateFiles(path, "*.dll", SearchOption.TopDirectoryOnly);

                var assemblies = new List<Assembly>();
                foreach (var file in files)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(file);
                        assemblies.Add(assembly);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e);
                    }
                }

                _assemblies = assemblies;
            }

            return _assemblies;
        }

        public IEnumerable<Plugin> GetPlugins(Type type)
        {
            var plugins = GetPlugins(GetAssemblies()).Where(p => p.Type.Name == type.Name || p.Type.GetInterfaces().Any(y => y.Name == type.Name) && !p.Type.IsInterface).ToList();

            return plugins;
        }

        public Plugin GetPlugin(Type type, string name)
        {
            var plugin = GetPlugins(type).FirstOrDefault(p => p.Name == name);
            if (plugin == null)
            {
                System.Diagnostics.Trace.WriteLine($"Plugin {name} does not exist");
            }

            return plugin;
        }

        public Plugin GetPlugin(Type type, HandlerNode node)
        {
            var plugin = GetPlugins(type).FirstOrDefault(p => p.Name == node.Name || (string.IsNullOrEmpty(node.Name) && p.Type == node.Type));
            if (plugin == null)
            {
                System.Diagnostics.Trace.WriteLine($"Plugin {node.Name}, {node.Type} does not exist");
                if (node.Type != null)
                {
                    return new Plugin
                    {
                        Type = node.Type
                    };
                }
            }

            return plugin;
        }

        public Plugin GetPlugin(Type type)
        {
            var plugin = new Plugin
            {
                Type = type
            };

            return plugin;
        }
    }

    public static class PluginManagerExtensions
    {
        public static IInputHandler GetInputHandler(this PluginManager mgr, PipelineConfiguration config)
        {
            var node = config.InputHandler;

            var handler = mgr.GetPlugin(typeof(IInputHandler), node);
            if (handler == null)
            {
                var logger = LoggerConfiguration.Setup();
                logger.Write($"Invalid InputHandler defined in configuration for Pipelilne: {config.Id}", Category.Log, LogLevel.Error);
                
                return null;
            }

            var arguments = BuildArguments(node);

            return handler.Type.CreateInstance<IInputHandler>(arguments);
        }

        public static IEnumerable<IOutputHandler> GetOutputHandlers(this PluginManager mgr, PipelineConfiguration config)
        {
            var handlers = new List<IOutputHandler>();

            foreach(var node in config.OutputHandlers)
            {
                var handler = mgr.GetPlugin(typeof(IOutputHandler), node);
                var arguments = BuildArguments(node);

                var instance = handler.Type.CreateInstance<IOutputHandler>(arguments);

                handlers.Add(instance);
            }

            return handlers;
        }

        private static Dictionary<Type, object> BuildArguments(HandlerNode node)
        {
            var arguments = new Dictionary<Type, object>
            {
                {typeof(IEventDataConverter), new EventDataConverter()},
                {typeof(ConfiguredArgumentsCollection), new ConfiguredArgumentsCollection()}
            };

            if (node.Filters != null)
            {
                var converter = new EventDataConverter();
                arguments[typeof(IEventDataConverter)] = converter;

                foreach (var filterString in node.Filters)
                {
                    var filter = FilterFactory.CreateFilter(filterString);
                    if (filter == null)
                    {
                        var logger = LoggerConfiguration.Setup();
                        logger.Write($"Could not convert '{filterString}' to a Filter", Category.Log, LogLevel.Warning, source: "FilterFactory");
                        continue;
                    }

                    converter.Add(filter);
                }
            }

            if (node.Arguments != null)
            {
                var collection = new ConfiguredArgumentsCollection();
                arguments[typeof(ConfiguredArgumentsCollection)] = collection;

                foreach (var item in node.Arguments)
                {
                    collection.Add(item.Key, item.Value);
                }
            }

            return arguments;
        }
    }
}
