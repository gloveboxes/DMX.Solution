using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace TimeToShineClient.Model.Entity
{
    public class SpecialColor
    {
        public SpecialColor(Color color, byte specialCode)
        {
            Color = color;
            SpecialCode = specialCode;
        }

        public Color Color { get; }

        public byte SpecialCode { get; }

        public SolidColorBrush Brush => new SolidColorBrush(Color);
    }
}
