using DMX.Client.Fixtures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace DMX.Client
{
    class Program
    {

        const string MqttBroker = "localhost";
        const string MqttTopic = "dmx/data/";
        static MqttClient client = new MqttClient(MqttBroker);

        static IFixture fixture = new ParTri7();
        static Random rndColour = new Random();

        static void Main(string[] args)
        {
            while (true)
            {
                PublishColour();

                Thread.Sleep(1);
            }
        }

        static void PublishColour()
        {
            if (client == null || !client.IsConnected)
            {
                try
                {
                    client.Connect(Guid.NewGuid().ToString().Substring(0, 20));
                }
                catch { }
            }
            if (!client.IsConnected) { return; }


            fixture.SetRgb((byte)rndColour.Next(0, 255), (byte)rndColour.Next(0, 255), (byte)rndColour.Next(0, 255));
            //colour.Red = 0;
            //colour.Green = (byte)rndColour.Next(0, 255);
            //colour.Blue = (byte)rndColour.Next(0, 255);
            //colour.White = (byte)rndColour.Next(0, 255);

            //colour.Red = 0;
            //colour.Green = 146;
            //colour.Blue = 148;
            //colour.White = 149;

            // fixture.id = new uint[] { 1 };

            //var j = fixture.ToJson();

            //string json = System.Text.Encoding.UTF8.GetString(j);

            //fixture = JsonConvert.DeserializeObject<SL3456>(json);


            client.Publish($"{MqttTopic}dmx01", fixture.ToJson());

        }
    }
}
