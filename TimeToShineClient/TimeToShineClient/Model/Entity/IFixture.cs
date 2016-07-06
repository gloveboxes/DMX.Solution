namespace TimeToShineClient.Model.Entity
{
    interface IFixture
    {
        byte[] data { get; }
        uint[] id { get; set; }
        void SetChannel(int channel, byte value);
        void SetRgb(byte red, byte green, byte blue, byte white = 0);
        bool IsSame(byte red, byte green, byte blue, byte white = 0);
        byte[] ToJson();
    }
}