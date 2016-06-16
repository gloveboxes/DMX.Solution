using System;

namespace DMX.Server
{
    public class Colour
    {
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte White;

        public Colour(byte red, Byte green, Byte blue, byte White) : this(red, green, blue)
        {
            this.White = White;
        }

        public Colour(byte red, Byte green, Byte blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }
    }
}
