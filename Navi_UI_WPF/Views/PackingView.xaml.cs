using System.Windows.Controls;

namespace Navi_UI_WPF.Views
{
    /// <summary>
    /// Interaction logic for PackingView.xaml
    /// </summary>
    public partial class PackingView : UserControl
    {
        public PackingView()
        {
            InitializeComponent();
        }

        private void StepsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dg && dg.SelectedItem != null)
            {
                dg.ScrollIntoView(dg.SelectedItem);
            }
        }
    }
}
