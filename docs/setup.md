---
title: Setup
layout: "default"
nav_order: 6
---
## Setup
The Server is usualy created in the Startup.cs
```
GlobalConfiguration.Configuration
    .UseProcessingServer(p =>
    {
        var config = new PipelineConfiguration
        {
            Id = "dependency_handler",
            OutputHandlers = new List<HandlerNode>
            {
                new HandlerNode(typeof(DependencyHandler))
            },
            InputHandler = new HandlerNode("CustomInput")
        };
        p.BuildPipeline(server, config);
    })
    .AddLogWriter(new ConsoleLogWriter())
    .UseOptions(new Options
    {
        LogLevel = Diagnostics.LogLevel.Debug,
        ServerName = "Testserver"
    });
```