using Gaucho.Handlers;
using Gaucho.Server.Test.Controllers;
using Gaucho.Server.Test.Handlers;

namespace Gaucho.Server.Test
{
    public class HandlerRegistration : Gaucho.Server.HandlerRegistration
    {
        public override void RegisterHandlers(HandlerRegistrationContext context)
        {
            context.Register<InputHandler<LogMessage>>("LogMessage");

            context.Register<ConsoleOutputHandler>("Console");
        }
    }
}
