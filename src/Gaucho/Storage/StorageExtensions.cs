using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server.Monitoring;

namespace Gaucho.Storage
{
    public static class StorageExtensions
    {
        public static void ClearMetrics(this IStorage storage, string server, string pipelineId)
        {
            storage.Delete(new StorageKey(pipelineId, $"log:{MetricType.WorkersLog}", server));
            storage.Delete(new StorageKey(pipelineId, $"logs", server));

            storage.Delete(new StorageKey(pipelineId, $"metric:{MetricType.ProcessedEvents}", server));
            storage.Delete(new StorageKey(pipelineId, $"metric:{MetricType.ThreadCount}", server));
            storage.Delete(new StorageKey(pipelineId, $"metric:{MetricType.QueueSize}", server));
        }
    }
}
