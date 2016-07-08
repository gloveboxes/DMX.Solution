using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlingCore.Portable.View;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TimeToShineClient.Controls
{
    public sealed partial class ExplainerControlView : UserControl
    {
        

        public ExplainerControlView()
        {
            this.InitializeComponent();
           
        }

        public bool IsRunning
        {
            get { return false; }
            set
            {
                _toggle(value);
            }
        }

        void _toggle(bool running)
        {
            if (running)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        public void Start()
        {
            EnterStory.BeginTime = TimeSpan.Zero;
            EnterStory.Begin();   
        }

        public void Stop()
        {
            EnterStory.Stop();
        }

        
    }
}
