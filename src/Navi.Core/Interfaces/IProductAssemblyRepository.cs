using System.Threading.Tasks;
using Navi.Core.Entities;

namespace Navi.Core.Interfaces
{
    /// <summary>
    /// Interface cho Product Assembly Repository - quản lý truy cập dữ liệu sản phẩm lắp ráp
    /// Interface for Product Assembly Repository - manages product assembly data access
    /// </summary>
    public interface IProductAssemblyRepository
    {
        /// <summary>
        /// Lấy thông tin sản phẩm và các bước lắp ráp theo ID
        /// Get product assembly information and items by ID
        /// </summary>
        /// <param name="productId">ID sản phẩm</param>
        /// <returns>Product assembly with items</returns>
        Task<ProductAssembly> GetProductAssemblyByIdAsync(int productId);
    }
}
