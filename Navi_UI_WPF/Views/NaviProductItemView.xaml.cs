using System.Windows.Controls;

namespace Navi_UI_WPF.Views
{
    public partial class NaviProductItemView : UserControl
    {
        public NaviProductItemView(ViewModels.NaviProductItemViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
