using System.Text;
using Windows.UI;
using Newtonsoft.Json;

namespace TimeToShineClient.Model.Entity
{
    public class Colour
    {
        public int MsgId { get; set; }
        public uint[] LightId { get; set; }
        public byte Red { get; set; } = 0;
        public byte Green { get; set; } = 0;
        public byte Blue { get; set; } = 0;
        public byte White { get; set; } = 0;
        public byte Ctrl { get; set; } = 0;

        public byte[] ToJson()
        {
            MsgId++;
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }

        public Color ToColor()
        {
            return new Color
            {
                R = Red,
                G = Green,
                B = Blue
            };
        }

        public static Colour FromColor(Color c)
        {
            return new Colour
            {
                Red = c.R,
                Green = c.G,
                Blue = c.B
            };
        }
    }
}
