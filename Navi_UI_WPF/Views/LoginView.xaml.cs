using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Navi_UI_WPF.ViewModels;

namespace Navi_UI_WPF.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// PasswordBox không hỗ trợ binding trực tiếp trong WPF
    /// nên phải dùng code-behind để truyền password vào ViewModel.
    /// </summary>
    public partial class LoginView : Window
    {
        private LoginViewModel _vm;
        private readonly System.IServiceProvider _serviceProvider;

        public LoginView(LoginViewModel vm, System.IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _vm = vm;
            _serviceProvider = serviceProvider;
            _vm.LoginSucceeded += OnLoginSucceeded;
            DataContext = _vm;

            // Cho phép nhấn Enter để đăng nhập
           
            TxtUserId.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter && _vm.LoginCommand.CanExecute(null))
                    _vm.LoginCommand.Execute(null);
            };
        }

     
        private void OnLoginSucceeded()
        {
            // Đóng Login, mở Main window từ DI
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            this.Close();
        }
    }
}
