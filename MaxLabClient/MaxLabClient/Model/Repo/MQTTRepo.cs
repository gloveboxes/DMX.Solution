using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
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

        MqttClient client;
        IFixture wristband = new Wristband();
        IFixture rgbwLight = new GenericRGBW();

        const int publishCycleTime = 100;
        AutoResetEvent publishEvent = new AutoResetEvent(false);

        public enum AutoPlayMode
        {
            AutoplayOn,
            AutoplayOff
        }

        string command = null;


        public MQTTService(IConfigService configService)
        {
            _configService = configService;

            Task.Run(new Action(_publish));
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

                    if (command != null) // set dmx server auto play mode
                    {
                        string jsonCommand = "{\"command\":\"" + command + "\"}";
                        var r = client.Publish($"{_configService.MqttTopic}{_configService.DMXChannel}", Encoding.UTF8.GetBytes(jsonCommand));
                        command = null;
                        continue;
                    }

                    //  latestColour.MsgId = sentCount++;
                    wristband.id = new uint[] { 1 }; 

                    var json = wristband.ToJson();

                    new DebugMessage($"Sending: Topic: {_configService.MqttTopic}, dmx: {_configService.DMXChannel}, Light Id: {Encoding.ASCII.GetString(json)}").Send();

                    var result = client.Publish($"{_configService.MqttTopic}{_configService.DMXChannel}", json);


                    rgbwLight.id = _configService.LightIdArray;
                    json = rgbwLight.ToJson();

                    new DebugMessage($"Sending: Topic: {_configService.MqttTopic}, dmx: {_configService.DMXChannel}, Light Id: {Encoding.ASCII.GetString(json)}").Send();               

                    result = client.Publish($"{_configService.MqttTopic}{_configService.DMXChannel}", json);

                    new DebugMessage($"Send result: {result}").Send();

                }
                catch (Exception ex)
                {
                    new DebugMessage($"Failed to send to client {ex.Message}").Send();
                }
            }
        }

        private bool _isConnected => client != null && client.IsConnected;

        public void SetAutoPlayMode(AutoPlayMode mode)
        {
            command = mode.ToString().ToLower();
            publishEvent.Set();
        }

        public void PublishSpecial(byte b, int channel, Color color)
        {
            if (channel == 9) { SetAutoPlayMode(AutoPlayMode.AutoplayOn); return; }
            if (channel == 8) { SetAutoPlayMode(AutoPlayMode.AutoplayOff); return; }

            if (wristband.IsSame(channel, b)) { return; }

            wristband.SetChannel(channel, b);
            rgbwLight.SetRgb(color.R, color.G, color.B);

            publishEvent.Set();
        }


        public void Publish(Colour colour)
        {
            if (wristband.IsSame(colour.Red, colour.Green, colour.Blue)) {return; }

            wristband.SetRgb(colour.Red, colour.Green, colour.Blue);

            publishEvent.Set();

            return;
        }
    }
}