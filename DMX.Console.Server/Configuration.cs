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

        public bool AutoPlayEnabled { get; set; } = false;
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

        Config config;

        public Configuration()
        {
            strProgramDataPath = Environment.OSVersion.Platform == PlatformID.Unix ? AppDomain.CurrentDomain.BaseDirectory : Path.Combine(Environment.ExpandEnvironmentVariables("%PROGRAMDATA%"), "dmx.server");
        }

        bool Initialise(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                Log("Expected event directory name. Remember on Linux names are case sensitive");

                return false;
            }

            string eventName = args[0];

            UniverseFilename = Path.Combine(strProgramDataPath, "config", eventName, "universe.json");
            AutoPlayFilename = Path.Combine(strProgramDataPath, "config", eventName, "autoplay.json");
            ConfigFilename = Path.Combine(strProgramDataPath, "config", eventName, "config.json");

            try
            {
                Log("\n\nLoading Config");
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFilename));
            }
            catch(Exception ex)
            {
                Log(ex.Message);
                return false;
            }

            return true;
        }

        public void Log(string messsage)
        {
            System.Console.WriteLine(messsage);
        }


        CycleMode? ValidateCycleMode(string mode)
        {
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

        public bool LoadConfig(string[] args)
        {
            StringBuilder message = new StringBuilder();

            if (!Initialise(args)) { return false; }            
       
            if (config.dmxRefreshRateMilliseconds != null) { DmxUpdateRateMilliseconds = (uint)config.dmxRefreshRateMilliseconds; }

            CycleMode? cycleMode = ValidateCycleMode(config.autoPlayCycleMode);
            if (cycleMode != null) { AutoPlayCycleMode = (CycleMode)cycleMode; }

            AutoPlayEnabled = config.autoPlayEnabled ?? false;

            if (config.autoPlayIntensity != null && config.autoPlayIntensity > 0 && config.autoPlayIntensity <= 1) { AutoPlayIntensity = (double)config.autoPlayIntensity; }
            if (config.autoPlayTimeout != null) { AutoPlayTimeout = (uint)config.autoPlayTimeout; }

            if (!string.IsNullOrEmpty(config.mqttBroker)) { MqttBroker = config.mqttBroker; }
            if (!string.IsNullOrEmpty(config.mqttDataTopic)) { MqttDataTopic = config.mqttDataTopic; }
            if (!string.IsNullOrEmpty(config.mqttStatusTopic)) { MqttStatusTopic = config.mqttStatusTopic; }


            message.Append("\nConfiguration\n");
            message.Append($"\nDMX Update Rate in milliseconds: {DmxUpdateRateMilliseconds}");
            message.Append($"\nAuto Play Enabled at Startup: {AutoPlayEnabled}");
            message.Append($"\nAuto Play Timeout in milliseconds: {AutoPlayTimeout}");
            message.Append($"\nAuto Play Intensity: {AutoPlayIntensity}");
            message.Append($"\nAuto Play Cycle Mode: {AutoPlayCycleMode}");
            message.Append($"\nMqtt Broker Address: {MqttBroker}");
            message.Append($"\nMqtt DMX Data Topic: {MqttDataTopic}");
            message.Append($"\nMqtt DMX Status Topic: {MqttStatusTopic}\n");

            Log(message.ToString());

            return true;
        }
    }
}
