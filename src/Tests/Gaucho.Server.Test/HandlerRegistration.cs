using Gaucho.Server.Test.Controllers;
using Gaucho.Server.Test.Handlers;

namespace Gaucho.Server.Test
{
    public class HandlerRegistration : Gaucho.HandlerRegistration
    {
        public override void RegisterHandlers(HandlerRegistrationContext context)
        {
            context.Register<GenericInputHandler<LogMessage>>("LogMessage");

            context.Register<ConsoleOutputHandler>("Console");
        }
    }
}
