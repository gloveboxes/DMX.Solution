using Newtonsoft.Json;

namespace DMX.Console.Simple
{
    public class FixtureDescription
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
        public int channels { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] autoPlayData { get; set; }

    }
}