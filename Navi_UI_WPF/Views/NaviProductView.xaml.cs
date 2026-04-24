using System.Windows.Controls;

namespace Navi_UI_WPF.Views
{
    public partial class NaviProductView : UserControl
    {
        public NaviProductView(ViewModels.NaviProductViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
