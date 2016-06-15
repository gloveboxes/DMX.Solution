using Newtonsoft.Json;

namespace DMX.Server
{
    public class FixtureDefinition
    {
        public int id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] redChannels { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] greenChannels { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] blueChannels { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] whiteChannels { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint startChannel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] initialise { get; set; }
    }
}