using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho
{
    public interface IGlobalConfiguration
    {
        Dictionary<string, object> Context { get; }
    }
}
