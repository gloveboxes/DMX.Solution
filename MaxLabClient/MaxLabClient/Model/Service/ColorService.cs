using System.Diagnostics;
using Windows.UI;
using TimeToShineClient.Model.Contract;
using TimeToShineClient.Model.Entity;
using XamlingCore.Portable.Contract.Downloaders;
using XamlingCore.Portable.Contract.Repos.Base;

namespace TimeToShineClient.Model.Service
{
    public class ColorService : IColorService
    {
        private readonly IMQTTService _mqttService;
        private readonly IConfigService _configService;
        private readonly IXWebRepo<UserColor> _colorsRepo;

        public ColorService(IMQTTService mqttService, 
            IConfigService configService, 
            IXWebRepo<UserColor> colorsRepo)
        {
            _mqttService = mqttService;
            _configService = configService;
            _colorsRepo = colorsRepo;
        }

        public int Channel { get; set; }

        public void PublishSpecialSampleColor(byte c, Color color)
        {
            Debug.WriteLine($"Color: {c}, Channel: {Channel}");
            _mqttService.PublishSpecial(c, Channel, color);
        }

        public void PublishSampleColor(Color c)
        {
            var colour = Colour.FromColor(c);
            colour.LightId = _configService.LightIdArray;
            _mqttService.Publish(colour);
        }

        public async void SaveColorToServer(UserColor c)
        {
            try
            {
                var saveResult = await _colorsRepo.Post(c, "UserColors");
            }
            catch
            {

            }

        }
    }
}
