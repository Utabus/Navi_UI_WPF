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
    /// HTTP service gọi tới /api/naviproductitems
    /// </summary>
    public class NaviProductItemService : INaviProductItemService
    {
        private readonly HttpClient _httpClient;

        public NaviProductItemService(HttpClient httpClient)
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

        public async Task<List<NaviProductItemDto>> GetAllAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.NaviProductItems);
            return await SendAsync<List<NaviProductItemDto>>(req);
        }

        public async Task<NaviProductItemDto> GetByIdAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductItemById, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviProductItemDto>(req);
        }

        public async Task<List<NaviProductItemDto>> GetByProductAsync(int productId)
        {
            var url = string.Format(ApiEndpoints.NaviProductItemsByProduct, productId);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviProductItemDto>>(req);
        }

        public async Task<List<NaviProductItemDto>> GetByItemAsync(int itemId)
        {
            var url = string.Format(ApiEndpoints.NaviProductItemsByItem, itemId);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviProductItemDto>>(req);
        }

        public async Task<bool> ExistsAsync(int productId, int itemId)
        {
            var url = $"{ApiEndpoints.NaviProductItemExists}?productId={productId}&itemId={itemId}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<bool>(req);
        }

        public async Task<NaviProductItemDto> CreateAsync(CreateNaviProductItemDto dto)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.NaviProductItems)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviProductItemDto>(req);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductItemById, id);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await SendAsync<object>(req);
            return true;
        }
    }
}
