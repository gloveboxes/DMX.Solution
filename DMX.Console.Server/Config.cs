using Newtonsoft.Json;

namespace DMX.Server
{
    public class Config
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? dmxRefreshRateMilliseconds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? autoPlayTimeout { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? autoPlayEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string autoPlayCycleMode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? autoPlayIntensity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mqttBroker { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mqttDataTopic { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mqttStatusTopic { get; set; }
    }
}