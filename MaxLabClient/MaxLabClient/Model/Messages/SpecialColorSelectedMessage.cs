using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeToShineClient.Model.Entity;
using XamlingCore.Portable.Messages.XamlingMessenger;

namespace TimeToShineClient.Model.Messages
{
    public class SpecialColorSelectedMessage : XMessage
    {
        public SpecialColorSelectedMessage(SpecialColor color)
        {
            Color = color;
        }

        public SpecialColor Color { get; }
    }
}
