using System.Threading.Tasks;
using Windows.UI;
using TimeToShineClient.Model.Entity;
using TimeToShineClient.Model.Repo;

namespace TimeToShineClient.Model.Contract
{
    public interface IMQTTService
    {
        void Publish(Colour colour);
        void PublishSpecial(byte b, int channel, Color color);

        void SetAutoPlayMode(MQTTService.AutoPlayMode mode);
    }
}