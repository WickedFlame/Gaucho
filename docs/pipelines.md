---
title: Pipelines
layout: "default"
nav_order: 3
---
## Pipelines

Each pipeline runs its own queue in dedicated workerthreads.
Depending on the size of the queue, the pipeline starts a number of threads to process the events in the queue.

### Alter configurations of Pipelines
Pipelines can be updated by registering a new configuration for the same PipelineId at the Server
```
ProcessingServer.Server.SetupPipeline(pipelineId, config);
```
The current pipeline will process all assigned events from its queue before closing the pipeline. All new events will be processed by the pipeline with the new configuration.