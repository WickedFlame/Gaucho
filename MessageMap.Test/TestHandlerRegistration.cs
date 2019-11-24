using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMap.Test
{
    public class TestHandlerRegistration : HandlerRegistration
    {
        public override void RegisterHandlers(HandlerRegistrationContext context)
        {
            context.Register<GenericInputHandler<LogMessage>>("GenericLogMessage");
            context.Register<CustomInputHandler>("CustomInput");
            context.Register<LogInputHandler>("LogInput");

            context.Register<ConsoleOutputHandler>("ConsoleOutput");
            context.Register<LogQueueHandler>("LogOutput");
        }
    }
}
