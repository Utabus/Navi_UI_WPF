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
    /// HTTP service gọi tới /api/naviproductmasteritems
    /// </summary>
    public class NaviProductMasterItemService : INaviProductMasterItemService
    {
        private readonly HttpClient _httpClient;

        public NaviProductMasterItemService(HttpClient httpClient)
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

        public async Task<List<NaviProductMasterItemDto>> GetAllAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.NaviProductMasterItems);
            return await SendAsync<List<NaviProductMasterItemDto>>(req);
        }

        public async Task<NaviProductMasterItemDto> GetByIdAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterItemById, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviProductMasterItemDto>(req);
        }

        public async Task<List<NaviProductMasterItemDto>> GetByProductMasterAsync(int productMasterId)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterItemsByProductMaster, productMasterId);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviProductMasterItemDto>>(req);
        }

        public async Task<List<NaviProductMasterItemDto>> GetByItemAsync(int itemId)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterItemsByItem, itemId);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviProductMasterItemDto>>(req);
        }

        public async Task<NaviProductMasterItemDto> CreateAsync(CreateNaviProductMasterItemDto dto)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.NaviProductMasterItems)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviProductMasterItemDto>(req);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterItemById, id);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await SendAsync<object>(req);
            return true;
        }
    }
}
