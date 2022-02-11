using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gaucho.Storage;

namespace Gaucho.Dashboard.Dispatchers
{
    public class ClearMetricsDispatcher : IDashboardDispatcher
    {
        public async Task Dispatch(DashboardContext context)
        {
            var path = context.Request.Path;
            var pipelineId = path.Substring(path.LastIndexOf("/") + 1);
            path = path.Remove(path.LastIndexOf("/"));
            var server = path.Substring(path.LastIndexOf("/") + 1);
            var storage = GlobalConfiguration.Configuration.Resolve<IStorage>();
            storage.ClearMetrics(server, pipelineId);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(pipelineId);
        }
    }
}
