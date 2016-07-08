using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TimeToShineClient.Model.Messages;
using XamlingCore.Portable.Messages.XamlingMessenger;
using XamlingCore.Portable.View.ViewModel;
using XamlingCore.Portable.View.ViewModel.Base;

namespace TimeToShineClient.View.Menu
{
    public class MenuMasterViewModel : DisplayListViewModel<MenuOptionViewModel, XViewModel>
    {
        public void ResetClick()
        {
            new ResetMessage().Send();
        }

        public async void SettingsClick()
        {
            var dialog = new ContentDialog()
            {
                Title = "Passcode",
                
            };

            var panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = "Please enter the admin passcode",
                TextWrapping = TextWrapping.Wrap,
            });

            var txt = new PasswordBox();
            panel.Children.Add(txt);

            dialog.Content = panel;

            // Add Buttons
            dialog.PrimaryButtonText = "OK";
            dialog.IsPrimaryButtonEnabled = true;
            dialog.PrimaryButtonClick += delegate
            {
                var t = txt.Password;
                if (t == "1080")
                {
                    new SettingsMessage().Send();
                }
            };

            dialog.SecondaryButtonText = "Cancel";
            dialog.SecondaryButtonClick += delegate {
                
            };

            // Show Dialog
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.None)
            {
               
            }

           // 
        }
    }
}
