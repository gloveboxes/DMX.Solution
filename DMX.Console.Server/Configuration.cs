using System;
using System.IO;
using System.Text;

namespace DMX.Server
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
        public uint DmxUpdateRateMilliseconds { get; set; } = 25; // default to 40 Hz
        public uint AutoPlay { get; set; } = 0;
        public CycleMode AutoPlayCycleMode { get; set; } = CycleMode.synced;
        public string FixtureFilename { get; set; }

        public Configuration()
        {
            FixtureFilename = AppDomain.CurrentDomain.BaseDirectory + "fixtures.json";
        }

        public void Log(string messsage)
        {
            System.Console.WriteLine(messsage);
        }


        public bool ParseArgs(string[] args)
        {
            StringBuilder message = new StringBuilder();
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
                        message.Append("Usage:\n \n -r DMX Update Rate in Milliseconds");
                        message.Append("\n -a Auto Play Timeout in seconds (0 to disable)");
                        message.Append("\n -c Cycle Modes: ");
                      
                        foreach (var name in Enum.GetNames(typeof(CycleMode)))
                        {
                            message.Append(name + " ");
                        }
                        message.Append("\n -f full path and file name of fixture json file");
                        message.Append("\n -b Mqtt Broker Address (default is localhost)");
                        message.Append("\n -t Mqtt Topic to subscribe to for DMX Data Messages (default is dmx/data/# )");

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
                                AutoPlay = autoPlay;
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
                    case "-b":
                        if (a + 1 < args.Length)
                        {
                            MqttBroker = args[a + 1];
                        }
                        break;
                    case "-t":
                        if (a + 1 < args.Length)
                        {
                            MqttDataTopic = args[a + 1];
                        }
                        break;

                }
            }

            
            message.Append("\n\nNew defaults:");
            message.Append($"\nDMX Update Rate in milliseconds {DmxUpdateRateMilliseconds}");
            message.Append($"\nAuto Play {AutoPlay} seconds");
            message.Append($"\nCycle Mode {AutoPlayCycleMode.ToString()}");
            message.Append($"\nMqtt Broker Address {MqttBroker}");
            message.Append($"\nMqtt DMX Data Subscribe Topic {MqttDataTopic}");

            Log(message.ToString());

            return true;
        }
    }
}
