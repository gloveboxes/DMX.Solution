﻿using Newtonsoft.Json;
using System.Text;
using System;

namespace TimeToShineClient.Model.Entity
{
    class SL3456 : IFixture
    {
        const int ChannelsPerFixture = 6;
        public uint[] id { get; set; } = new uint[] { 1, 7 };
        public byte[] data { get; } = new byte[ChannelsPerFixture];

        public void SetChannel(int channel, byte value)
        {
            if (channel < 1 || channel > ChannelsPerFixture) { return; }
            data[channel - 1] = value; // map 1 based channel IDs to zero based arrays
        }

        public void SetRgb(byte red, byte green, byte blue, byte white = 0)
        {
            data[1] = red;
            data[2] = green;
            data[3] = blue;
        }

        public byte[] ToJson()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }

        public bool IsSame(int channel, byte value)
        {
            throw new NotImplementedException();
        }

        public bool IsSame(byte red, byte green, byte blue, byte white = 0)
        {
            return red == data[1] && green == data[2] && blue == data[3];
        }
    }
}
