using Newtonsoft.Json;
using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;

namespace DMX.Server
{
    public class Instrumentation
    {

        private ulong messagesReceived;

        private MqttClient client;
        private Configuration config;

        public DateTime StartupTime = DateTime.Now;

        public DateTime Time { get { return DateTime.Now; } }
        public uint DmxSentCount { get; set; }
        public ulong MessagesReceived
        {
            get { return messagesReceived; }
            set
            {
                messagesReceived = value;
                if (messagesReceived % 10000 == 0)
                {
                    Publish();
                }
            }
        }

        public Instrumentation(Configuration config)
        {
            this.config = config;
        }

        public void SetMqttClient(MqttClient client)
        {
            this.client = client;
        }

        public ulong Exceptions { get; set; } = 0;

        private string exceptionMessage;

        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set
            {
                exceptionMessage = value;
                Publish();
            }
        }

        public int MsgId { get; set; }

        private byte[] ToJson()
        {
            MsgId++;
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }

        public void Publish()
        {
            try
            {
                if (client != null && client.IsConnected)
                {
                    client.Publish($"{config.MqttStatusTopic}", this.ToJson());
                }
            }
            catch { Exceptions++; }
        }
    }
}
