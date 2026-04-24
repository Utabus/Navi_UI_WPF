using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Navi.Core.Common;
using Navi.Core.Constants;
using Navi.Core.Interfaces;
using Navi.Core.Entities;

namespace Navi.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Manufa integration using HTTP API
    /// </summary>
    public class ManufaRepository : IManufaRepository
    {
        private readonly HttpClient _httpClient;
        
        public ManufaRepository()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiEndpoints.BaseUrl)
            };
        }
        
        public async Task<ManufaAssist> GetAssistByPOAsync(string po)
        {
            try
            {
                var endpoint = string.Format(ApiEndpoints.ManufaAssist, po);
                var response = await _httpClient.GetAsync(endpoint);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ManufaAssist>>(content);
                
                if (apiResponse == null || !apiResponse.Success)
                {
                    return null;
                }
                
                return apiResponse.Data;
            }
            catch (Exception)
            {
                // Rely on upper layers for error handling if needed,
                // or use a standard trace for legacy systems.
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
