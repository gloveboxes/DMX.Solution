using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace DMX.REST.Function
{
    public static class Controller
    {

        [FunctionName("Command")]
        public static Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [SignalR(HubName = "DMX")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {

            string command = req.Query["name"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            command = command ?? data?.name;

            if (command == null) { return null; }

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { command }
                });
        }

        [FunctionName("Red")]
        public static Task Red(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [SignalR(HubName = "DMX")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {

            string command = "{\"id\":[1,2,3], \"red\":255, \"green\":0, \"blue\":0}";

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { command }
                });
        }

        [FunctionName("Green")]
        public static Task Green(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [SignalR(HubName = "DMX")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            string command = "{\"id\":[1,2,3], \"red\":0, \"green\":255, \"blue\":0}";

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { command }
                });
        }

        [FunctionName("Blue")]
        public static Task Blue(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [SignalR(HubName = "DMX")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            string command = "{\"id\":[1,2,3], \"red\":0, \"green\":0, \"blue\":255}";

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { command }
                });
        }

        [FunctionName("Off")]
        public static Task Off(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [SignalR(HubName = "DMX")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            string command = "{\"id\":[1,2,3], \"red\":0, \"green\":0, \"blue\":0}";

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { command }
                });
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
            [SignalRConnectionInfo(HubName = "DMX")]SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}
