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
    /// HTTP service gọi tới /api/naviitems
    /// </summary>
    public class NaviItemService : INaviItemService
    {
        private readonly HttpClient _httpClient;

        public NaviItemService(HttpClient httpClient)
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

        // ── CRUD ────────────────────────────────────────────────────────

        public async Task<List<NaviItemDto>> GetAllAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.NaviItems);
            return await SendAsync<List<NaviItemDto>>(req);
        }

        public async Task<NaviItemDto> GetByIdAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviItemById, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviItemDto>(req);
        }

        public async Task<NaviItemWithProductsDto> GetWithProductsAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviItemWithProducts, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviItemWithProductsDto>(req);
        }

        public async Task<List<NaviItemDto>> GetByTypeAsync(string type)
        {
            var url = string.Format(ApiEndpoints.NaviItemByType, Uri.EscapeDataString(type));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviItemDto>>(req);
        }

        public async Task<List<NaviItemDto>> SearchAsync(string term)
        {
            var url = $"{ApiEndpoints.NaviItemSearch}?term={Uri.EscapeDataString(term)}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviItemDto>>(req);
        }

        public async Task<List<NaviItemDto>> GetByProductMasterNameAsync(string productName)
        {
            var url = string.Format(ApiEndpoints.NaviItemsByProductMasterName, Uri.EscapeDataString(productName));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviItemDto>>(req);
        }

        public async Task<List<NaviItemStatusDto>> GetWithHistoryStatusAsync(string productName, string po)
        {
            var url = string.Format(ApiEndpoints.NaviItemsWithHistoryStatus, 
                Uri.EscapeDataString(productName ?? ""), 
                Uri.EscapeDataString(po ?? ""));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviItemStatusDto>>(req);
        }

        public async Task<NaviItemDto> CreateAsync(CreateNaviItemDto dto)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.NaviItems)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviItemDto>(req);
        }

        public async Task<NaviItemDto> UpdateAsync(int id, UpdateNaviItemDto dto)
        {
            var url = string.Format(ApiEndpoints.NaviItemById, id);
            var req = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviItemDto>(req);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviItemById, id);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await SendAsync<object>(req);
            return true;
        }
    }
}
