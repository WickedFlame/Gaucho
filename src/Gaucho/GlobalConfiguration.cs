using System.Collections.Generic;

namespace Gaucho
{
    public class GlobalConfiguration : IGlobalConfiguration
    {
        internal GlobalConfiguration()
        {
        }

        public static IGlobalConfiguration Configuration { get; } = new GlobalConfiguration();

        public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();
    }
}
