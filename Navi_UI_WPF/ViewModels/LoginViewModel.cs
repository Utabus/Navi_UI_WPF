using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Navi_UI_WPF.Services;

namespace Navi_UI_WPF.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        // ─── Event để App.xaml.cs biết khi login thành công ─────────────────
        public event Action LoginSucceeded;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(async () => await ExecuteLoginAsync(), CanLogin);
        }

        // ─── Properties ──────────────────────────────────────────────────────

        private string _userId;
        public string UserId
        {
            get => _userId;
            set
            {
                SetProperty(ref _userId, value);
                ((RelayCommand)LoginCommand).NotifyCanExecuteChanged();
            }
        }

 
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                ((RelayCommand)LoginCommand).NotifyCanExecuteChanged();
            }
        }

        // ─── Commands ─────────────────────────────────────────────────────────

        public ICommand LoginCommand { get; }

        private bool CanLogin() =>
            !string.IsNullOrWhiteSpace(UserId) &&
           
            !IsLoading;

        private async System.Threading.Tasks.Task ExecuteLoginAsync()
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            try
            {
                var result = await _authService.LoginAsync(UserId.Trim(), "1234");

                if (result == null)
                {
                    ErrorMessage = "Không nhận được phản hồi từ máy chủ.";
                    return;
                }

                if (result.StatusCode == 200 && result.Data != null)
                {
                    // Lấy danh sách roles (dùng để reference nếu cần, không bắt buộc)
                    var rolesResult = await _authService.GetRolesAsync();
                    var allRoles = rolesResult?.Data ?? new System.Collections.Generic.List<Models.AuthRole>();

                    // Lưu session
                    SessionManager.Instance.SetSession(result.Data, allRoles);

                    Serilog.Log.Information("Login success: {UserId}", UserId);
                    LoginSucceeded?.Invoke();
                }
                else
                {
                    ErrorMessage = string.IsNullOrEmpty(result.Message)
                        ? "Đăng nhập thất bại. Kiểm tra lại mã nhân viên và mật khẩu."
                        : result.Message;

                    Serilog.Log.Warning("Login failed for {UserId}: {Message}", UserId, result.Message);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi: " + ex.Message;
                Serilog.Log.Error(ex, "Login exception for {UserId}", UserId);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
