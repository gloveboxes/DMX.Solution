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

        static List<Fixture> fixtures = new List<Fixture>();
        static Fixture fixture;
        static object FixturesLock = new object();

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

            dmx = new DmxController(0, config.Channels, instrumentation);

            config.Log("Opening DMX Controller");
            dmx.Open();

            config.Log("Initialising DMX Controller");
            dmx.InitializeOpenDMX();

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

                fixture = JsonConvert.DeserializeObject<Fixture>(json);

                if (!string.IsNullOrEmpty(fixture.command)) { ProcessCommand(fixture.command); }

                if (fixture.dmxChn == null || fixture.data == null) { return; }

                instrumentation.MessagesReceived++;

                uint[] tempDmxChn = new uint[fixture.dmxChn.Length];

                Array.Copy(fixture.dmxChn, tempDmxChn, tempDmxChn.Length);

                for (int id = 0; id < tempDmxChn.Length; id++)
                {
                    if ((from f in fixtures where f.dmxChn.Contains<uint>(tempDmxChn[id]) select f).FirstOrDefault() == null)
                    {
                        lock (FixturesLock)  // ensure access to fixtures collection is thread safe
                        {
                            Fixture newFixture = (Fixture)fixture.Clone();
                            newFixture.dmxChn = new uint[] { tempDmxChn[id] };
                            fixtures.Add(newFixture);
                        }
                    }

                    dmx.UpdateChannel(tempDmxChn[id], fixture.data);
                }

                dmxUpdateEvent.Set();
            }
            catch { instrumentation.Exceptions++; }
        }

        static void ProcessCommand(string command)
        {
            switch (command.ToUpper())
            {
                case "CLEAR":
                    fixtures.Clear();
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

            lock (FixturesLock)
            {
                foreach (var fixture in fixtures)
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

                    if (fixture.dmxChn == null) { continue; }
                    foreach (var item in fixture.dmxChn)
                    {
                        UpdateColour(colour.Red, fixture.rChns, item);
                        UpdateColour(colour.Green, fixture.gChns, item);
                        UpdateColour(colour.Blue, fixture.bChns, item);
                    }
                }
            }
        }

        private static void UpdateColour(byte colour, byte[] chns, uint item)
        {
            if (fixture.rChns == null) { return; }
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
                if (dmxUpdateEvent.WaitOne(new TimeSpan(0, 0, (int)config.AutoPlay), false))
                {
                    //  System.Console.WriteLine($"Time {DateTime.Now.ToShortTimeString()}, Selected Colour");
                    dmx.DmxUpdate();
                }
                else
                {
                    RandomColour(config.AutoPlayCycleMode);
                    dmx.DmxUpdate();
                }

                instrumentation.DmxSentCount++;

                //System.Console.WriteLine($"Mqtt Messages Recieved: {instrumentation.MessagesReceived}, DMX Messages Sent: {dmxSentCount}");
            }
        }
    }
}
