using XamlingCore.Portable.Contract.Config;

namespace TimeToShineClient.Model.Contract
{
    public interface IConfigRepo : IConfig
    {
        void Write(string setting, string value);
    }
}