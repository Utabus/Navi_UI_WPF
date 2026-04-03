using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Navi_UI_WPF.Models;

namespace Navi_UI_WPF.Services
{
    /// <summary>
    /// Dịch vụ xác thực — gọi API Auth backend.
    /// Base URL: http://192.168.100.100
    /// Dùng DataContractJsonSerializer (built-in .NET 4.7.2, không cần NuGet package).
    /// </summary>
    public class AuthService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://192.168.100.100"),
            Timeout = TimeSpan.FromSeconds(15)
        };

        /// <summary>
        /// Đăng nhập.
        /// POST /api/v2/Auth/login
        /// </summary>
        public async Task<ApiResponse<AuthLoginResponse>> LoginAsync(string userId, string password, string lastIP = "string")
        {
            // Build JSON thủ công để tránh phụ thuộc serializer
            var json = $"{{\"userId\":\"{EscapeJson(userId)}\",\"password\":\"{EscapeJson(password)}\",\"lastIP\":\"{EscapeJson(lastIP)}\"}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var httpResponse = await _httpClient.PostAsync("/api/v2/Auth/login", content);
                var responseString = await httpResponse.Content.ReadAsStringAsync();

                Serilog.Log.Debug("LoginAsync response ({StatusCode}): {Body}", httpResponse.StatusCode, responseString);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new ApiResponse<AuthLoginResponse>
                    {
                        StatusCode = (int)httpResponse.StatusCode,
                        Message = $"Server trả về lỗi: {httpResponse.StatusCode}"
                    };
                }

                return Deserialize<ApiResponse<AuthLoginResponse>>(responseString);
            }
            catch (TaskCanceledException)
            {
                Serilog.Log.Warning("LoginAsync timeout");
                return new ApiResponse<AuthLoginResponse>
                {
                    StatusCode = 408,
                    Message = "Hết thời gian kết nối. Kiểm tra mạng nội bộ."
                };
            }
            catch (HttpRequestException ex)
            {
                Serilog.Log.Error(ex, "LoginAsync network error");
                return new ApiResponse<AuthLoginResponse>
                {
                    StatusCode = 503,
                    Message = "Không thể kết nối đến máy chủ. Kiểm tra mạng nội bộ."
                };
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "LoginAsync unexpected error");
                return new ApiResponse<AuthLoginResponse>
                {
                    StatusCode = 500,
                    Message = "Lỗi không xác định: " + ex.Message
                };
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả roles trong hệ thống.
        /// GET /api/v2/Auth/roles
        /// </summary>
        public async Task<ApiResponse<List<AuthRole>>> GetRolesAsync()
        {
            try
            {
                var httpResponse = await _httpClient.GetAsync("/api/v2/Auth/roles");
                var responseString = await httpResponse.Content.ReadAsStringAsync();

                Serilog.Log.Debug("GetRolesAsync response ({StatusCode}): {Body}", httpResponse.StatusCode, responseString);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new ApiResponse<List<AuthRole>>
                    {
                        StatusCode = (int)httpResponse.StatusCode,
                        Message = $"Không lấy được danh sách roles: {httpResponse.StatusCode}"
                    };
                }

                return Deserialize<ApiResponse<List<AuthRole>>>(responseString);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "GetRolesAsync error");
                return new ApiResponse<List<AuthRole>>
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private static T Deserialize<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T),
                new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                });
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (T)serializer.ReadObject(ms);
            }
        }

        private static string EscapeJson(string value)
        {
            if (value == null) return "";
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
