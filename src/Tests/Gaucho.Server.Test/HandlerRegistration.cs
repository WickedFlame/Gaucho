using Gaucho.Server.Test.Handlers;

namespace Gaucho.Server.Test
{
    public class HandlerRegistration : Gaucho.HandlerRegistration
    {
        public override void RegisterHandlers(HandlerRegistrationContext context)
        {
            context.Register<LogMessageInputHandler>("LogMessage");

            context.Register<ConsoleOutputHandler>("Console");
        }
    }
}
