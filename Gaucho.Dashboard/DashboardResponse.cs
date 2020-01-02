using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Gaucho.Dashboard
{
    public abstract class DashboardResponse
    {
        public abstract string ContentType { get; set; }
        public abstract int StatusCode { get; set; }

        public abstract Stream Body { get; }

        //public abstract void SetExpire(DateTimeOffset? value);
        public abstract Task WriteAsync(string text);
    }
}
