namespace TimeToShineClient.Model.Entity
{
    interface IFixture
    {
        byte[] data { get; }
        uint[] id { get; set; }
        bool SetChannel(int channel, byte value);   // returns true if new values are the same as old
        bool SetRgb(byte red, byte green, byte blue, byte white = 0);  // returns true if new values are the same as old
        byte[] ToJson();
    }
}