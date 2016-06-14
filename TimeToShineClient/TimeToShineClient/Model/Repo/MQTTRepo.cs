using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeToShineClient.Model.Contract;
using TimeToShineClient.Model.Entity;
using TimeToShineClient.Model.Messages;
using uPLibrary.Networking.M2Mqtt;
using XamlingCore.Portable.Messages.XamlingMessenger;

namespace TimeToShineClient.Model.Repo
{
    public class MQTTService : IMQTTService
    {
        private readonly IConfigService _configService;
        //const string MqttBroker = "27.33.31.102";
        private string _mqttTopic = "dmx/data/";
        private string _dmxChannel = "1";
        MqttClient client;
        //    Colour latestColour = new Colour();
        IFixture latestColour = new ParTri7();

        const int publishCycleTime = 100;
        AutoResetEvent publishEvent = new AutoResetEvent(false);


        public MQTTService(IConfigService configService)
        {
            _configService = configService;

            Task.Run(new Action(_publish));
        }

        void _config()
        {
            var dChannel = _configService.DMXChannel;

            if (string.IsNullOrWhiteSpace(dChannel))
            {
                _dmxChannel = "1";
            }
            else
            {
                _dmxChannel = dChannel;
            }

            _mqttTopic = _configService.MqttTopic;

            if (_mqttTopic == null)
            {
                _configService.MqttTopic = "msstore/vivid/light/";
                _mqttTopic = _configService.MqttTopic;
            }
        }


        async void _publish()  //run async
        {
            while (true)
            {
                await Task.Delay(publishCycleTime);
                publishEvent.WaitOne();

                if (client == null || !client.IsConnected)
                {
                    try
                    {
                        if (client == null)
                        {
                            new DebugMessage($"Creating new client from null - broker {_configService.MqttBroker}").Send();
                            client = new MqttClient(_configService.MqttBroker);
                        }

                        new DebugMessage("Connecting to broker").Send();

                        client.Connect(Guid.NewGuid().ToString().Substring(0, 20));

                    }
                    catch (Exception ex)
                    {
                        new DebugMessage($"Failed connection to broker: exception: {ex.Message}").Send();
                    }
                }

                if (!_isConnected) {
                    continue;
                }


                try
                {
                  //  latestColour.MsgId = sentCount++;
                    latestColour.id = _configService.LightIdArray;


                    var json = latestColour.ToJson();

                    new DebugMessage($"Sending: Topic: {_mqttTopic}, dmx: {_dmxChannel}, Light Id: {Encoding.ASCII.GetString(json)}").Send();

                    var result = client.Publish($"{_mqttTopic}{_dmxChannel}", json);

                    new DebugMessage($"Send result: {result}").Send();

                }
                catch (Exception ex)
                {
                    new DebugMessage($"Failed to send to client {ex.Message}").Send();
                }
            }
        }

        private bool _isConnected => client != null && client.IsConnected;


        public void Publish(Colour colour)
        {
            if (latestColour.IsSame(colour.Red, colour.Green, colour.Blue)) {return; }

            latestColour.SetRgb(colour.Red, colour.Green, colour.Blue);

            publishEvent.Set();

            return;
        }
    }
}