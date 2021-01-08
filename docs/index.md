---
title: Gaucho
layout: "default"
nav_order: 1
---
# Gaucho

A .NET Message Broker Middleware for handling, converting and delegating log messages

Installation
---

Documentation
---
```
var pipelineId = "DefaultPipeline";

var server = new ProcessingServer();
server.Register(pipelineId, () =>
{
    var pipeline = new EventPipeline();
    pipeline.AddHandler(new ConsoleOutputHandler());

    return pipeline;
});
server.Register(pipelineId, new LogMessageInputHandler());

var client = new EventDispatcher(server);
client.Process(pipelineId, new LogMessage { Message = "InstanceServer" });
```

Defining the Handlers
```
public class LogMessageInputHandler : IInputHandler<LogMessage>
{
    public string PipelineId { get; set; }

    public Event ProcessInput(LogMessage message)
    {
        // transform data
        return new Event(PipelineId, f => f.BuildFrom(data));
    }
}
```

```
public class ConsoleOutputHandler : IOutputHandler
{
    public void Handle(Event @event)
    {
        System.Diagnostics.Trace.WriteLine(data);
    }
}
```