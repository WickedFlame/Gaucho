using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Gaucho.Dashboard
{
    public class DashboardRequest
    {
        private readonly HttpContext _context;

        public DashboardRequest(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public string Method => _context.Request.Method;
        public string Path => _context.Request.Path.Value;
        public string PathBase => _context.Request.PathBase.Value;
        public string LocalIpAddress => _context.Connection.LocalIpAddress.ToString();
        public string RemoteIpAddress => _context.Connection.RemoteIpAddress.ToString();
        public string GetQuery(string key) => _context.Request.Query[key];

        public async Task<IList<string>> GetFormValuesAsync(string key)
        {
            var form = await _context.Request.ReadFormAsync();
            return form[key];
        }
    }
}
