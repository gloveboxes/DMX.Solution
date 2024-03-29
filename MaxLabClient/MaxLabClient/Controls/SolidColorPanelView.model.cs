﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using TimeToShineClient.Model.Contract;
using TimeToShineClient.Model.Entity;
using TimeToShineClient.Model.Messages;
using XamlingCore.Portable.Messages.XamlingMessenger;
using XamlingCore.Portable.View.ViewModel;

namespace TimeToShineClient.Controls
{
    public class SolidColorPanelViewModel : XViewModel
    {
        private readonly IColorService _colorService;
        private SpecialColor _colour;
        private double _width;

        public SolidColorPanelViewModel(IColorService colorService)
        {
            _colorService = colorService;
        }

        public void MousedIn()
        {
            new SpecialColorSelectedMessage(_colour).Send();
            _colorService.PublishSpecialSampleColor(_colour.SpecialCode, _colour.Color);
        }

        public SpecialColor Colour
        {
            get { return _colour; }
            set
            {
                _colour = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value; 
                OnPropertyChanged();
            }
        }
    }
}
