using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMX.Console.Simple
{
    public class Configuration
    {
        public enum CycleMode
        {
            synced,
            sequential,
            random
        }

        public string MqttBroker { get; set; } = "localhost";
        public string MqttDataTopic { get; set; } = "msstore/vivid/light/#";
        public uint Channels { get; set; } = 50;
        public uint DmxUpdateRateMilliseconds { get; set; } = 25; // default to 40 Hz
        public uint AutoPlay { get; set; } = 3;
        public CycleMode AutoPlayCycleMode { get; set; } = CycleMode.synced;

        public void Log(string messsage)
        {
            System.Console.WriteLine(messsage);
        }

        public bool ParseArgs(string[] args)
        {
            // eg DMX.Console.Simple.exe numberOfChannels DmxUpdateRate AutoPlaySeconds AutoPlayCycleMode

            uint channels, dmxUpdateRateMilliseconds, autoPlay = 0;
            CycleMode cycleMode;

            //args 0 = number of lights, 1 = Channels Per Light, 2 = Rgb Start Channel, 3 = DMX Update Rate in milliseconds, Auto Play timeout in minutes - all numeric
            if (args == null || args.Length != 4)
            {
                Log($"Defaults: Number of Channels {Channels}, DMX Update Rate (Milliseconds) {DmxUpdateRateMilliseconds}, Auto Play Minutes {AutoPlay}, Cycle Mode {AutoPlayCycleMode.ToString()}");
                return true;
            }


            if (!uint.TryParse(args[0], out channels) || !uint.TryParse(args[1], out dmxUpdateRateMilliseconds) || 
                !uint.TryParse(args[2], out autoPlay) || !Enum.TryParse<CycleMode>(args[3].ToLowerInvariant(), out cycleMode))
            {
                Log($"Invalid arguments passed. Expected\n NumberOfChannels DmxUpdateRateInMilliseconds AutoPlayInSeconds CycleMode(sync squential random)");
                return false;
            }

            if (dmxUpdateRateMilliseconds < 25) { dmxUpdateRateMilliseconds = 25; }

            Channels = channels;
            DmxUpdateRateMilliseconds = dmxUpdateRateMilliseconds;
            AutoPlay = autoPlay;
            AutoPlayCycleMode = cycleMode;

            Log($"\n\nNew defaults: \n\nDMX Channels {Channels},\nDMX Update Rate in milliseconds {DmxUpdateRateMilliseconds},\nAuto Play {AutoPlay} seconds, \nCycle Mode {AutoPlayCycleMode.ToString()}.\n\n");

            return true;
        }


    }
}
