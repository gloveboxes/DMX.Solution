using Newtonsoft.Json;

namespace DMX.Server
{
    public class AutoPlay
    {
        public string autoPlayId { get; set; }
        public string type { get; set; }
        public byte[][] data { get; set; }
    }
}