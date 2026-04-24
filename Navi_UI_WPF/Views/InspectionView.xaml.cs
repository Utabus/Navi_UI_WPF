using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Navi_UI_WPF.ViewModels;

namespace Navi_UI_WPF.Views
{
    public partial class InspectionView : UserControl
    {
        public InspectionView(InspectionViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            
            vm.RequestFocusAction = () =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    PoTextBox.Focus();
                    Keyboard.Focus(PoTextBox);
                    PoTextBox.SelectAll();
                }));
            };
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
