---
title: Setup
layout: "default"
nav_order: 6
---
## Setup
There are two kinds of possible ways to use Gaucho.
- Using the global Server from ProcessingServer.Server
- As a own instance of ProcessingServer
  
The Configuration is setup and configured with the GlobalConfiguration class.  
A call to GlobalConfiguration.Setup(c => { }) resets all current settings from the GlobalConfiguration. The running and configured pipelines on the Server are not reset and stay running.  
The setup is usualy done in the Startup.cs

### UseProcessingServer
The extension method UseProcessingServer on the GlobalConfiguration allows to setup the ProcessingServer.
```
GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
    {
        // create the configuration of a pipeline
        var config = new PipelineConfiguration
        {
            Id = "pipeline1",
            OutputHandlers = new List<HandlerNode>
            {
                new HandlerNode(typeof(ConsoleOutputHandler)),
                new HandlerNode(typeof(SqlOuputHandler))
                {
                    Filters = new List<string>
                    {
                        "Level -> lvl",
                        "Message",
                        "Id <- ${lvl}_error_${Message}"
                    }
                }
            },
            InputHandler = new HandlerNode(typeof(InputHandler<LogMessage>))
        };

        // add the pipeline to the default server
        p.BuildPipeline(config);
    }));
```
This sample configuration takes in objects of type LogMessage and passes these to a ConsoleOutputHandler and a SqlOutputHandler.  
The Data that is passed to the SqlOutputHandler is converted based on the filters that are configured.  
The pipeline is added to the Staic instance of the ProcessingServer.  
  
BuildPipeline alowes to pass in a custom instance of a ProcessingServer without having to use the global Server.  
```
var server = new ProcessingServer();
GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
    {
        // create the configuration of a pipeline
        var config = new PipelineConfiguration
        {
            ...
        };

        // add the pipeline to the default server
        p.BuildPipeline(server, config);
    }));
```
### UseProcessingServer
```
GlobalConfiguration.Setup(c => c.AddLogWriter(new ConsoleLogWriter()));
```
  
### UseOptions
```
GlobalConfiguration.Setup(c => c.UseOptions(new Options
    {
        LogLevel = Diagnostics.LogLevel.Debug,
        ServerName = "Testserver"
    }));
```
