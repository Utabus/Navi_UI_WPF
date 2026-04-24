using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Navi.Application.DTOs;
using Navi.Application.Interfaces;
using Navi.Core.Common;
using Newtonsoft.Json;

namespace Navi.Application.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly HttpClient _httpClient;

        public SettingsService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        private async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);
            var content  = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API Error: {response.StatusCode} - {response.ReasonPhrase}");

            var wrapper = JsonConvert.DeserializeObject<ApiResponse<T>>(content);
            if (wrapper == null || !wrapper.Success)
                throw new Exception(wrapper?.Message ?? "Unknown API Error");

            return wrapper.Data;
        }

        public async Task<SystemSettingsDto> GetSettingsAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "api/settings");
            return await SendAsync<SystemSettingsDto>(req);
        }

        public async Task<SystemSettingsDto> UpdateSettingsAsync(SystemSettingsDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            var req  = new HttpRequestMessage(HttpMethod.Put, "api/settings")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            return await SendAsync<SystemSettingsDto>(req);
        }
    }
}
