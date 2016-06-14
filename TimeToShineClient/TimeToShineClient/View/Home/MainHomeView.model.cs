using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeToShineClient.View.ColorSelection;
using XamlingCore.Portable.View.ViewModel;

namespace TimeToShineClient.View.Home
{
    public class MainHomeViewModel :XViewModel
    {
        public void ClickNext()
        {
            NavigateTo<ColorSelectViewModel>();
        }
    }
}
