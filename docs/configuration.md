---
title: Configuration
layout: "default"
nav_order: 4
---
## Configuration

The Configuration as yml.
```
Id: EventLog
InputHandler:
  Name: LogMessage
#  Type: Gaucho.Server.Test.Handlers.GenericInputHandler`1[[Gaucho.Server.Test.Controllers.LogMessage, Gaucho.Server.Test]], Gaucho.Server.Test
  Arguments:
    Index: 1
  Filters:
    - Message
    - Level
    - WebPortalId


OutputHandlers:
  - Type: Gaucho.Server.Test.Handlers.ElasticsearchOutputHandler, Gaucho.Server.Test
    Filters:
      - Message -> msg
      - Level -> lvl
	    - WebPortalId
      - 'format <- {"level":"${lvl}","message":"${msg}"}'
	    - 'json <- [lvl,msg -> message]'
```

### Arguments
Arguments represent constant values that are passed as a Collection to the Handlers.  
To acces Arguments in the Handlers, the handler has to request a ConfiguredArgumentsCollection as a parameter.  


### Filters
Filters are used to define or format the properties that are passed to the handlers.  
See [filters.html](Filters) for a detailed description.
