using Gaucho.Server;

namespace Gaucho.Integration.Tests.Handlers
{
	public class HandlerRegistration : Gaucho.Server.HandlerRegistration
	{
		public override void RegisterHandlers(HandlerRegistrationContext context)
		{
			context.Register<CustomInputHandler>("CustomInput");
			context.Register<ConsoleOutputHandler>("ConsoleOutput");
		}
	}
}
