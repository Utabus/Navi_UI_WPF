using System.Windows;
using Navi_UI_WPF.ViewModels;

namespace Navi_UI_WPF.Views
{
    public partial class NgReasonDialog : Window
    {
        private readonly NgReasonViewModel _viewModel;

        public NgReasonDialog(NgReasonViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // Handle viewmodel-driven close
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(NgReasonViewModel.DialogResult))
                {
                    if (_viewModel.DialogResult.HasValue)
                    {
                        this.DialogResult = _viewModel.DialogResult.Value;
                        this.Close();
                    }
                }
            };
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
