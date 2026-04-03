using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using Navi_UI_WPF.Services;
using Navi_UI_WPF.Views;

namespace Navi_UI_WPF.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            // Lấy thông tin từ SessionManager (đã set sau khi login)
            var session = SessionManager.Instance;
            _currentUserName = session.CurrentUser?.UserName ?? "Unknown";
            _currentUserRole = GetRoleDisplayName(session.UserLevel);
            _isFullAccess = session.IsFullAccess;

            // Default view
            CurrentView = new HomeViewModel();
            IsMenuOpen = true;

            // Navigation Commands
            NavigateToHomeCommand        = new RelayCommand(() => CurrentView = new HomeViewModel());
            NavigateToAssemblyCommand    = new RelayCommand(() => CurrentView = new ProductAssemblyViewModel());
            NavigateToProductCommand     = new RelayCommand(() => CurrentView = new NaviProductViewModel());
            NavigateToItemCommand        = new RelayCommand(() => CurrentView = new NaviItemViewModel());
            NavigateToProductItemCommand = new RelayCommand(() => CurrentView = new NaviProductItemViewModel());
            NavigateToHistoryCommand     = new RelayCommand(() => CurrentView = new NaviHistoryViewModel());
            NavigateToForceGaugeCommand  = new RelayCommand(() => CurrentView = new ForceGaugeViewModel());
            NavigateToInsertBallCommand  = new RelayCommand(() => CurrentView = new InsertBallViewModel());
            ExitCommand                  = new RelayCommand(() => Application.Current.Shutdown());
            ToggleMenuCommand            = new RelayCommand(() => IsMenuOpen = !IsMenuOpen);
            LogoutCommand                = new RelayCommand(ExecuteLogout);
        }

        // ─── User Info ────────────────────────────────────────────────────────

        private string _currentUserName;
        public string CurrentUserName
        {
            get => _currentUserName;
            set => SetProperty(ref _currentUserName, value);
        }

        private string _currentUserRole;
        public string CurrentUserRole
        {
            get => _currentUserRole;
            set => SetProperty(ref _currentUserRole, value);
        }

        /// <summary>
        /// true nếu user có quyền SubLeader trở lên (level >= 1)
        /// Dùng để ẩn/hiện menu items Admin-only trong XAML
        /// </summary>
        private bool _isFullAccess;
        public bool IsFullAccess
        {
            get => _isFullAccess;
            set => SetProperty(ref _isFullAccess, value);
        }

        // ─── Navigation ───────────────────────────────────────────────────────

        private bool _isMenuOpen;
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set => SetProperty(ref _isMenuOpen, value);
        }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        // ─── Commands ─────────────────────────────────────────────────────────

        public ICommand ToggleMenuCommand { get; }
        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToAssemblyCommand { get; }
        public ICommand NavigateToProductCommand { get; }
        public ICommand NavigateToItemCommand { get; }
        public ICommand NavigateToProductItemCommand { get; }
        public ICommand NavigateToHistoryCommand { get; }
        public ICommand NavigateToForceGaugeCommand { get; }
        public ICommand NavigateToInsertBallCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand LogoutCommand { get; }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private static string GetRoleDisplayName(int level)
        {
            switch (level)
            {
                case 999: return "Admin";
                case 2:   return "Leader";
                case 1:   return "SubLeader";
                case 0:   return "User";
                default:  return "Unknown";
            }
        }

        private void ExecuteLogout()
        {
            var result = MessageBox.Show(
                "Bạn có muốn đăng xuất không?",
                "Xác nhận đăng xuất",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SessionManager.Instance.ClearSession();

                // Mở lại màn hình Login
                var loginWindow = new LoginView();
                Application.Current.MainWindow = loginWindow;
                loginWindow.Show();

                // Đóng tất cả các MainWindow đang mở để tránh rò rỉ và duplicate
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is MainWindow)
                    {
                        window.Close();
                    }
                }
            }
        }
    }
}
