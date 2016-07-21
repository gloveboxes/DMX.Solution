using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeToShineClient.Model.Entity
{
    class GenericRGBW : IFixture
    {
        const int ChannelsPerFixture = 6;
        public uint[] id { get; set; } = new uint[] { 1, 6 };
        public byte[] data { get; }

        public byte Red {get;set;}
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte White { get; set; }


        public bool SetChannel(int channel, byte value)
        {
            throw new NotImplementedException();
        }

        public bool SetRgb(byte red, byte green, byte blue, byte white = 0)
        {
            if (IsSame(red, green, blue, white)){ return true; }

            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.White = white;

            return false;
        }

        public byte[] ToJson()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }

        private bool IsSame(int channel, byte value)
        {
            throw new NotImplementedException();
        }

        private bool IsSame(byte red, byte green, byte blue, byte white = 0)
        {
            return red == this.Red && green == this.Green && blue == this.Blue && this.White == white;
        }
    }
}
