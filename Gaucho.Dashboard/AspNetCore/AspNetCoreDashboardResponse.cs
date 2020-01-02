using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Gaucho.Dashboard
{
    internal sealed class AspNetCoreDashboardResponse : DashboardResponse
    {
        private readonly HttpContext _context;

        public AspNetCoreDashboardResponse(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public override string ContentType
        {
            get => _context.Response.ContentType;
            set => _context.Response.ContentType = value;
        }

        public override int StatusCode
        {
            get => _context.Response.StatusCode;
            set => _context.Response.StatusCode = value;
        }

        public override Stream Body => _context.Response.Body;

        //public override void SetExpire(DateTimeOffset? value)
        //{
        //    _context.Response.Expires = value;
        //}

        public override Task WriteAsync(string text)
        {
            return _context.Response.WriteAsync(text);
        }
    }
}
