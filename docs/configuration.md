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
Filters are provided in different flavours. 
- DataFilters
- Formatters

To access Filters in the Handlers, the handler has to request a IEventDataConverter as a parameter.

#### DataFilters: 
DataFilters create a new EventData container and convert the properties to the desired destination.

| Pointer | Syntax | Description |
|----|----|----|
| -> | property -> destination | mapps property to destination property in the new table |
|  | property | maps property to the new table |

#### Formatters: 
Formatters can be used to format the EventData to a string output in the given format.

The syntax for formatting uses the following format:
- ${PROP}
- ${PROP:STRINGFORMAT}

The Formatter ${Date:yyyy-MM-ddTHH:mm:ss.sssZ} takes in a DateTime from the Date property and formats it to a ISO 8601 date string format

| Pointer | Syntax | Description |
|----|----|----|
| <- | property <- ${propertyA}_${propertyB} | Creates a string value formatted with the value of propertyA and propertyB with a _ between |

##### Predefined Formatters
There are some predefined Formatters that take a configuration and produce a string in a certain format.

| Key | Pointer | Syntax | Description |
|----|----|----|----|
| json | <- | json <- [property, property2 -> destination2] | Creates a json containing property and destination2 |
