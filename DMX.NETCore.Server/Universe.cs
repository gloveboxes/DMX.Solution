using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DMX.Server
{
    public class Universe : DmxController
    {
        List<Fixture> universe;
        List<AutoPlay> autoplay;
        int channels;
        Configuration config;

        uint NextColour;
        Random rndColour = new Random();

        public Universe(Configuration config, uint DmxPort) : base(DmxPort)
        {
            this.config = config;
        }

        public bool InitialiseUniverse()
        {
            try
            {
                config.Log("Loading AutoPlay");
                LoadAutoPlay();

                config.Log("Loading Universe");
                LoadUniverse();

                config.Log("Opening DMX Controller");
                Open();

                config.Log("Initialising DMX Controller");
                InitializeDMX(channels);

                SetFixtureMasks();
            }
            catch (Exception ex)
            {
                config.Log("Error: " + ex.Message);
                return false;
            }
            return true;
        }

        void SetFixtureMasks()
        {
            foreach (var fixture in universe)
            {
                UpdateChannel(fixture.startChannel, fixture.numberOfChannels, fixture.initialChannelMask);
            }
            DmxUpdate();
        }

        public void LoadUniverse()
        {
            universe = JsonConvert.DeserializeObject<List<Fixture>>(File.ReadAllText(config.UniverseFilename));

            var fixture = (from f in universe orderby f.startChannel descending select f).FirstOrDefault();

            if (fixture != null)
            {
                channels = (int)(fixture.startChannel + fixture.numberOfChannels);
                if (channels > 512) { channels = 512; }
            }
            config.Log($"DMX Channels {channels}\n\n");
        }

        public void LoadAutoPlay()
        {
            autoplay = JsonConvert.DeserializeObject<List<AutoPlay>>(File.ReadAllText(config.AutoPlayFilename));
        }

        public void UpdateFixtureChannels(FixtureMessage fixtureMsg)
        {
            if (fixtureMsg.red == null && fixtureMsg.green == null
                && fixtureMsg.blue == null && fixtureMsg.white == null &&
                fixtureMsg.strobe == null) { return; }

            foreach (var id in fixtureMsg.id)
            {
                var fixture = (from f in universe where f.fixtureId == id select f).FirstOrDefault();
                if (fixture == null) { return; }

                UpdateChannelData(fixtureMsg.red, fixture.redChannels, fixture);
                UpdateChannelData(fixtureMsg.green, fixture.greenChannels, fixture);
                UpdateChannelData(fixtureMsg.blue, fixture.blueChannels, fixture);
                UpdateChannelData(fixtureMsg.white, fixture.whiteChannels, fixture);
                UpdateChannelData(fixtureMsg.strobe, fixture.strobeChannels, fixture);
            }
        }


        public void UpdateFixtureData(FixtureMessage fixtureMsg)
        {
            foreach (var id in fixtureMsg.id)
            {
                var fixture = (from f in universe where f.fixtureId == id select f).FirstOrDefault();
                if (fixture != null) { UpdateChannel(fixture.startChannel, fixture.numberOfChannels, fixtureMsg.data); }
            }
        }

        private void UpdateChannelData(byte? data, byte[] channels, Fixture fixture)
        {
            if (data == null || channels == null || fixture.startChannel < 1) { return; }

            foreach (var channel in channels)
            {
                if (channel > fixture.numberOfChannels) { continue; }
                UpdateChannel((int)(fixture.startChannel + channel - 1), (byte)data);
            }
        }

        public void RandomColour(Configuration.CycleMode mode)
        {
            if (mode == Configuration.CycleMode.synced)
            {
                NextColour++;
            }

            foreach (var fixture in universe)
            {
                switch (mode)
                {
                    case Configuration.CycleMode.sequential:
                        NextColour++;
                        break;
                    //case Configuration.CycleMode.random:
                    //    colour = new Colour((byte)rndColour.Next(0, 255), (byte)rndColour.Next(0, 255), (byte)rndColour.Next(0, 255));
                    //    break;
                    default:
                        break;
                }

                var ap = (from f in autoplay where f.autoPlayId.ToLower() == fixture.autoPlayId.ToLower() select f).FirstOrDefault();
                if (ap == null) { continue; }

                var chnData = ap.data[NextColour % ap.data.Length];

                switch (ap.type.ToLower())
                {
                    case "raw":
                        if (chnData.Length > 0)
                        {
                            UpdateChannel(fixture.startChannel, fixture.numberOfChannels, chnData);
                        }
                        break;
                    case "rgb":
                        if (chnData.Length == 3)
                        {
                            var red = chnData[0];
                            var green = chnData[1];
                            var blue = chnData[2];

                            UpdateChannelData((byte)(red * config.AutoPlayIntensity), fixture.redChannels, fixture);
                            UpdateChannelData((byte)(green * config.AutoPlayIntensity), fixture.greenChannels, fixture);
                            UpdateChannelData((byte)(blue * config.AutoPlayIntensity), fixture.blueChannels, fixture);
                        }
                        break;
                    case "rgbw":
                        if (ap.data[NextColour % ap.data.Length].Length == 4)
                        {
                            var red = chnData[0];
                            var green = chnData[1];
                            var blue = chnData[2];
                            var white = chnData[3];

                            UpdateChannelData((byte)(red * config.AutoPlayIntensity), fixture.redChannels, fixture);
                            UpdateChannelData((byte)(green * config.AutoPlayIntensity), fixture.greenChannels, fixture);
                            UpdateChannelData((byte)(blue * config.AutoPlayIntensity), fixture.blueChannels, fixture);
                            UpdateChannelData((byte)(white * config.AutoPlayIntensity), fixture.whiteChannels, fixture);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
