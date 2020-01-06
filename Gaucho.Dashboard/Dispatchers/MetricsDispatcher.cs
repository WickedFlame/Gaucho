using System.Collections.Generic;
using System.Threading.Tasks;
using Gaucho.Server.Monitoring;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Gaucho.Dashboard.Dispatchers
{
    internal class MetricsDispatcher : IDashboardDispatcher
    {
        public async Task Dispatch(DashboardContext context)
        {
            var page = new StubPage();
            page.Assign(context);

            var monitor = context.ServerMonitor;
            var pipelines = monitor.GetPipelineMetrics();

            var result = new Dictionary<string, PipelineMetric>();

            foreach (var pipeline in pipelines)
            {
                result.Add(pipeline.Name, pipeline);
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new JsonConverter[] {new StringEnumConverter {CamelCaseText = true}}
            };
            var serialized = JsonConvert.SerializeObject(result, settings);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(serialized);
        }

        private class StubPage : RazorPage
        {
            public override void Execute()
            {
            }
        }
    }
}
