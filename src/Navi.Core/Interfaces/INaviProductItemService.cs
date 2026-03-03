using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Core.Interfaces
{
    /// <summary>
    /// Interface quản lý liên kết NaviProductItem (many-to-many) — gọi tới /api/naviproductitems
    /// </summary>
    public interface INaviProductItemService
    {
        /// <summary>Lấy tất cả relationships.</summary>
        Task<List<NaviProductItemDto>> GetAllAsync();

        /// <summary>Lấy một relationship theo Id.</summary>
        Task<NaviProductItemDto> GetByIdAsync(int id);

        /// <summary>Lấy tất cả items thuộc một product.</summary>
        Task<List<NaviProductItemDto>> GetByProductAsync(int productId);

        /// <summary>Lấy tất cả products chứa một item.</summary>
        Task<List<NaviProductItemDto>> GetByItemAsync(int itemId);

        /// <summary>Kiểm tra một relationship đã tồn tại chưa.</summary>
        Task<bool> ExistsAsync(int productId, int itemId);

        /// <summary>Tạo relationship mới giữa product và item.</summary>
        Task<NaviProductItemDto> CreateAsync(CreateNaviProductItemDto dto);

        /// <summary>Xóa mềm một relationship.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
