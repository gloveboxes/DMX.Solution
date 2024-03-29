﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeToShineClient.Model.Entity
{
    class Wristband : IFixture
    {
        const int ChannelsPerFixture = 9;
        public uint[] id { get; set; } = new uint[] { 1 };
        public byte[] data { get; } = new byte[ChannelsPerFixture];

        private int currentChannel = 0;

        public void SetChannel(int channel, byte value)
        {
            if (value == 0 || value == 1)  // special case for black then zero out all as this is off for all channels
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = 1;
                }

                return;
            }


            if (channel < 1 || channel > ChannelsPerFixture) { return; }
            data[channel - 1] = value; // map 1 based channel IDs to zero based arrays
            currentChannel = channel;
        }

        public void SetRgb(byte red, byte green, byte blue, byte white = 0)
        {
        }

        public byte[] ToJson()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }

        public bool IsSame(byte red, byte green, byte blue, byte white = 0)
        {
            return red == data[1] && green == data[2] && blue == data[3];
        }

        public bool IsSame(int channel, byte value)
        {
            if (channel < 1 || channel > ChannelsPerFixture) { return false; }
            return currentChannel == channel && data[channel - 1] == value; // map 1 based channel IDs to zero based arrays
        }
    }
}
