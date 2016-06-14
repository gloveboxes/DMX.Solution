﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Chat;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;
using TimeToShineClient.Model;
using TimeToShineClient.Model.Contract;
using TimeToShineClient.Model.Entity;
using TimeToShineClient.Model.Messages;
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

        SolidColorBrush _brush = new SolidColorBrush(Colors.White);

        public ICommand SaveCommand { get; set; }
        public ICommand StartSaveCommand { get; set; }

        public ICommand SaveSettingsCommand { get; set; }
        public ICommand CancelSettingsCommand { get; set; }

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

        public ColorSelectViewModel(IColorService colorService, IConfigService configService)
        {
            _colorService = colorService;
            _configService = configService;
            SaveCommand = new XCommand(_onSave);
            StartSaveCommand = new XCommand(_onStartSave);
            SaveSettingsCommand = new XCommand(_saveSettings);
            CancelSettingsCommand = new XCommand(_cancelSettings);
            _attractTimer();
            this.Register<ResetMessage>(_onReset);
            this.Register<SettingsMessage>(_onSettings);
            this.Register<DebugMessage>(_onDebugMessage);
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
            var c = Brush.Color;

            var uc = new UserColor
            {
                Red = c.R,
                Green = c.G,
                Blue = c.B,
                ColorName = ColorName,
                SubmitterName = FirstName
            };

            _colorService.SaveColorToServer(uc);

            await Task.Delay(10000);

            StartAttract();
        }

        public override void OnInitialise()
        {
            base.OnInitialise();
            StartAttract();
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
            _chase();
        }

        async void _chase()
        {
            while (AttractRunning)
            {
                await Task.Delay(20);
                _chaseColor++;
                var h = ColorUtils.FromHsv(_chaseColor, 1f, 1f);
                Brush = new SolidColorBrush(h);
            }
        }

        public void StopAttract()
        {
            AttractRunning = false;
        }

        public void SetColor(Color c)
        {

            _resetAttractTimer();
            StartColorSelect();
            Brush = new SolidColorBrush(c);
            _publish(c);
        }


        void _publish(Color colour)
        {
            _colorService.PublishSampleColor(colour);
        }

        public SolidColorBrush Brush
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
    }
}
