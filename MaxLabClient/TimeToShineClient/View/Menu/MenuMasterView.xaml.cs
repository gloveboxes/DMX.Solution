using XamlingCore.UWP.Contract;
using XamlingCore.UWP.View;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TimeToShineClient.View.Menu
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuMasterView : XPage, IXPage<MenuMasterViewModel>
    {
        public MenuMasterView()
        {
            this.InitializeComponent();
        }

        public override void SetViewModel(object vm)
        {
            ViewModel = vm as MenuMasterViewModel;
        }

        public MenuMasterViewModel ViewModel { get; set; }
    }
}
