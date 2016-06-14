using TimeToShineClient.View.ColorSelection;
using TimeToShineClient.View.Home;
using TimeToShineClient.View.Menu;
using XamlingCore.UWP.Contract;
using XamlingCore.UWP.Navigation.MasterDetail;
using XUWPMasterDetailViewModel = TimeToShineClient.View.Root.XUWPMasterDetailViewModel;

namespace TimeToShineClient.View
{
    public class RootViewModel : XUWPMasterDetailViewModel
    {
        public RootViewModel(IUWPViewResolver viewResolver) : base(viewResolver)
        {
        }

        public override void OnInitialise()
        {
            AddPackage<ColorSelectViewModel>();

            SetMaster(CreateContentModel<MenuMasterViewModel>());

            Build();

            base.OnInitialise();
        }
    }
}
