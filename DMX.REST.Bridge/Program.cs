using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DMX.REST.Bridge
{
    class Program
    {
        private const string _mqttBroker = "localhost";

        private static IMqttClient _mqttClient;

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Expecting SignalR Function URI as command line argument");
            }
            else
            {
                Console.WriteLine(args[0]);
            }

            Uri signalrFunctionUri = new Uri(args[0]);

            // Set up MQTT COnnection to DMX Server
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_mqttBroker)
                .WithCleanSession()
                .Build();

            _mqttClient.Connected += (s, e) =>
            {
                Console.WriteLine("### MQTT DMX Server Connected ###");
            };

            _mqttClient.Disconnected += async (s, e) =>
            {
                Console.WriteLine("### Disconnected from MQTT DMX Server ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await _mqttClient.ConnectAsync(options);
                    Console.WriteLine("### Reconnected to MQTT DMX Server ### ");
                }
                catch
                {
                    Console.WriteLine("### Reconnecting to MQTT DMX Server Failed ###");
                }
            };

            // Set up SignalR Connection

            var signalrConnection = new HubConnectionBuilder()
                 .WithUrl(signalrFunctionUri)
                 .ConfigureLogging(logging =>
                 {
                     logging.SetMinimumLevel(LogLevel.Information);
                     logging.AddConsole();
                 }).Build();

            signalrConnection.On<string>("newMessage", CommandAsync);

            signalrConnection.Closed += async error =>
            {
                Console.WriteLine("### SignalR Connection closed... ###");
                // await signalrConnection.StartAsync();
                try
                {
                    await Task.Delay(new Random().Next(0,5) * 1000);
                    await signalrConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"### SignalR Connection Exception: {ex.Message}");
                }
                Console.WriteLine("### Connected to SignalR... ###");
            };

            try
            {
                await _mqttClient.ConnectAsync(options);
                await signalrConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Thread.Sleep(Timeout.Infinite);
        }

        private static async Task CommandAsync(String cmd)
        {
            byte[] data = Encoding.ASCII.GetBytes(cmd);
            var msg = new MqttApplicationMessage();
            msg.Topic = "dmx/data";
            msg.Payload = data;
            await _mqttClient.PublishAsync(msg);
        }
    }
}
