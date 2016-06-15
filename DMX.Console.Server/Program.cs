using DMX.Console.Simple;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using System.Linq;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DMX.Server
{
    class Program
    {
        static DmxController dmx;
        static MqttClient client;

        static Configuration config = new Configuration();
        static Instrumentation instrumentation = new Instrumentation();

        static FixtureData fixtureData;

        static Colour[] RandomColours = new Colour[]
        {
            //new Colour(255, 0,0),
            //new Colour(0, 255, 0),
            //new Colour(0,0,255)
          
            new Colour(90, 195,0), // lime
            new Colour(172, 0, 164), // purple
            new Colour(0, 185, 137), //green
            new Colour(192, 60, 0), // copper
            new Colour(254, 29, 0), // red
            new Colour(0, 249, 253), // teal
            new Colour(255, 107, 0), // orange
            new Colour(128, 0, 2), // crimson
            new Colour(30,0,255), // royal blue
            new Colour(215, 0, 103), // pink
            new Colour(255, 125,0), // gold
            new Colour(141, 0, 253), // violet
        };

        static uint NextColour;
        static Random rndColour = new Random();


        static AutoResetEvent dmxUpdateEvent = new AutoResetEvent(false);
        static Thread dmxUpdateThread = new Thread(new ThreadStart(DmxUpdate));

        static void Main(string[] args)
        {
            if (!config.ParseArgs(args)) { return; }

            if (!config.LoadFixtures()) { return; };

            dmx = new DmxController(0, config.Channels, instrumentation);

            try
            {
                config.Log("Opening DMX Controller");
                dmx.Open();

                config.Log("Initialising DMX Controller");
                dmx.InitializeOpenDMX();
            }
            catch (Exception ex)
            {
                config.Log("Problem opening DMX Controller: " + ex.Message);
                return;
            }

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
                string json = System.Text.Encoding.UTF8.GetString(e.Message);

                fixtureData = JsonConvert.DeserializeObject<FixtureData>(json);

                if (!string.IsNullOrEmpty(fixtureData.command)) { ProcessCommand(fixtureData.command); }

                if (fixtureData.id == null || fixtureData.data == null) { return; }

                instrumentation.MessagesReceived++;

                for (int id = 0; id < fixtureData.id.Length; id++)
                {
                    var fixture = (from f in config.Fixtures where f.id == fixtureData.id[id] select f).FirstOrDefault();
                    if (fixture != null) { dmx.UpdateChannel(fixture.startChannel, fixture.channels, fixtureData.data); }
                }

                dmxUpdateEvent.Set();
            }
            catch { instrumentation.Exceptions++; }
        }

        static void ProcessCommand(string command)
        {
            switch (command.ToLower())
            {
                case "stats":
                    instrumentation.Publish();
                    break;
                default:
                    break;
            }
        }

        static void RandomColour(Configuration.CycleMode mode)
        {
            Colour colour = null;

            if (mode == Configuration.CycleMode.synced)
            {
                colour = RandomColours[NextColour++ % RandomColours.Length];
            }


            foreach (var fixture in config.Fixtures)
            {
                switch (mode)
                {
                    case Configuration.CycleMode.sequential:
                        colour = RandomColours[NextColour++ % RandomColours.Length];
                        break;
                    case Configuration.CycleMode.random:
                        colour = new Colour((byte)rndColour.Next(0, 255), (byte)rndColour.Next(0, 255), (byte)rndColour.Next(0, 255));
                        break;
                    default:
                        break;
                }

                dmx.UpdateChannel(fixture.startChannel, fixture.channels, fixture.autoPlayData);

                UpdateColour(colour.Red, fixture.redChannels, fixture.startChannel);
                UpdateColour(colour.Green, fixture.greenChannels, fixture.startChannel);
                UpdateColour(colour.Blue, fixture.blueChannels, fixture.startChannel);
            }
        }

        private static void UpdateColour(byte colour, byte[] chns, uint item)
        {
            //   if (fixture.rChns == null) { return; }
            foreach (var chn in chns)
            {
                dmx.UpdateChannel((int)(item + chn - 1), colour);
            }
        }

        static void DmxUpdate()
        {
            while (true)
            {
                Thread.Sleep((int)config.DmxUpdateRateMilliseconds); // sets minimum cadence
                if (config.AutoPlay == 0)
                {
                    dmxUpdateEvent.WaitOne();
                    dmx.DmxUpdate();
                }
                else
                {
                    if (dmxUpdateEvent.WaitOne(new TimeSpan(0, 0, (int)config.AutoPlay), false))
                    {
                        dmx.DmxUpdate();
                    }
                    else
                    {
                        RandomColour(config.AutoPlayCycleMode);
                        dmx.DmxUpdate();
                    }
                }

                instrumentation.DmxSentCount++;
            }
        }
    }
}
