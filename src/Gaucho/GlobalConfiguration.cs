using Gaucho.Configuration;
using System.Collections.Generic;
using Gaucho.Storage;

namespace Gaucho
{
    public class GlobalConfiguration : IGlobalConfiguration
    {
        internal GlobalConfiguration()
        {
        }

        public static IGlobalConfiguration Configuration { get; } = new GlobalConfiguration()
	        .Register(new Options())
	        .Register<IStorage>(new InmemoryStorage());

        public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();
    }
}
