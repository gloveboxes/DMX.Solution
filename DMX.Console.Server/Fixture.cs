using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMX.Server
{
    public class Fixture
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] rChns { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] gChns { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] bChns { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] wChns { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint[] dmxChn { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string command { get; set; }


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
