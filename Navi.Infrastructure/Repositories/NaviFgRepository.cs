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
    public class NaviFgRepository : INaviFgRepository
    {
        private readonly HttpClient _httpClient;

        public NaviFgRepository()
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

        private StringContent ToJson(object obj) => new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

        public async Task<List<NaviFg>> GetAllAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.NaviFg);
            return await SendAsync<List<NaviFg>>(req);
        }

        public async Task<NaviFg> GetByIdAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviFgById, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviFg>(req);
        }

        public async Task<List<NaviFg>> GetByPOAsync(string po)
        {
            var url = string.Format(ApiEndpoints.NaviFgByPO, Uri.EscapeDataString(po));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviFg>>(req);
        }

        public async Task<List<NaviFg>> GetByCodeNVAsync(string codeNV)
        {
            var url = string.Format(ApiEndpoints.NaviFgByCodeNV, Uri.EscapeDataString(codeNV));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<List<NaviFg>>(req);
        }

        public async Task<NaviFg> CreateAsync(NaviFg fg)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.NaviFg) { Content = ToJson(fg) };
            return await SendAsync<NaviFg>(req);
        }

        public async Task<NaviFg> UpdateAsync(NaviFg fg)
        {
            var url = string.Format(ApiEndpoints.NaviFgById, fg.Id);
            var req = new HttpRequestMessage(HttpMethod.Put, url) { Content = ToJson(fg) };
            return await SendAsync<NaviFg>(req);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviFgById, id);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await SendAsync<object>(req);
            return true;
        }

        public void Dispose() => _httpClient?.Dispose();
    }
}
