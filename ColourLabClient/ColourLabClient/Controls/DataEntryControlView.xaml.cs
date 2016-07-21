using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using TimeToShineClient.View.ColorSelection;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TimeToShineClient.Controls
{
    public sealed partial class DataEntryControlView : UserControl
    {
        public ICommand SaveCommand { get; set; }

        public DataEntryControlView()
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
            VisualStateManager.GoToState(this, "EnterColourNameState", true);
            FirstTextBox.Focus(FocusState.Programmatic);
            //EnterStory.BeginTime = TimeSpan.Zero;
            //EnterStory.Begin();
        }

        public void Stop()
        {
            VisualStateManager.GoToState(this, "BeforeState", true);
            // EnterStory.Stop();
        }

        private async void ButtonToEnterName_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_validateColor())
            {
                return;
            }
            VisualStateManager.GoToState(this, "EnterYourNameState", true);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_validate())
            {
                return;
            }
            SaveCommand?.Execute(null);
            VisualStateManager.GoToState(this, "ThanksState", true);
            await Task.Delay(6000);
            VisualStateManager.GoToState(this, "BackHomeState", true);
        }


        bool _validate()
        {
            var vm = DataContext as ColorSelectViewModel;
            if (vm == null)
            {
                return false;
            }

            return vm.ValidatePersonDemo();
        }

        bool _validateColor()
        {
            var vm = DataContext as ColorSelectViewModel;
            if (vm == null)
            {
                return false;
            }

            return vm.ValidateColorName();
        }

        private void Grid_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
