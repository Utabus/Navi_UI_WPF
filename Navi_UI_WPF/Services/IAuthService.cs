using System.Collections.Generic;
using System.Threading.Tasks;
using Navi_UI_WPF.Models;

namespace Navi_UI_WPF.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthLoginResponse>> LoginAsync(string userId, string password, string lastIP = "string");
        Task<ApiResponse<List<AuthRole>>> GetRolesAsync();
    }
}
