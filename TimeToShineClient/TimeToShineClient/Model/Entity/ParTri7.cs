using Newtonsoft.Json;
using System.Text;

namespace TimeToShineClient.Model.Entity
{
    class ParTri7 : IFixture
    {
        const int ChannelsPerFixture = 6;
        public byte[] rChns { get; set; }  = new byte[] { 2 };
        public byte[] gChns { get; set; } = new byte[] { 3 };
        public byte[] bChns { get; set; } = new byte[] { 4 };
        public byte[] wChns { get; set; }
        public uint[] dmxChn { get; set; } = new uint[] { 1, 8 };
        public byte[] data { get; } = new byte[ChannelsPerFixture];



        public void SetChannel(int channel, byte value)
        {
            if (channel < 0 || channel >= ChannelsPerFixture) { return; }
            data[channel] = value;
        }

        public void SetRgb(byte red, byte green, byte blue)
        {
            data[0] = 255; // max brightness
            data[1] = red;
            data[2] = green;
            data[3] = blue;
        }


        public bool IsSame(byte red, byte green, byte blue)
        {
            return red == data[1] && green == data[2] && blue == data[3];
        }

        public byte[] ToJson()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}
