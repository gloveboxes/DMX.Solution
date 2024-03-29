﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TimeToShineClient.Controls
{
    public sealed partial class SolidColorPanelView : UserControl
    {
        public SolidColorPanelView()
        {
            this.InitializeComponent();
            this.PointerEntered += SolidColorPanelView_PointerEntered;
            this.PointerPressed += SolidColorPanelView_PointerPressed;
        }

        private void SolidColorPanelView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var vm = this.DataContext as SolidColorPanelViewModel;

            vm?.MousedIn();
        }

        private void SolidColorPanelView_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Pointer.IsInContact)
            {
                return;
            }
            var vm = this.DataContext as SolidColorPanelViewModel;

            vm?.MousedIn();
        }
    }
}
