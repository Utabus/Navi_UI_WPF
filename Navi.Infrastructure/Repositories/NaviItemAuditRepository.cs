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
    public class NaviItemAuditRepository : INaviItemAuditRepository
    {
        private readonly HttpClient _httpClient;

        public NaviItemAuditRepository()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(ApiEndpoints.BaseUrl) };
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

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

        public async Task<List<NaviItemAudit>> GetByItemIdAsync(int itemId)
        {
            var url = string.Format(ApiEndpoints.NaviItemAudits, itemId);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviItemAudit>>(req);
        }

        public async Task<NaviItemAudit> GetByIdAsync(int auditId)
        {
            var url = string.Format(ApiEndpoints.NaviItemAuditById, auditId);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviItemAudit>(req);
        }

        public void Dispose() => _httpClient?.Dispose();
    }
}
