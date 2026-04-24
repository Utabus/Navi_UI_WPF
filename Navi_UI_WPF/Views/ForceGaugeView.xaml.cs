using System.Windows.Controls;
using Navi_UI_WPF.ViewModels;

namespace Navi_UI_WPF.Views
{
    public partial class ForceGaugeView : UserControl
    {
        public ForceGaugeView(ForceGaugeViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
