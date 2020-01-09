using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Gaucho.Dashboard
{
    public class DashboardResponse
    {
        private readonly HttpContext _context;

        public DashboardResponse(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public string ContentType
        {
            get => _context.Response.ContentType;
            set => _context.Response.ContentType = value;
        }

        public int StatusCode
        {
            get => _context.Response.StatusCode;
            set => _context.Response.StatusCode = value;
        }

        public Stream Body => _context.Response.Body;

        //public override void SetExpire(DateTimeOffset? value)
        //{
        //    _context.Response.Expires = value;
        //}

        public Task WriteAsync(string text)
        {
            return _context.Response.WriteAsync(text);
        }
    }
}
