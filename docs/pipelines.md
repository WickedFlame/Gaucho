---
title: Pipelines
layout: "default"
nav_order: 3
---
## Pipelines

A pipeline is setup of a InputHandler, 1-n OutputHandlers and a set of filter definitions.  
InputHandlers take a object in, do some datamanipulation and pass this to the OutpuHandlers.  
OutputHandlers dispach the updated data to the desired storage  
  
Each pipeline runs its own queue in dedicated workerthreads.  
Each pipeline owns and runs a dedicated threadpool that process the events in the queue.  
Depending on the size of the queue, the pipeline starts a number of threads to process the events in the queue.

### Alter configurations of Pipelines
Pipelines can be updated by registering a new configuration for the same PipelineId at the Server
```
ProcessingServer.Server.SetupPipeline(pipelineId, config);
```
The current pipeline will process all assigned events from its queue before closing the pipeline. All new events will be processed by the pipeline with the new configuration.