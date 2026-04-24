using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Navi_UI_WPF.Views
{
    /// <summary>
    /// Interaction logic for ProductAssemblyView.xaml
    /// </summary>
    public partial class ProductAssemblyView : UserControl
    {
        public ProductAssemblyView(ViewModels.ProductAssemblyViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            
            // Register focus request from ViewModel
            vm.RequestFocusAction = () =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    PoTextBox.Focus();
                    Keyboard.Focus(PoTextBox);
                    PoTextBox.SelectAll(); // Select all text to make it easy to overwrite
                }));
            };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Use BeginInvoke to ensure the UI is fully loaded before focusing
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                PoTextBox.Focus();
                Keyboard.Focus(PoTextBox);
            }));
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
