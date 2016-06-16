using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DMX.Server
{
    public class FixtureManager : DmxController
    {
        List<Fixtures> fixtures;
        int channels;
        Configuration config;

        #region random colour collection
        Colour[] RandomColours = new Colour[]
        {
            new Colour(90, 195,0), // lime
            new Colour(172, 0, 164), // purple
            new Colour(255, 0,0), // red
            new Colour(0, 185, 137), //green
            new Colour(192, 60, 0), // copper
            new Colour(254, 29, 0), // red
            new Colour(0, 249, 253), // teal
            new Colour(255, 107, 0), // orange
            new Colour(0,0,255), // blue
            new Colour(128, 0, 2), // crimson
            new Colour(30,0,255), // royal blue
            new Colour(215, 0, 103), // pink
            new Colour(0, 255, 0), // green
            new Colour(255, 125,0), // gold
            new Colour(141, 0, 253), // violet
        };


        uint NextColour;
        Random rndColour = new Random();

        #endregion

        public FixtureManager(Configuration config, uint DmxPort) : base(DmxPort)
        {
            this.config = config;
        }

        public bool InitialiseFixtures()
        {
            try
            {
                config.Log("Loading Fixtures");
                LoadFixtures();

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
            foreach (var fixture in fixtures)
            {
                UpdateChannel(fixture.startChannel, fixture.numberOfChannels, fixture.initialChannelMask);
            }
            DmxUpdate();
        }

        public void LoadFixtures()
        {
            fixtures = JsonConvert.DeserializeObject<List<Fixtures>>(File.ReadAllText(config.FixtureFilename));

            var fixture = (from f in fixtures orderby f.startChannel descending select f).FirstOrDefault();

            if (fixture != null)
            {
                channels = (int)(fixture.startChannel + fixture.numberOfChannels);
                if (channels > 512) { channels = 512; }
            }
            config.Log($"DMX Channels {channels}\n\n");
        }

        public void UpdateFixtureRGBW(FixtureData fixtureData)
        {
            if (fixtureData.red == null && fixtureData.green == null
                && fixtureData.red == null && fixtureData.white == null) { return; }

            foreach (var id in fixtureData.id)
            {
                var fixture = (from f in fixtures where f.id == id select f).FirstOrDefault();
                if (fixture == null) { return; }

                UpdateColour(fixtureData.red, fixture.redChannels, fixture);
                UpdateColour(fixtureData.green, fixture.greenChannels, fixture);
                UpdateColour(fixtureData.blue, fixture.blueChannels, fixture);
                UpdateColour(fixtureData.white, fixture.whiteChannels, fixture);
            }
        }


        public void UpdateFixtureData(FixtureData fixtureData)
        {
            foreach (var id in fixtureData.id)
            {
                var fixture = (from f in fixtures where f.id == id select f).FirstOrDefault();
                if (fixture != null) { UpdateChannel(fixture.startChannel, fixture.numberOfChannels, fixtureData.data); }
            }
        }

        private void UpdateColour(byte? colour, byte[] channels, Fixtures fixture)
        {
            if (colour == null || channels == null || fixture.startChannel < 1) { return; }

            foreach (var channel in channels)
            {
                if (channel > fixture.numberOfChannels) { continue; }
                UpdateChannel((int)(fixture.startChannel + channel - 1), (byte)colour);
            }
        }

        public void RandomColour(Configuration.CycleMode mode)
        {
            Colour colour = null;

            if (mode == Configuration.CycleMode.synced)
            {
                colour = RandomColours[NextColour++ % RandomColours.Length];
            }

            foreach (var fixture in fixtures)
            {
                switch (mode)
                {
                    case Configuration.CycleMode.sequential:
                        colour = RandomColours[NextColour++ % RandomColours.Length];
                        break;
                    case Configuration.CycleMode.random:
                        colour = new Colour((byte)rndColour.Next(0, 255), (byte)rndColour.Next(0, 255), (byte)rndColour.Next(0, 255));
                        break;
                    default:
                        break;
                }

                UpdateColour(colour.Red, fixture.redChannels, fixture);
                UpdateColour(colour.Green, fixture.greenChannels, fixture);
                UpdateColour(colour.Blue, fixture.blueChannels, fixture);
            }
        }
    }
}
