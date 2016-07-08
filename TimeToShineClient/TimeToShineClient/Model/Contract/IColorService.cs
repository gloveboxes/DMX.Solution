using Windows.UI;
using TimeToShineClient.Model.Entity;

namespace TimeToShineClient.Model.Contract
{
    public interface IColorService
    {
        void PublishSampleColor(Color c);
        void SaveColorToServer(UserColor c);
        void PublishSpecialSampleColor(byte c);
    }
}