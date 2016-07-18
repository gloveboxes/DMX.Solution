using Newtonsoft.Json;
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


        string strProgramDataPath;

        public uint DmxUpdateRateMilliseconds { get; set; } = 25; // default to 40 Hz

        #region Auto Cycle Mode
        public uint AutoPlayTimeout { get; set; } = 0;
        public CycleMode AutoPlayCycleMode { get; set; } = CycleMode.synced;
        public double AutoPlayIntensity { get; set; }
        #endregion

        #region Mqtt Defaults
        public string MqttBroker { get; set; } = "localhost";
        public string MqttDataTopic { get; set; } = "dmx/data/#";
        public string MqttStatusTopic { get; set; } = "dmx/status";
        #endregion

        public string UniverseFilename { get; set; }
        public string AutoPlayFilename { get; set; }
        private string ConfigFilename { get; set; }

        public Configuration()
        {
            strProgramDataPath = Environment.OSVersion.Platform == PlatformID.Unix ? "/home/pi" : Environment.ExpandEnvironmentVariables("%PROGRAMDATA%");

            UniverseFilename = Path.Combine(strProgramDataPath, "dmx.server", "universe.json");
            AutoPlayFilename = Path.Combine(strProgramDataPath, "dmx.server", "autoplay.json");
            ConfigFilename = Path.Combine(strProgramDataPath, "dmx.server", "config.json");
        }

        public void Log(string messsage)
        {
            System.Console.WriteLine(messsage);
        }


        CycleMode? ValidateCycleMode(string mode) {
            CycleMode cycleMode;
            if (Enum.TryParse<CycleMode>(mode.ToLowerInvariant(), out cycleMode))
            {
                return cycleMode;
            }
            else
            {
                return null;
            }
        }

        double? ValidateDouble(string value)
        {
            double outValue;
            if (double.TryParse(value, out outValue))
            {
                return outValue;
            }
            else
            {
                return null;
            }
        }

        public bool LoadConfig()
        {
            StringBuilder message = new StringBuilder();

            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFilename));   

            if (config.dmxRefreshRateMilliseconds !=null) { DmxUpdateRateMilliseconds = (uint)config.dmxRefreshRateMilliseconds; }

            CycleMode? cycleMode = ValidateCycleMode(config.autoPlayCycleMode);

            if (cycleMode != null) { AutoPlayCycleMode = (CycleMode)cycleMode; }
            if(config.autoPlayIntensity != null && config.autoPlayIntensity > 0 && config.autoPlayIntensity <= 1) { AutoPlayIntensity = (double)config.autoPlayIntensity; }
            if(config.autoPlayTimeout != null) { AutoPlayTimeout = (uint)config.autoPlayTimeout; }

            if(!string.IsNullOrEmpty(config.mqttBroker)) { MqttBroker = config.mqttBroker; }
            if(!string.IsNullOrEmpty(config.mqttDataTopic)) { MqttDataTopic = config.mqttDataTopic; }
            if(!string.IsNullOrEmpty(config.mqttStatusTopic)) { MqttStatusTopic = config.mqttStatusTopic; }


            message.Append("\n\nSettings\n");
            message.Append($"\nDMX Update Rate in milliseconds {DmxUpdateRateMilliseconds}");
            message.Append($"\nAuto Play {AutoPlayTimeout} seconds");
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
