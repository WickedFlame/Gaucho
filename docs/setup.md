---
title: Setup
layout: "default"
nav_order: 6
---
## Setup
The Server is usualy created in the Startup.cs and is configured with the Setup of the GlobalConfiguration.  
```
GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
    {
        var reader = new WickedFlame.Yaml.YamlReader();
        var config = reader.Read<PipelineConfiguration>("APILogMessage.yml");
        p.BuildPipeline(config);

        config = reader.Read<PipelineConfiguration>("RecurringJob.yml");
        p.BuildPipeline(config);
    })
    .AddLogWriter(new ConsoleLogWriter())
    .UseOptions(new Options
    {
        LogLevel = Diagnostics.LogLevel.Debug,
        ServerName = "Testserver"
    }));
```