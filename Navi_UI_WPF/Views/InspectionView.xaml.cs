using System.Windows.Controls;

namespace Navi_UI_WPF.Views
{
    /// <summary>
    /// Interaction logic for InspectionView.xaml
    /// </summary>
    public partial class InspectionView : UserControl
    {
        public InspectionView()
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
