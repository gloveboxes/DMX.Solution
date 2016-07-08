using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TimeToShineClient.Model.Contract;
using XamlingCore.Portable.Contract.Config;
using XamlingCore.Portable.Contract.Downloaders;
using XamlingCore.Portable.Contract.Network;
using XamlingCore.Portable.Messages.Network;
using XamlingCore.Portable.Messages.XamlingMessenger;
using XamlingCore.Portable.Net.DownloadConfig;
using XamlingCore.Portable.Net.Service;

namespace TimeToShineClient.Model.Service
{
    public class TransferConfigService : HttpTransferConfigServiceBase
    {
        private readonly IConfigService _config;
        
        private readonly IDeviceNetworkStatus _deviceNetworkStatus;

        public TransferConfigService(IConfigService config, IDeviceNetworkStatus deviceNetworkStatus)
        {
            _config = config;
           
            _deviceNetworkStatus = deviceNetworkStatus;
        }

        public override async Task<IHttpTransferConfig> GetConfig(string service, string verb)
        {
            var baseUrl = _config.ServiceBase;

            var url = $"{baseUrl}/{service}";

            var config = new StandardHttpConfig
            {
                Accept = "application/json",
                IsValid = true,
                Url = url,
                BaseUrl = "",
                Verb = verb,
                Headers = new Dictionary<string, string>()
            };

            return config;
        }
    }
}
