using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Navi.Application.DTOs;
using Navi.Core.Constants;
using Navi.Core.Exceptions;
using Navi.Application.Interfaces;
using Newtonsoft.Json;

namespace Navi.Application.Services
{
    /// <summary>
    /// HTTP service gọi tới /api/navihistory
    /// </summary>
    public class NaviHistoryService : INaviHistoryService
    {
        private readonly HttpClient _httpClient;

        public NaviHistoryService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // ── Helper ──────────────────────────────────────────────────────

        private async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);
            var content  = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ApiException((int)response.StatusCode, response.ReasonPhrase, content);

            var wrapper = JsonConvert.DeserializeObject<Core.Common.ApiResponse<T>>(content);
            if (wrapper == null || !wrapper.Success)
                throw new ApiException((int)response.StatusCode, wrapper?.Message ?? "Lỗi không xác định");

            return wrapper.Data;
        }

        private StringContent ToJson(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

        // ── Read ─────────────────────────────────────────────────────────

        public async Task<List<NaviHistoryDto>> GetAllAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.NaviHistory);
            return await SendAsync<List<NaviHistoryDto>>(req);
        }

        public async Task<NaviHistoryDto> GetByIdAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryById, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviHistoryDto>(req);
        }

        public async Task<List<NaviHistoryDto>> GetByCodeNVAsync(string codeNV)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryByCodeNV, Uri.EscapeDataString(codeNV));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviHistoryDto>>(req);
        }

        public async Task<List<NaviHistoryDto>> GetByProductItemAsync(int productItemId)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryByProductItem, productItemId);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviHistoryDto>>(req);
        }

        public async Task<List<NaviHistoryDto>> GetByPOAsync(string po)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryByPO, Uri.EscapeDataString(po));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviHistoryDto>>(req);
        }

        // ── Write ────────────────────────────────────────────────────────

        public async Task<NaviHistoryDto> CreateAsync(CreateNaviHistoryDto dto)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.NaviHistory)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviHistoryDto>(req);
        }

        public async Task<NaviHistoryDto> UpdateAsync(int id, UpdateNaviHistoryDto dto)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryById, id);
            var req = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviHistoryDto>(req);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryById, id);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await SendAsync<object>(req);
            return true;
        }
    }
}
