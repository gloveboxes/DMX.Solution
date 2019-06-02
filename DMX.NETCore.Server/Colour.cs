using System;

namespace DMX.Server
{
    public class Colour
    {
        public static double Intensity { get; set; } = 1;

        private byte _red, _green, _blue, _white;

        public byte Red { get { return SetLevel(_red); } set { _red = value; } }
        public byte Green { get { return SetLevel(_green); } set { _green = value; } }
        public byte Blue { get { return SetLevel(_blue); } set { _blue = value; } }
        public byte White { get { return SetLevel(_white); } set { _white = value; } }

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

        private byte SetLevel(byte value)
        {
            return (byte)(value * Intensity);
        }
    }
}
