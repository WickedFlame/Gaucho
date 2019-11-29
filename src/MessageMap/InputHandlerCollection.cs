using System.Collections.Generic;

namespace MessageMap
{
    public class InputHandlerCollection
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
    }
}
