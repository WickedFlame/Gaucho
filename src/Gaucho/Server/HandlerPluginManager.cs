using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gaucho.Configuration;
using Gaucho.Diagnostics;

namespace Gaucho.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class HandlerPluginManager
    {
        internal static IEnumerable<HandlerPlugin> _plugins;

        internal static IEnumerable<HandlerPlugin> GetPlugins(IEnumerable<Assembly> assemblies)
        {
            if (_plugins != null)
            {
                return _plugins;
            }

            var plugins = new List<HandlerPlugin>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes().Where(x => typeof(HandlerRegistration).IsAssignableFrom(x));
                    foreach(var type in types)
                    {
                        if (type.IsAbstract)
                        {
                            continue;
                        }

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
                logger.Write($"Loaded HandlerPlugin: {plugin.Name}", LogLevel.Info, "HandlerPluginManager");
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
                        // switched from Assembly.LoadFrom(file) to Assembly.Load for safer load context
                        var assemblyName = AssemblyName.GetAssemblyName(file);
                        var assembly = Assembly.Load(assemblyName);
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

        /// <summary>
        /// Gets a list of registered <see cref="HandlerPlugin"/> for the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<HandlerPlugin> GetPlugins(Type type)
        {
            var plugins = GetPlugins(GetAssemblies()).Where(p => p.Type.Name == type.Name || p.Type.GetInterfaces().Any(y => y.Name == type.Name) && !p.Type.IsInterface).ToList();

            return plugins;
        }

        /// <summary>
        /// Gets a list of registered <see cref="HandlerPlugin"/> for the type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public HandlerPlugin GetPlugin(Type type, string name)
        {
            var plugin = GetPlugins(type).FirstOrDefault(p => p.Name == name);
            if (plugin == null)
            {
                System.Diagnostics.Trace.WriteLine($"HandlerPlugin with name {name} does not exist");
            }

            return plugin;
        }

        /// <summary>
        /// Gets a list of registered <see cref="HandlerPlugin"/> for the type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public HandlerPlugin GetPlugin(Type type, HandlerNode node)
        {
            var plugin = GetPlugins(type).FirstOrDefault(p => p.Name == node.Name || (string.IsNullOrEmpty(node.Name) && p.Type == node.Type));
            if (plugin == null)
            {
                if (node.Type != null)
                {
	                if (type.IsAssignableFrom(node.Type))
	                {
		                return new HandlerPlugin
		                {
			                Type = node.Type
		                };
	                }
                }

                System.Diagnostics.Trace.WriteLine($"HandlerPlugin {node.Name}, {node.Type} does not exist");
			}

            return plugin;
        }
    }
}
