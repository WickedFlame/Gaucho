using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho
{
    public class GlobalConfiguration : IGlobalConfiguration
    {
        internal GlobalConfiguration()
        {
        }

        public static IGlobalConfiguration Configuration { get; } = new GlobalConfiguration();
    }
}
