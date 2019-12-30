
namespace Gaucho
{
    /// <summary>
    /// Interface that can be implemented by InputHandlers that is called when the server is initialized
    /// </summary>
    public interface IServerInitialize
    {
        /// <summary>
        /// Gets called when the server is initialized
        /// </summary>
        /// <param name="server"></param>
        void Initialize(IProcessingServer server);
    }
}
