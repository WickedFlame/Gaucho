﻿using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Gaucho.Dashboard.Dispatchers
{
    internal class MetricsDispatcher : IDashboardDispatcher
    {
        public async Task Dispatch(DashboardContext context)
        {
            var monitor = context.Monitor;
            var pipelines = monitor.GetMetrics();
            
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new JsonConverter[] {new StringEnumConverter {NamingStrategy = new CamelCaseNamingStrategy()} }
            };
            var serialized = JsonConvert.SerializeObject(pipelines, settings);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(serialized);
        }
    }
}
