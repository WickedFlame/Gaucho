using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Gaucho.Test
{
    [TestFixture]
    public class ServerInitializeTests
    {
        [Test]
        public void InitializServerInHandler()
        {
            var handler = new InitialieableHandler();
            var server = new ProcessingServer();

            server.Register(Guid.NewGuid().ToString(), handler);

            Assert.AreSame(server, handler.Server);
        }

        public class InitialieableHandler : IInputHandler, IServerInitialize
        {
            public IProcessingServer Server { get; set; }

            public string PipelineId { get; set; }

            public void Initialize(IProcessingServer server)
            {
                Server = server;
            }
        }
    }
}
