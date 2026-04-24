using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Navi.Core.Common;
using Navi.Core.Constants;
using Navi.Core.Entities;
using Navi.Core.Interfaces;

namespace Navi.Infrastructure.Repositories
{
    /// <summary>
    /// Triển khai Repository cho NaviHistory sử dụng HTTP API
    /// REPOSITORY layer implementation for NaviHistory using HTTP API
    /// </summary>
    public class NaviHistoryRepository : INaviHistoryRepository
    {
        private readonly HttpClient _httpClient;

        public NaviHistoryRepository()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiEndpoints.BaseUrl)
            };

            // Bypass SSL certificate validation for localhost development
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
        }

        /// <summary>
        /// Helper gửi request và xử lý response chung
        /// </summary>
        private async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API Error: {(int)response.StatusCode} {response.ReasonPhrase}. Details: {content}");

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(content);
            if (apiResponse == null || !apiResponse.Success)
                throw new Exception($"API returned error: {apiResponse?.Message ?? "Unknown error"}");

            return apiResponse.Data;
        }

        private StringContent ToJson(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

        // ── Read ─────────────────────────────────────────────────────────

        public async Task<List<NaviHistory>> GetAllAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.NaviHistory);
            return await SendAsync<List<NaviHistory>>(req);
        }

        public async Task<NaviHistory> GetByIdAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryById, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviHistory>(req);
        }

        public async Task<List<NaviHistory>> GetByCodeNVAsync(string codeNV)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryByCodeNV, Uri.EscapeDataString(codeNV));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviHistory>>(req);
        }

        public async Task<List<NaviHistory>> GetByItemIdAsync(int itemId)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryByItem, itemId);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviHistory>>(req);
        }

        public async Task<List<NaviHistory>> GetByPOAsync(string po)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryByPO, Uri.EscapeDataString(po));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviHistory>>(req);
        }

        public async Task<List<NaviHistoryAuditComparison>> GetAuditComparisonAsync(int itemId)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryAuditComparison, itemId);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviHistoryAuditComparison>>(req);
        }

        // ── Write ────────────────────────────────────────────────────────

        public async Task<NaviHistory> CreateAsync(NaviHistory history)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.NaviHistory)
            {
                Content = ToJson(history)
            };
            return await SendAsync<NaviHistory>(req);
        }

        public async Task<NaviHistory> UpdateAsync(NaviHistory history)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryById, history.Id);
            var req = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = ToJson(history)
            };
            return await SendAsync<NaviHistory>(req);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviHistoryById, id);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await SendAsync<object>(req);
            return true;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
