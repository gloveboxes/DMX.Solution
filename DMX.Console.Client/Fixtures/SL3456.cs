﻿using Newtonsoft.Json;
using System.Text;

namespace DMX.Client.Fixtures
{
    class SL3456 : IFixture
    {
        const int ChannelsPerFixture = 6;

        public uint[] id { get; set; } = new uint[] { 1, 7 };
        public byte[] data { get; } = new byte[ChannelsPerFixture];



        public void SetChannel(int channel, byte value)
        {
            if (channel < 0 || channel >= ChannelsPerFixture) { return; }
            data[channel] = value;
        }

        public void SetRgb(byte red, byte green, byte blue)
        {
            data[1] = red;
            data[2] = green;
            data[3] = blue;
        }

        public byte[] ToJson()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}
