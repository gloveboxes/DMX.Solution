using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using TimeToShineClient.Util;
using XamlingCore.UWP.Contract;
using XamlingCore.UWP.View;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TimeToShineClient.View.ColorSelection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ColorSelectView : XPage, IXPage<ColorSelectViewModel>
    {
        public ColorSelectView()
        {
            this.InitializeComponent();
        }


        public override void SetViewModel(object vm)
        {
            ViewModel = vm as ColorSelectViewModel;
        }

        public ColorSelectViewModel ViewModel { get; set; }



        private void ChangeHue(Point p)
        {
          //  Debug.WriteLine($"X: {p.X}, Y: {p.Y}");

            //var pointX = 

            var py = Math.Max(0d, p.Y);
            py = Math.Min(this.colorSpectrum.ActualHeight, py);

            var px = Math.Max(0d, p.X);
            px = Math.Min(this.colorSpectrum.ActualWidth, px);


            var hue = (float)(py * 360f / this.colorSpectrum.ActualHeight);
            var bright = (float)((px/2) * 1f / this.colorSpectrum.ActualWidth) + .5f;

            Debug.WriteLine($"hue: {hue}, bright: {bright}");

            var h = ColorUtils.FromHsv(hue, 1f, bright);

            var colorString = string.Format("#FF{0:X2}{1:X2}{2:X2}", h.R, h.G, h.B);

           // Debug.WriteLine(colorString);

           // ViewModel.SetColor(h);

            //var specPoint = Math.Round(py, MidpointRounding.AwayFromZero);

            //this.ViewModel.ColorSpectrumPoint = Math.Round(py, MidpointRounding.AwayFromZero);
        }

        //private void OnHuePressed(object sender, PointerRoutedEventArgs e)
        //{
        //    this.ChangeHue(e.GetCurrentPoint(this.colorSpectrum).Position.Y);
        //    this.colorSpectrum.CapturePointer(e.Pointer);

        //    PointerEventHandler moved = null;
        //    moved = (s, args) =>
        //    {
        //        this.ChangeHue(args.GetCurrentPoint(this.colorSpectrum).Position.Y);
        //    };
        //    PointerEventHandler released = null;
        //    released = (s, args) =>
        //    {
        //        this.colorSpectrum.ReleasePointerCapture(args.Pointer);
        //        this.ChangeHue(args.GetCurrentPoint(this.colorSpectrum).Position.Y);
        //        this.colorSpectrum.PointerMoved -= moved;
        //        this.colorSpectrum.PointerReleased -= released;
        //    };
        //    this.colorSpectrum.PointerMoved += moved;
        //    this.colorSpectrum.PointerReleased += released;
        //}

        private void UIElement_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pos = e.GetCurrentPoint(sender as UIElement).Position;
            

            var p = new Point(pos.X, pos.Y);
            this.ChangeHue(p);

            this.colorSpectrum.CapturePointer(e.Pointer);

            PointerEventHandler moved = null;
            moved = (s, args) =>
            {
                var posMoved = e.GetCurrentPoint(s as UIElement).Position;
                var pMoved = new Point(posMoved.X, posMoved.Y);
                this.ChangeHue(pMoved);
            };
            PointerEventHandler released = null;
            released = (s, args) =>
            {
                this.colorSpectrum.ReleasePointerCapture(args.Pointer);
                var posMoved = e.GetCurrentPoint(s as UIElement).Position;
                var pMoved = new Point(posMoved.X, posMoved.Y);
                this.ChangeHue(pMoved);
                this.colorSpectrum.PointerMoved -= moved;
                this.colorSpectrum.PointerReleased -= released;
            };
            this.colorSpectrum.PointerMoved += moved;
            this.colorSpectrum.PointerReleased += released;

        }
    }
}
