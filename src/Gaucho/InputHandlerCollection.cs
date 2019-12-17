using System.Collections;
using System.Collections.Generic;

namespace Gaucho
{
    public class InputHandlerCollection : IEnumerable<IInputHandler>
    {
        private readonly Dictionary<string, IInputHandler> _plugins;

        public InputHandlerCollection()
        {
            _plugins = new Dictionary<string, IInputHandler>();
        }

        public void Register(string pipelineId, IInputHandler plugin)
        {
            plugin.PipelineId = pipelineId;

            _plugins[pipelineId] = plugin;
        }

        public IInputHandler<T> GetHandler<T>(string pipelineId)
        {
            return _plugins[pipelineId] as IInputHandler<T>;
        }

        public IEnumerator<IInputHandler> GetEnumerator()
        {
            return _plugins.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
