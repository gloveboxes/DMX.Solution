using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamlingCore.Portable.View.ViewModel;

namespace TimeToShineClient.Model.Entity
{
    public class Channel : XViewModel
    {
        public string ChannelNumber { get; set; }

        bool _isToggled;

        public Action<Channel> ToggleChannel { get; set; }

        public bool IsToggled
        {
            get { return _isToggled; }
            set
            {
                if (value == _isToggled)
                {
                    return;
                }
                _isToggled = value;
                if (_isToggled)
                {
                    ToggleChannel(this);
                }
                
                
                OnPropertyChanged();
            }
        }

    }
}
