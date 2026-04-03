using System.Windows.Controls;

namespace Navi_UI_WPF.Views
{
    /// <summary>
    /// Interaction logic for ProductAssemblyView.xaml
    /// </summary>
    public partial class ProductAssemblyView : UserControl
    {
        public ProductAssemblyView()
        {
            InitializeComponent();
            
            // Load sample data (Product ID = 1)
            if (DataContext is ViewModels.ProductAssemblyViewModel vm)
            {
                _ = vm.LoadProductAssemblyAsync(1);
            }
        }

        private void StepsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StepsDataGrid.SelectedItem != null)
            {
                StepsDataGrid.ScrollIntoView(StepsDataGrid.SelectedItem);
            }
        }
    }
}
