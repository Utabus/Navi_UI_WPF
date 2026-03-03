using System;
using System.Threading.Tasks;
using Navi.Application.DTOs;
using Navi.Core.Interfaces;

namespace Navi.Application.Services
{
    /// <summary>
    /// Service xử lý business logic cho Product Assembly
    /// Service for handling Product Assembly business logic
    /// </summary>
    public class ProductAssemblyService
    {
        private readonly IProductAssemblyRepository _repository;
        
        public ProductAssemblyService(IProductAssemblyRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        /// <summary>
        /// Lấy thông tin sản phẩm và các bước lắp ráp theo ID
        /// Get product assembly information and items by ID
        /// </summary>
        /// <param name="productId">ID sản phẩm</param>
        /// <returns>ProductAssemblyDto</returns>
        public async Task<ProductAssemblyDto> GetProductAssemblyByIdAsync(int productId)
        {
            var productAssembly = await _repository.GetProductAssemblyByIdAsync(productId);
            
            if (productAssembly == null)
            {
                throw new Exception($"Product with ID {productId} not found");
            }
            
            // Map entity to DTO
            var dto = new ProductAssemblyDto
            {
                Id = productAssembly.Id,
                ProductName = productAssembly.ProductName,
                Description = productAssembly.Description,
                Cdt = productAssembly.Cdt,
                Udt = productAssembly.Udt
            };
            
            // Map items
            foreach (var item in productAssembly.Items)
            {
                dto.Items.Add(new AssemblyItemDto
                {
                    Id = item.Id,
                    Description = item.Description,
                    Note = item.Note,
                    Bolts = item.Bolts,
                    Force = item.Force,
                    Images = item.Images,
                    Type = item.Type,
                    Cdt = item.Cdt,
                    Udt = item.Udt
                });
            }
            
            return dto;
        }
    }
}
