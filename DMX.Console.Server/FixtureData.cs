using Newtonsoft.Json;

namespace DMX.Server
{
    public class FixtureData
    {
       
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint[] id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? red { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? green { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? blue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? white { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string command { get; set; }

    }
}
