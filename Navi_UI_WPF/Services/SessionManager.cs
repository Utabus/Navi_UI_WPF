using System;
using System.Collections.Generic;
using System.Linq;
using Navi_UI_WPF.Models;

namespace Navi_UI_WPF.Services
{
    /// <summary>
    /// Singleton — lưu trạng thái đăng nhập toàn app.
    /// </summary>
    public class SessionManager
    {
        private static readonly Lazy<SessionManager> _instance =
            new Lazy<SessionManager>(() => new SessionManager());

        public static SessionManager Instance => _instance.Value;

        private SessionManager() { }

        // ─── Thông tin user hiện tại ────────────────────────────────────────
        public UserInfo CurrentUser { get; private set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Level cao nhất trong tất cả roles của user.
        /// Level 0 = User, 1 = SubLeader, 2 = Leader, 999 = Admin
        /// </summary>
        public int UserLevel { get; private set; } = -1;

        /// <summary>
        /// true nếu user có level >= 1 (SubLeader trở lên) → được truy cập full menu
        /// </summary>
        public bool IsFullAccess => UserLevel >= 1;

        public bool IsLoggedIn => CurrentUser != null;

        // ─── Methods ─────────────────────────────────────────────────────────

        /// <summary>
        /// Lưu thông tin session sau khi login thành công.
        /// Parse UserRoles từ format "roleName=level=DisplayName" để tính UserLevel.
        /// </summary>
        public void SetSession(AuthLoginResponse loginData, List<AuthRole> allRoles)
        {
            CurrentUser = loginData.User;
            AccessToken = loginData.AccessTokenValue;
            RefreshToken = loginData.RefreshTokenValue;

            // Parse roles của user để lấy level cao nhất
            // Format trong userRoles: "admin=999=Admin" hoặc "ERP=0=User"
            int maxLevel = -1;

            if (loginData.User?.UserRoles != null)
            {
                foreach (var roleStr in loginData.User.UserRoles)
                {
                    // Tách theo '='
                    var parts = roleStr.Split('=');
                    // parts[1] là level number
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int lvl))
                    {
                        if (lvl > maxLevel)
                            maxLevel = lvl;
                    }
                }
            }

            UserLevel = maxLevel;

            Serilog.Log.Information(
                "Session set: User={UserId} | UserName={UserName} | Level={Level} | FullAccess={FullAccess}",
                CurrentUser?.UserId, CurrentUser?.UserName, UserLevel, IsFullAccess);
        }

        /// <summary>
        /// Xóa session khi đăng xuất.
        /// </summary>
        public void ClearSession()
        {
            Serilog.Log.Information("Session cleared for user: {UserId}", CurrentUser?.UserId);
            CurrentUser = null;
            AccessToken = null;
            RefreshToken = null;
            UserLevel = -1;
        }
    }
}
