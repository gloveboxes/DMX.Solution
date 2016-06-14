using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public string MqttDataTopic { get; set; } = "dmx/data/#";
        public uint Channels { get; set; } = 50;
        public uint DmxUpdateRateMilliseconds { get; set; } = 25; // default to 40 Hz
        public uint AutoPlay { get; set; } = 3;
        public CycleMode AutoPlayCycleMode { get; set; } = CycleMode.synced;
        public List<FixtureDescription> Fixtures { get; set; }

        string FixtureFilename { get; set; }

        public Configuration()
        {
            FixtureFilename = AppDomain.CurrentDomain.BaseDirectory + "fixtures.json";
        }

        public void Log(string messsage)
        {
            System.Console.WriteLine(messsage);
        }

        public bool LoadFixtures()
        {
            try
            {
                Fixtures = JsonConvert.DeserializeObject<List<FixtureDescription>>(File.ReadAllText(FixtureFilename));

                var fixture = (from f in Fixtures orderby f.startChannel descending select f).FirstOrDefault();

                if (fixture != null)
                {
                    Channels = (uint)(fixture.startChannel + fixture.channels);
                    if (Channels > 512) { Channels = 512; }
                }
                Log($"DMX Channels {Channels}\n\n");
            }
            catch (Exception ex)
            {
                Log("Feature Json File: " + ex.Message);
                return false;
            }
            return true;
        }


        public bool ParseArgs(string[] args)
        {
            uint dmxUpdateRateMilliseconds, autoPlay = 0;
            CycleMode cycleMode;
            string fixtureFilename = string.Empty;

            // -c 50 -r 25 -a 5 -s random -f 

            for (int a = 0; a < args.Length; a += 2)
            {
                switch (args[a].ToLower())
                {
                    case "/?":
                    case "?":
                    case "-?":
                        StringBuilder message = new StringBuilder();
                        message.Append("Usage:\n \n -r DMX Update Rate in Milliseconds");
                        message.Append("\n -a Auto Play Timeout in seconds");
                        message.Append("\n -c Cycle Modes: ");
                        foreach (var name in Enum.GetNames(typeof(CycleMode)))
                        {
                            message.Append(name + " ");
                        }
                        message.Append("\n -f full path and file name of fixture json file");
                        Log(message.ToString());

                        return false;
                    case "-r":
                        if (a + 1 < args.Length)
                        {
                            if (uint.TryParse(args[a + 1], out dmxUpdateRateMilliseconds))
                            {
                                DmxUpdateRateMilliseconds = dmxUpdateRateMilliseconds < 25 ? 25 : dmxUpdateRateMilliseconds;
                            }
                            else
                            {
                                Log("Invalid DMX Update Rate");
                                return false;
                            }
                        }
                        break;
                    case "-a":
                        if (a + 1 < args.Length)
                        {
                            if (uint.TryParse(args[a + 1], out autoPlay))
                            {
                                AutoPlay = autoPlay < 1 ? 1 : autoPlay;
                            }
                            else
                            {
                                Log("Invalid Auto Play timeout");
                                return false;
                            }
                        }
                        break;
                    case "-c":
                        if (a + 1 < args.Length)
                        {
                            if (Enum.TryParse<CycleMode>(args[a + 1].ToLowerInvariant(), out cycleMode))
                            {
                                AutoPlayCycleMode = cycleMode;
                            }
                            else
                            {
                                Log("Invalid Cycle Mode");
                                return false;
                            }
                        }
                        break;
                    case "-f":
                        if (a + 1 < args.Length)
                        {
                            fixtureFilename = args[a + 1];
                            if (File.Exists(fixtureFilename)) { FixtureFilename = fixtureFilename; }
                            else
                            {
                                Log($"{fixtureFilename} not found");
                                return false;
                            }
                        }
                        break;
                }
            }

            Log($"\n\nNew defaults: \nDMX Update Rate in milliseconds {DmxUpdateRateMilliseconds},\nAuto Play {AutoPlay} seconds, \nCycle Mode {AutoPlayCycleMode.ToString()}.");

            return true;
        }
    }
}
