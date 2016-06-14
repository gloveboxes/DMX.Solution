namespace DMX.Client.Fixtures
{
    interface IFixture
    {
        byte[] bChns { get; set; }
        byte[] data { get; }
        //uint[] id { get; set; }
        byte[] gChns { get; set; }
        byte[] rChns { get; set; }
        uint[] dmxChn { get; set; }
        byte[] wChns { get; set; }

        void SetChannel(int channel, byte value);
        void SetRgb(byte red, byte green, byte blue);
        byte[] ToJson();
    }
}