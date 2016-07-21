using Newtonsoft.Json;
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

        public bool SetChannel(int channel, byte value)  // returns true if new values are the same as old
        {
            if (channel < 1 || channel > ChannelsPerFixture) { return false; }

            if(IsSame(channel,value)) { return true; }

            data[channel - 1] = value; // map 1 based channel IDs to zero based arrays
            currentChannel = channel;

            return false;
        }

        public bool SetRgb(byte red, byte green, byte blue, byte white = 0)
        {
            throw new NotImplementedException();
        }

        public byte[] ToJson()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }

        private bool IsSame(byte red, byte green, byte blue, byte white = 0)
        {
            throw new NotImplementedException();
        }

        private bool IsSame(int channel, byte value)
        {
            if (channel < 1 || channel > ChannelsPerFixture) { return false; }
            return currentChannel == channel && data[channel - 1] == value; // map 1 based channel IDs to zero based arrays
        }
    }
}
