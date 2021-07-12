using System;
using System.Text.RegularExpressions;
using Gaucho.Dashboard.Monitoring;
using Gaucho.Server.Monitoring;
using Microsoft.AspNetCore.Http;

namespace Gaucho.Dashboard
{
	/// <summary>
	/// 
	/// </summary>
    public class DashboardContext
    {
		/// <summary>
		/// Creates a new instance of the DashboardContext
		/// </summary>
		/// <param name="httpContext"></param>
		/// <param name="monitor"></param>
		/// <param name="options"></param>
        public DashboardContext(HttpContext httpContext, IPipelineMonitor monitor, DashboardOptions options)
        {
	        HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            Request = new DashboardRequest(httpContext);
            Response = new DashboardResponse(httpContext);

            Monitor = monitor;
            Options = options;

            //var antiforgery = HttpContext.RequestServices.GetService<IAntiforgery>();
        }

		/// <summary>
		/// Gets the <see cref="HttpContext"/>
		/// </summary>
        public HttpContext HttpContext { get; }

		/// <summary>
		/// Gets or sets the <see cref="IPipelineMonitor"/>
		/// </summary>
        public IPipelineMonitor Monitor { get; }

		/// <summary>
		/// Gets the <see cref="DashboardOptions"/>
		/// </summary>
        public DashboardOptions Options { get; }

		/// <summary>
		/// Gets the <see cref="Match"/>
		/// </summary>
        public Match UriMatch { get; set; }

		/// <summary>
		/// Gets the <see cref="DashboardResponse"/>
		/// </summary>
        public DashboardResponse Response { get; protected set; }

		/// <summary>
		/// Gets the <see cref="DashboardRequest"/>
		/// </summary>
        public DashboardRequest Request { get; protected set; }
    }
}
