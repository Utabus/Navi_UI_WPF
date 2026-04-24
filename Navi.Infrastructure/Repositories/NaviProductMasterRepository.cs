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
    public class NaviProductMasterRepository : INaviProductMasterRepository
    {
        private readonly HttpClient _httpClient;

        public NaviProductMasterRepository()
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

        public async Task<List<NaviProductMaster>> GetAllAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.NaviProductMaster);
            return await SendAsync<List<NaviProductMaster>>(req);
        }

        public async Task<NaviProductMaster> GetByIdAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterById, id);
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviProductMaster>(req);
        }

        public async Task<NaviProductMaster> GetByProductPAsync(string productP)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterByP, Uri.EscapeDataString(productP));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviProductMaster>(req);
        }

        public async Task<NaviProductMaster> GetByProductHAsync(string productH)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterByH, Uri.EscapeDataString(productH));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviProductMaster>(req);
        }

        public async Task<NaviProductMaster> GetByProductNameAsync(string productName)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterByName, Uri.EscapeDataString(productName));
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync<NaviProductMaster>(req);
        }

        public async Task<NaviProductMaster> CreateAsync(NaviProductMaster productMaster)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.NaviProductMaster) { Content = ToJson(productMaster) };
            return await SendAsync<NaviProductMaster>(req);
        }

        public async Task<NaviProductMaster> UpdateAsync(NaviProductMaster productMaster)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterById, productMaster.Id);
            var req = new HttpRequestMessage(HttpMethod.Put, url) { Content = ToJson(productMaster) };
            return await SendAsync<NaviProductMaster>(req);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiEndpoints.NaviProductMasterById, id);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            await SendAsync<object>(req);
            return true;
        }

        public void Dispose() => _httpClient?.Dispose();
    }
}
