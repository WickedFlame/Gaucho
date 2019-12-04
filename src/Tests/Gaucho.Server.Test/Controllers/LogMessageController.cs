using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gaucho.Server.Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogMessageController : ControllerBase
    {
        [HttpPost]
        public void Post(LogMessage message)
        {
            var dispatcher = new EventDispatcher();
            dispatcher.Process("LogMessage", message);
        }
    }

    public class LogMessage
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public string Level { get; set; }
    }
}
