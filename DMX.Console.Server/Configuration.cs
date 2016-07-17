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

        public enum Intensity : byte
        {
            high,
            medium,
            low
        }


        public string MqttBroker { get; set; } = "localhost";
        public string MqttDataTopic { get; set; } = "dmx/data/#";

        public string MqttStatusTopic = "dmx/status";

        public uint DmxUpdateRateMilliseconds { get; set; } = 25; // default to 40 Hz
        public uint AutoPlay { get; set; } = 0;
        public CycleMode AutoPlayCycleMode { get; set; } = CycleMode.synced;

        public Intensity LightIntensity = Intensity.high;
        public string UniverseFilename { get; set; }
        public string AutoPlayFilename { get; set; }
        public double AutoPlayIntensity { get; set; }

        public Configuration()
        {
            UniverseFilename = AppDomain.CurrentDomain.BaseDirectory + "universe.json";
            AutoPlayFilename = AppDomain.CurrentDomain.BaseDirectory + "autoplay.json";
        }

        public void Log(string messsage)
        {
            System.Console.WriteLine(messsage);
        }

        public void SetIntensity(string level = null)
        {
            AutoPlayIntensity = 1;
            if (level != null)
            {
                if (!Enum.TryParse<Intensity>(level.ToLowerInvariant(), out LightIntensity)) { return; }
            }

            switch (LightIntensity)
            {
                case Intensity.high:
                    AutoPlayIntensity = 1;
                    break;
                case Intensity.medium:
                    AutoPlayIntensity = 0.7;
                    break;
                case Intensity.low:
                    AutoPlayIntensity = 0.4;
                    break;
                default:
                    break;
            }
        }


        public bool ParseArgs(string[] args)
        {
            StringBuilder message = new StringBuilder();
            uint dmxUpdateRateMilliseconds, autoPlay = 0;
            CycleMode cycleMode;
            Intensity intensity;
            string universeFilename = string.Empty;

            // -c 50 -r 25 -a 5 -s random -f 

            for (int a = 0; a < args.Length; a += 2)
            {
                switch (args[a].ToLower())
                {
                    case "/?":
                    case "?":
                    case "-?":
                    case "/h":
                    case "-h":
                    case "help":
                    case "/help":
                    case "-help":
                        message.Append("Usage:\n \n -r DMX Update Rate in Milliseconds");
                        message.Append("\n -? ? /? /h -h help /help -help This help");
                        message.Append("\n -a Auto Play Timeout in seconds (0 to disable)");
                        message.Append("\n -c Cycle Modes: ");
                        foreach (var name in Enum.GetNames(typeof(CycleMode)))
                        {
                            message.Append(name + " ");
                        }

                        message.Append("\n -i Light Intensity: ");
                        foreach (var name in Enum.GetNames(typeof(Intensity)))
                        {
                            message.Append(name + " ");
                        }

                        message.Append("\n -u full path and file name of universe.json file");
                        message.Append("\n -b Mqtt Broker Address (default is localhost)");
                        message.Append("\n -t Mqtt Topic to subscribe to for DMX Data Messages (default is dmx/data/# )");
                        message.Append("\n -s Mqtt Topic for DMX Status (default is dmx/status )");

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

                    case "-i":
                        if (a + 1 < args.Length)
                        {
                            if (Enum.TryParse<Intensity>(args[a + 1].ToLowerInvariant(), out intensity))
                            {
                                LightIntensity = intensity;
                                SetIntensity(null);
                            }
                            else
                            {
                                Log("Invalid Intensity");
                                return false;
                            }
                        }
                        break;
                    case "-u":
                        if (a + 1 < args.Length)
                        {
                            universeFilename = args[a + 1];
                            if (File.Exists(universeFilename)) { UniverseFilename = universeFilename; }
                            else
                            {
                                Log($"{universeFilename} not found");
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
                    case "-s":
                        if (a + 1 < args.Length)
                        {
                            MqttStatusTopic = args[a + 1];
                        }
                        break;

                }
            }


            message.Append("\n\nSettings\n");
            message.Append($"\nDMX Update Rate in milliseconds {DmxUpdateRateMilliseconds}");
            message.Append($"\nAuto Play {AutoPlay} seconds");
            message.Append($"\nAuto Play Light Level Intensity is {AutoPlayIntensity.ToString()}");
            message.Append($"\nCycle Mode {AutoPlayCycleMode.ToString()}");
            message.Append($"\nMqtt Broker Address {MqttBroker}");
            message.Append($"\nMqtt DMX Data Topic {MqttDataTopic}");
            message.Append($"\nMqtt DMX Status Topic {MqttStatusTopic}");
            message.Append($"\nUniverse definition path and file name {UniverseFilename}");


            Log(message.ToString());

            return true;
        }
    }
}
