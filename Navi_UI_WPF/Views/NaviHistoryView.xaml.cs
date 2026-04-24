using System.Windows.Controls;

namespace Navi_UI_WPF.Views
{
    public partial class NaviHistoryView : UserControl
    {
        public NaviHistoryView(ViewModels.NaviHistoryViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
