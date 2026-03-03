using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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
    /// Repository implementation for Product Assembly using HTTP API
    /// Triển khai repository cho Product Assembly sử dụng HTTP API
    /// </summary>
    public class ProductAssemblyRepository : IProductAssemblyRepository
    {
        private readonly HttpClient _httpClient;
        
        public ProductAssemblyRepository()
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
        /// Lấy thông tin sản phẩm và các bước lắp ráp theo ID
        /// Get product assembly information and items by ID
        /// </summary>
        public async Task<ProductAssembly> GetProductAssemblyByIdAsync(int productId)
        {
            try
            {
                var endpoint = string.Format(ApiEndpoints.NaviProductItems, productId);
                var response = await _httpClient.GetAsync(endpoint);
                
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ProductAssembly>>(content);
                
                if (apiResponse == null || !apiResponse.Success)
                {
                    throw new Exception($"API returned error: {apiResponse?.Message ?? "Unknown error"}");
                }
                
                return apiResponse.Data;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to fetch product assembly: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse API response: {ex.Message}", ex);
            }
        }
        
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
