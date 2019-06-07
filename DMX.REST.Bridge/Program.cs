using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DMX.REST.Bridge
{
    class Program
    {
        static MqttClient client;
        static AutoResetEvent signalrDisconnectedEvent = new AutoResetEvent(false);
        static string baseUrl = "https://dmx-controller.azurewebsites.net/api";
        static string mqttBroker = "localhost";

        static void Main(string[] args)
        {
            while (true)
            {
                InitMQTT();
                InitSignalR();

                signalrDisconnectedEvent.WaitOne();
                client.Disconnect();
            }
        }

        private static void InitMQTT()
        {
            client = null;
            while (true)
            {
                try
                {
                    client = new MqttClient(mqttBroker);
                    client.Connect("DMXBridge");

                    if (client != null && client.IsConnected)
                    {
                        break;
                    }
                    else {
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to MQTT Server {ex.Message}");
                    client = null;
                    Thread.Sleep(5000);                    
                }
            }
        }

        public static async Task<int> InitSignalR()
        {
            var uri = new Uri(baseUrl);
            MqttClient _client = client;

            var connection = new HubConnectionBuilder()
                .WithUrl(uri)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddConsole();
                }).Build();


            // Set up handler
            connection.On<string>("newMessage", Command);

            connection.Closed += e =>
            {
                Console.WriteLine("Connection closed...");

                signalrDisconnectedEvent.Set();
                return Task.CompletedTask;
            };
            await ConnectAsync(connection);

            Console.WriteLine("Connected to {0}", uri);

            return 0;
        }

        private static void Command(String cmd)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(cmd);
            client.Publish("dmx/data", bytes);
        }

        private static async Task<bool> ConnectAsync(HubConnection connection)
        {
            while (true)
            {
                try
                {
                    await connection.StartAsync();
                    return true;
                }
                catch (ObjectDisposedException)
                {
                    // Client side killed the connection
                    return false;
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to connect, trying again in 5000(ms)");

                    await Task.Delay(5000);
                }
            }
        }
    }
}
