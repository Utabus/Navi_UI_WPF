using System.Windows;
using HandyControl.Controls;
using Navi_UI_WPF.ViewModels;

namespace Navi_UI_WPF.Views
{
    public partial class NaviProductFormWindow : HandyControl.Controls.Window
    {
        public NaviProductFormWindow(NaviProductViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // If the ViewModel's SaveCommand handles the logic, we just close the window
            // Assuming SaveCommand validates and saves successfully.
            // In a real app, you might want to bind an event or a CloseWindow attached behavior.
            DialogResult = true;
            Close();
        }
    }
}
