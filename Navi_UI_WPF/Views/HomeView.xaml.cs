using System.Windows.Controls;
using Navi_UI_WPF.ViewModels;

namespace Navi_UI_WPF.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView(HomeViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
