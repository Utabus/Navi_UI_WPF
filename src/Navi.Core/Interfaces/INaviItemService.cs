using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Core.Interfaces
{
    /// <summary>
    /// Interface quản lý bước lắp ráp NaviItem — gọi tới /api/naviitems
    /// </summary>
    public interface INaviItemService
    {
        /// <summary>Lấy tất cả items chưa bị xóa.</summary>
        Task<List<NaviItemDto>> GetAllAsync();

        /// <summary>Lấy item theo Id.</summary>
        Task<NaviItemDto> GetByIdAsync(int id);

        /// <summary>Lấy item kèm danh sách sản phẩm liên kết.</summary>
        Task<NaviItemWithProductsDto> GetWithProductsAsync(int id);

        /// <summary>Lấy items theo loại (Type).</summary>
        Task<List<NaviItemDto>> GetByTypeAsync(string type);

        /// <summary>Tìm kiếm items theo từ khóa.</summary>
        Task<List<NaviItemDto>> SearchAsync(string term);

        /// <summary>Tạo item mới.</summary>
        Task<NaviItemDto> CreateAsync(CreateNaviItemDto dto);

        /// <summary>Cập nhật item.</summary>
        Task<NaviItemDto> UpdateAsync(int id, UpdateNaviItemDto dto);

        /// <summary>Xóa mềm item.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
