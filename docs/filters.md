---
title: Filters
layout: "default"
nav_order: 7
---
## Filters
Filters are used to define the Properties that are passed to the Handler or can be used to format and parse values of the object.  
  
Filters are provided in different flavours. 
- DataFilters
- Formatters
  
To access Formatters in the Handlers, the handler has to request a IEventDataConverter as a parameter.  

### DataFilters: 
DataFilters create a new EventData container and convert the properties to the desired destination.

| Pointer | Syntax | Description |
|----|----|----|
| -> | property -> destination | mapps property to destination property in the new table |
|  | property | maps property to the new table |

### Formatters: 
Formatters can be used to format the EventData to a string output in the given format.

The syntax for formatting uses the following format:
- ${PROP}
- ${PROP:STRINGFORMAT}

The Formatter ${Date:yyyy-MM-ddTHH:mm:ss.sssZ} takes in a DateTime from the Date property and formats it to a ISO 8601 date string format

| Pointer | Syntax | Description |
|----|----|----|
| <- | property <- ${propertyA}_${propertyB} | Creates a string value formatted with the value of propertyA and propertyB with a _ between |

#### Predefined Formatters
There are some predefined Formatters that take a configuration and produce a string in a certain format.

| Key | Pointer | Syntax | Description |
|----|----|----|----|
| json | <- | json <- [property, property2 -> destination2] | Creates a json containing property and destination2 |

### Examples
#### Simple property
Add a property from the eventdata to the output data.  
```
"Message",
```
Only the properties that are defined in the filters are passed to the handlers.  
If no filters are provided, all properties from the eventdata are passed on.  

#### Alter the property
Use a pointer -> to push a property to another propertyname.  
```
"Level -> lvl",
```
This takes the property Level and outputs a property named lvl.  

#### Formated properties
The property Id is created with a formatted value consisting of the property lvl the value "_error_" followed by the property Message.  
```
"Id <- ${lvl}_error_${Message}"
```

#### String formatters
Format a DateTime to a ISO 8601 format  
```
"DateString <- ${Date:yyyy-MM-ddTHH:mm:ss.sssZ}"
```
This takes in the property Date and outputs the date to a new property DateString as a formatted string  
