using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Chat;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;
using Autofac;
using TimeToShineClient.Controls;
using TimeToShineClient.Model;
using TimeToShineClient.Model.Contract;
using TimeToShineClient.Model.Entity;
using TimeToShineClient.Model.Messages;
using TimeToShineClient.Model.Repo;
using TimeToShineClient.Util;
using uPLibrary.Networking.M2Mqtt;
using XamlingCore.Portable.Messages.XamlingMessenger;
using XamlingCore.Portable.View;
using XamlingCore.Portable.View.ViewModel;

namespace TimeToShineClient.View.ColorSelection
{
    public class ColorSelectViewModel : XViewModel
    {
        private readonly IColorService _colorService;
        private readonly IConfigService _configService;
        private readonly IMQTTService _mqttService;

        private Color _brush = Colors.Transparent;


        public ICommand SaveCommand { get; set; }
        public ICommand StartSaveCommand { get; set; }

        public ICommand SaveSettingsCommand { get; set; }
        public ICommand CancelSettingsCommand { get; set; }

        public ICommand AutoplayOnCommand { get; set; }
        public ICommand AutoplayOffCommand { get; set; }

        private Visibility _attractVisible;
        private bool _attractRunning;

        private bool _saveRunning;
        private bool _settingsRunning;
        private bool _colorSelectRunning;

        private float _chaseColor = 0;

        private string _firstName;
        private string _age;
        private string _suburb;
        private string _colorName;

        private string _broker;
        private string _topic;
        private string _baseUrl;
        private string _lightIds;
        private string _dmxChannel;
        private bool _debugMode;

        private string _debugText;

        int counter = 0;

        private List<SolidColorPanelViewModel> _colours;

        private List<Channel> _channels;

        public ColorSelectViewModel(IColorService colorService, IConfigService configService, IMQTTService mqttService)
        {
            _colorService = colorService;
            _configService = configService;
            _mqttService = mqttService;
            SaveCommand = new XCommand(_onSave);
            StartSaveCommand = new XCommand(_onStartSave);
            SaveSettingsCommand = new XCommand(_saveSettings);
            CancelSettingsCommand = new XCommand(_cancelSettings);

            AutoplayOffCommand = new XCommand(_onAutoplayOff);
            AutoplayOnCommand = new XCommand(_onAutoplayOn);


            //  _attractTimer();
            this.Register<ResetMessage>(_onReset);
            this.Register<SettingsMessage>(_onSettings);
            this.Register<DebugMessage>(_onDebugMessage);

            this.Register<SpecialColorSelectedMessage>(_onSpecialColorSelected);

        }

        void _onAutoplayOn()
        {
            _mqttService.SetAutoPlayMode(MQTTService.AutoPlayMode.AutoplayOn);
        }

        void _onAutoplayOff()
        {
            _mqttService.SetAutoPlayMode(MQTTService.AutoPlayMode.AutoplayOff);
        }

        void _onSpecialColorSelected(object message)
        {
            Dispatcher.Invoke(() =>
            {
                var m = message as SpecialColorSelectedMessage;

                Brush = m.Color.Color;
            });

        }

        void _toggled(Channel c)
        {
            foreach (var channel in Channels)
            {
                if (c == channel && c.IsToggled)
                {
                    _colorService.Channel = Convert.ToInt32(c.ChannelNumber);
                    continue;
                }

                channel.IsToggled = false;

            }
        }

        async void _init()
        {
            await Task.Delay(300);
            _colorService.Channel = 1;

            Channels = new List<Channel>();

            for (var c = 1; c <= 9; c++)
            {
                Channels.Add(new Channel()
                {
                    ChannelNumber = c.ToString(),
                    ToggleChannel = _toggled,
                    IsToggled = c == 1
                });

               
            }

            var colours = new List<Color>()
            {
                Colors.Black,
                Colors.Red,
                Colors.Green,
                Colors.Blue,
                Colors.Pink,
                Colors.White,
                Colors.DeepSkyBlue, 
                Colors.ForestGreen,
                Colors.MediumPurple,
                Colors.Orange,
                Colors.LightPink,
                Colors.Aqua,
                Colors.Lime,
                Colors.Teal,
                Colors.MediumVioletRed,

            };

            var width = Window.Current.Bounds.Width / colours.Count;

            var colTemp = new List<SolidColorPanelViewModel>();

            var i = new List<byte>()
            {
                    1,
                    16,
                    32,
                    48,
                    64,
                    80,
                    112,
                    128,
                    144,
                    160,
                    176,
                    192,
                    208,
                    224,
                    240,
            };

            for (var iColor = 0; iColor < colours.Count; iColor++)
            {
                var specialColor = new SpecialColor(colours[iColor], i[iColor]);

                Debug.WriteLine($"Color: {specialColor.Color}, code: {specialColor.SpecialCode} ");

                var vm = CreateContentModel<SolidColorPanelViewModel>(_ =>
                {
                    _.Colour = specialColor;
                    _.Width = width;
                });

                colTemp.Add(vm);
            }

            Colours = colTemp;
        }

        void _onDebugMessage(object message)
        {
            var m = message as DebugMessage;

            if (m == null)
            {
                return;
            }
            Dispatcher.Invoke(() =>
            {
                DebugText += $"\r\n{m.Message}";
            });
        }

        void _saveSettings()
        {
            _configService.MqttBroker = Broker;
            _configService.MqttTopic = Topic;
            _configService.ServiceBase = BaseUrl;
            _configService.LightIds = LightIds;
            _configService.DMXChannel = DmxChannel;
            _cancelSettings();
        }

        void _cancelSettings()
        {
            SettingsRunning = false;
        }

        void _onSettings()
        {


            Dispatcher.Invoke(() =>
            {
                Broker = _configService.MqttBroker;
                Topic = _configService.MqttTopic;
                BaseUrl = _configService.ServiceBase;
                LightIds = _configService.LightIds;
                DmxChannel = _configService.DMXChannel;
                SettingsRunning = true;
            });

        }

        void _onReset()
        {
            Dispatcher.Invoke(StartAttract);

        }

        async void _attractTimer()
        {
            while (true)
            {
                await Task.Delay(1000);
                if (counter > 45)
                {
                    counter = 0;
                    if (!AttractRunning)
                    {
                        StartAttract();
                    }

                }

                counter++;
            }
        }

        void _onStartSave()
        {
            _resetAttractTimer();
            StartSave();
        }

        void _resetAttractTimer()
        {
            counter = 0;
        }

        async void _onSave()
        {
            //var c = Brush.Color;

            //var uc = new UserColor
            //{
            //    Red = c.R,
            //    Green = c.G,
            //    Blue = c.B,
            //    ColorName = ColorName,
            //    SubmitterName = FirstName
            //};

            //_colorService.SaveColorToServer(uc);

            //await Task.Delay(10000);

            //StartAttract();
        }

        public override void OnInitialise()
        {
            base.OnInitialise();
            StartAttract();
            _init();
        }

        public void StartSave()
        {
            StopColorSelect();
            StopAttract();
            SaveRunning = true;
        }

        public bool ValidateColorName()
        {
            if (string.IsNullOrWhiteSpace(ColorName))
            {
                _showError("You need to enter a name for your awesome new color!");
                return false;
            }

            return true;
        }

        public bool ValidatePersonDemo()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                _showError("Hey! You have a first name right?? Enter it to continue!");
                return false;
            }

            //if (string.IsNullOrWhiteSpace(Age))

            //{
            //    _showError("Beep bop! Hrm - I cannot detect how old you are, can you help me out by entering your age?");
            //    return false;
            //}

            //try
            //{
            //    var intAge = Convert.ToInt32(Age);
            //}
            //catch
            //{
            //    _showError("Boom! The age you entered isn't really actually an age. It's like - some other thing. Enter Real age plz!");
            //    return false;
            //}

            //if (string.IsNullOrWhiteSpace(Suburb))
            //{
            //    _showError("We know where you live. Oh wait... no we don't. Can you please enter your suburb?");
            //    return false;
            //}

            return true;
        }

        async void _showError(string error)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(error, "Oops!");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Ok") { Id = 0 });

            await dialog.ShowAsync();
        }

        public void StopSave()
        {
            SaveRunning = false;
        }

        public void StartColorSelect()
        {
            _resetAttractTimer();
            FirstName = "";
            Age = "";
            Suburb = "";
            ColorName = "";

            if (ColorSelectRunning)
            {
                return;
            }
            StopAttract();
            StopSave();
            ColorSelectRunning = true;
        }

        public void StopColorSelect()
        {
            ColorSelectRunning = false;
        }

        public void StartAttract()
        {
            StopColorSelect();
            StopSave();
            AttractRunning = true;
            // _chase();
        }

        //async void _chase()
        //{
        //    while (AttractRunning)
        //    {
        //        await Task.Delay(20);
        //        _chaseColor++;
        //        var h = ColorUtils.FromHsv(_chaseColor, 1f, 1f);
        //        Brush = new SolidColorBrush(h);
        //    }
        //}

        public void StopAttract()
        {
            AttractRunning = false;
        }

        //public void SetColor(Color c)
        //{

        //    _resetAttractTimer();
        //    StartColorSelect();
        //    Brush = new SolidColorBrush(c);
        //    _publish(c);
        //}


        void _publish(Color colour)
        {
            _colorService.PublishSampleColor(colour);
        }

        public Color Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                OnPropertyChanged();
            }
        }

        public bool AttractRunning
        {
            get { return _attractRunning; }
            set
            {
                _attractRunning = value;
                OnPropertyChanged();
            }
        }

        public bool ColorSelectRunning
        {
            get { return _colorSelectRunning; }
            set
            {
                _colorSelectRunning = value;
                OnPropertyChanged();
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string Age
        {
            get { return _age; }
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }

        public string Suburb
        {
            get { return _suburb; }
            set
            {
                _suburb = value;
                OnPropertyChanged();
            }
        }

        public bool SaveRunning
        {
            get { return _saveRunning; }
            set
            {
                _saveRunning = value;
                OnPropertyChanged();
            }
        }

        public string ColorName
        {
            get { return _colorName; }
            set
            {
                _colorName = value;
                OnPropertyChanged();
            }
        }

        public bool SettingsRunning
        {
            get { return _settingsRunning; }
            set
            {
                _settingsRunning = value;
                OnPropertyChanged();
            }
        }

        public string Broker
        {
            get { return _broker; }
            set
            {
                _broker = value;
                OnPropertyChanged();
            }
        }

        public string Topic
        {
            get { return _topic; }
            set
            {
                _topic = value;
                OnPropertyChanged();
            }
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set
            {
                _baseUrl = value;
                OnPropertyChanged();
            }
        }

        public string LightIds
        {
            get { return _lightIds; }
            set
            {
                _lightIds = value;
                OnPropertyChanged();
            }
        }

        public string DmxChannel
        {
            get { return _dmxChannel; }
            set
            {
                _dmxChannel = value;
                OnPropertyChanged();
            }
        }

        public bool DebugMode
        {
            get { return _debugMode; }
            set
            {
                _debugMode = value;
                OnPropertyChanged();
            }
        }

        public string DebugText
        {
            get { return _debugText; }
            set
            {
                _debugText = value;
                OnPropertyChanged();
            }
        }

        public List<SolidColorPanelViewModel> Colours
        {
            get { return _colours; }
            set
            {
                _colours = value;
                OnPropertyChanged();
            }
        }

        public List<Channel> Channels
        {
            get { return _channels; }
            set
            {
                _channels = value;
                OnPropertyChanged();
            }
        }
    }
}
