using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamlingCore.Portable.Messages.XamlingMessenger;

namespace TimeToShineClient.Model.Messages
{
    public class DebugMessage : XMessage
    {
        public DebugMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
