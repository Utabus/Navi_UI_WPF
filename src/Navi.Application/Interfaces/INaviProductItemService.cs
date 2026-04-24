using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Application.Interfaces
{
    /// <summary>
    /// Interface quản lý liên kết Sản phẩm - Bước lắp ráp (NaviProductItem)
    /// </summary>
    public interface INaviProductItemService
    {
        /// <summary>Lấy tất cả liên kết chưa bị xóa.</summary>
        Task<List<NaviProductItemDto>> GetAllAsync();

        /// <summary>Lấy liên kết theo Id.</summary>
        Task<NaviProductItemDto> GetByIdAsync(int id);

        /// <summary>Lấy danh sách steps cho một sản phẩm cụ thể.</summary>
        Task<List<NaviProductItemDto>> GetByProductAsync(int productId);

        /// <summary>Kiểm tra xem liên kết đã tồn tại chưa.</summary>
        Task<bool> ExistsAsync(int productId, int itemId);

        /// <summary>Tạo liên kết mới.</summary>
        Task<NaviProductItemDto> CreateAsync(CreateNaviProductItemDto dto);

        /// <summary>Xóa mềm liên kết.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
