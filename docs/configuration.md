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


### Filters
DataFilters: 

| Pointer | Syntax | Description |
|----|----|----|
| -> | property -> destination | mapps property to destination property in the new table |
|  | property | maps property to the new table |

Formatters: 
| Key | Pointer | Syntax | Description |
|----|----|----|----|
| json | <- | json <- [property, property2 -> destination2] | Creates a json containing property and destination2 |
