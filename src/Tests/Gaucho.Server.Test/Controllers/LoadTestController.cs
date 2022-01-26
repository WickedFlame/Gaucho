using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MeasureMap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Gaucho.Server.Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoadTestController : ControllerBase
    {
        [HttpPost]
        public async void Post(int iterations = 10, int threads = 4)
        {
            var message = new LogMessage
            {
                Level = "Info",
                Message = "Some message",
                Title = "Loadtest"
            };
            const string url = "https://localhost:44326/LogMessage";


            ProfilerSession.StartSession()
                .SetIterations(iterations)
                .SetThreads(threads)
                .Task(() =>
                {
                    RunTask(url, message);
                }).RunSession();
        }

        private static async void RunTask(string url, LogMessage message)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    using (var httpContent = CreateHttpContent(message))
                    {
                        request.Content = httpContent;

                        using (var result = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None))
                        {
                        }
                    }
                }
            }
        }

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            {
                using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
                {
                    var js = new JsonSerializer();
                    js.Serialize(jtw, value);
                    jtw.Flush();
                }
            }
        }
    }
}
