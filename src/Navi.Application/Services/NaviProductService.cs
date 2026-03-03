using System;
using System.Collections.Generic;
using System.IO;
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
    /// HTTP service gọi tới /api/naviproducts
    /// </summary>
    public class NaviProductService : INaviProductService
    {
        private readonly HttpClient _httpClient;

        public NaviProductService(HttpClient httpClient)
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

        // ── CRUD cơ bản ─────────────────────────────────────────────────

        public async Task<List<NaviProductDto>> GetAllAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.NaviProducts);
            return await SendAsync<List<NaviProductDto>>(req);
        }

        public async Task<NaviProductDto> GetByIdAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductById, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviProductDto>(req);
        }

        public async Task<NaviProductWithItemsDto> GetWithItemsAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductWithItems, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviProductWithItemsDto>(req);
        }

        public async Task<List<NaviProductDto>> SearchAsync(string term)
        {
            var url = $"{ApiEndpoints.NaviProductSearch}?term={Uri.EscapeDataString(term)}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviProductDto>>(req);
        }

        public async Task<NaviProductDto> CreateAsync(CreateNaviProductDto dto)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.NaviProducts)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviProductDto>(req);
        }

        public async Task<NaviProductDto> UpdateAsync(int id, UpdateNaviProductDto dto)
        {
            var url = string.Format(ApiEndpoints.NaviProductById, id);
            var req = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviProductDto>(req);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductById, id);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await SendAsync<object>(req);
            return true;
        }

        // ── With Items (transaction) ─────────────────────────────────────

        public async Task<NaviProductDto> CreateWithItemsAsync(CreateNaviProductWithItemsDto dto)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.NaviProductCreateWithItems)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviProductDto>(req);
        }

        public async Task<NaviProductDto> UpdateWithItemsAsync(int id, UpdateNaviProductWithItemsDto dto)
        {
            var url = string.Format(ApiEndpoints.NaviProductUpdateWithItems, id);
            var req = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = ToJson(dto)
            };
            return await SendAsync<NaviProductDto>(req);
        }

        public async Task<bool> DeleteWithItemsAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductDeleteWithItems, id);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await SendAsync<object>(req);
            return true;
        }

        // ── Import Excel ─────────────────────────────────────────────────

        public async Task<ImportResultDto> ImportExcelAsync(Stream fileStream, string fileName)
        {
            using (var formData = new MultipartFormDataContent())
            {
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                formData.Add(fileContent, "file", fileName);

                var response = await _httpClient.PostAsync(ApiEndpoints.NaviProductImportExcel, formData);
                var content  = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new ApiException((int)response.StatusCode, response.ReasonPhrase, content);

                var wrapper = JsonConvert.DeserializeObject<Core.Common.ApiResponse<ImportResultDto>>(content);
                return wrapper?.Data;
            }
        }
    }
}
