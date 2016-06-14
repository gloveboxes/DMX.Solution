using System.Diagnostics;

namespace DMX.Console.Simple
{
    public class SysControl
    {
        ProcessStartInfo info = new ProcessStartInfo();

        public void Control(int ctrl)
        {
            Actions a = (Actions)ctrl;
            switch (a)
            {
                case Actions.None:
                    break;
                case Actions.Halt:
                    Halt();
                    break;
                case Actions.Reboot:
                    Reboot();
                    break;
                default:
                    break;
            }
        }

        enum Actions
        {
            None = 0,
            Halt = 1,
            Reboot = 2
        }
        void Halt()
        {
            info.FileName = "sudo";
            info.Arguments = "halt";
            Process.Start(info);
        }

        void Reboot()
        {
            info.FileName = "sudo";
            info.Arguments = "reboot";
            Process.Start(info);
        }
    }
}
