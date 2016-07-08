using System.Threading.Tasks;
using TimeToShineClient.Model.Entity;

namespace TimeToShineClient.Model.Contract
{
    public interface IMQTTService
    {
        void Publish(Colour colour);
        void PublishSpecial(byte b);
    }
}