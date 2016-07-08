using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeToShineClient.Model.Contract;
using XamlingCore.Portable.Contract.Config;

namespace TimeToShineClient.Model.Repo
{
    public class ConfigRepo : IConfig, IConfigRepo
    {
        Windows.Storage.ApplicationDataContainer _localSettings =
    Windows.Storage.ApplicationData.Current.LocalSettings;

        public void Write(string setting, string value)
        {
            _localSettings.Values[setting] = value;
        }

        public string this[string index] => _localSettings.Values[index]?.ToString();
    }
}
