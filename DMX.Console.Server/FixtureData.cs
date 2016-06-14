using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMX.Server
{
    public class FixtureData
    {
       
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint[] id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string command { get; set; }

    }
}
