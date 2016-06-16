using Newtonsoft.Json;

namespace DMX.Server
{
    public class Fixtures
    {
        public int id { get; set; }

        public int numberOfChannels { get; set; }

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
        public byte[] initialChannelMask { get; set; }

    }
}