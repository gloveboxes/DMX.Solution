using Newtonsoft.Json;
using System;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DMX.Server
{
    class Program
    {
        static MqttClient client;

        static Configuration config = new Configuration();
        static Instrumentation instrumentation = new Instrumentation(config);

        static FixtureMessage fixtureMsg;
        static Universe universe = new Universe(config, 0);

        static AutoResetEvent dmxUpdateEvent = new AutoResetEvent(false);
        static Thread dmxUpdateThread = new Thread(new ThreadStart(DmxUpdate));

        static void Main(string[] args)
        {
            if (!config.LoadConfig()) { return; }
            if (!universe.InitialiseUniverse()) { return; }

            dmxUpdateThread.Start();

            while (true)
            {
                if (client == null || !client.IsConnected)
                {
                    try
                    {
                        config.Log($"Mqtt Broker: {config.MqttBroker}");
                        client = new MqttClient(config.MqttBroker);
                        client.Connect("DMXController01");

                        if (client != null && client.IsConnected)
                        {
                            config.Log("Mqtt Connected");
                            instrumentation.SetMqttClient(client);
                            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
                            client.Subscribe(new string[] { config.MqttDataTopic }, new byte[] { 0 });
                        }
                        else { config.Log("Mqtt not connected"); }
                    }
                    catch (Exception ex)
                    {
                        config.Log("Mqtt not connected");
                        instrumentation.ExceptionMessage = ex.Message;
                        instrumentation.Exceptions++;
                        client = null;
                        instrumentation.SetMqttClient(client);
                    }
                }
                Thread.Sleep(5000);  // every 5 seconds check that the mqtt client is still connected
            }
        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                instrumentation.MessagesReceived++;

                string json = System.Text.Encoding.UTF8.GetString(e.Message);

                fixtureMsg = JsonConvert.DeserializeObject<FixtureMessage>(json);

                if (!string.IsNullOrEmpty(fixtureMsg.command))
                {
                    ProcessCommand(fixtureMsg.command);
                    return;
                }

                if (fixtureMsg.id == null) { return; }

                if (fixtureMsg.data == null)
                {
                    universe.UpdateFixtureChannels(fixtureMsg);
                }
                else
                {
                    universe.UpdateFixtureData(fixtureMsg);
                }

                dmxUpdateEvent.Set();
            }
            catch (Exception ex)
            {
                //   config.Log(ex.Message);
                instrumentation.Exceptions++;
            }
        }

        static void ProcessCommand(string command)
        {
            switch (command.ToLower())
            {
                case "stats":
                    instrumentation.Publish();
                    break;
                case "auto":
                    if (config.AutoPlayEnabled)
                    {
                        config.AutoPlayEnabled = false;
                    }
                    else
                    {
                        config.AutoPlayEnabled = true;
                        dmxUpdateEvent.Set();
                    }
                    break;
                case "autoplayon":
                    config.AutoPlayEnabled = true;
                    dmxUpdateEvent.Set();
                    break;
                case "autoplayoff":
                    config.AutoPlayEnabled = false;
                    break;
                default:
                    break;
            }
        }

        static void DmxUpdate()
        {
            while (true)
            {
                Thread.Sleep((int)config.DmxUpdateRateMilliseconds); // sets minimum cadence
                if (!config.AutoPlayEnabled)
                {
                    dmxUpdateEvent.WaitOne();
                    universe.DmxUpdate();
                }
                else
                {
                    if (dmxUpdateEvent.WaitOne(new TimeSpan(0, 0, (int)config.AutoPlayTimeout), false))
                    {
                        universe.DmxUpdate();
                    }
                    else
                    {
                        universe.RandomColour(config.AutoPlayCycleMode);
                        universe.DmxUpdate();
                    }
                }

                instrumentation.DmxSentCount++;
            }
        }
    }
}
