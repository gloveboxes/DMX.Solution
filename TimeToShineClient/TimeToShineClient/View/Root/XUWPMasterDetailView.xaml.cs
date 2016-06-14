﻿using Windows.UI.Xaml;
using XamlingCore.Portable.Messages.View;
using XamlingCore.Portable.Messages.XamlingMessenger;
using XamlingCore.UWP.Contract;
using XamlingCore.UWP.View;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TimeToShineClient.View.Root
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class XUWPMasterDetailView : XPage, IXPage<XUWPMasterDetailViewModel>
    {
        public XUWPMasterDetailView()
        {
            this.InitializeComponent();


            this.Register<CollapseMasterDetailMessage>(_onCollapse);
            this.Register<ShowMasterDetailMessage>(_onShow);

            this.Register<EnableMenuSwipeGesture>(_onEnableSwipe);
            this.Register<DisableMenuSwipeGesture>(_onDisableSwipe);
        }

        void _onEnableSwipe()
        {


        }

        void _onDisableSwipe()
        {

        }


        void _onShow(object obj)
        {

        }

        void _onCollapse(object obj)
        {

        }

        void _setContent()
        {
            if (ViewModel.MasterContent != null && ViewModel.MasterContent != SplittyViewMcSplitFace.Pane && SplittyViewMcSplitFace.Pane == null)
            {
                SplittyViewMcSplitFace.Pane = ViewModel.MasterContent;
            }

            if (ViewModel.DetailContent != null && ViewModel.DetailContent != SplittyViewMcSplitFace.Content)
            {
                SplittyViewMcSplitFace.Content = ViewModel.DetailContent;
            }

            //IsPresented = false;
        }

        public override void SetViewModel(object vm)
        {
            ViewModel = vm as XUWPMasterDetailViewModel;
            _setContent();
        }


        public XUWPMasterDetailViewModel ViewModel { get; set; }

        private void TogglePaneButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
