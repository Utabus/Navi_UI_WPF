using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Application.Interfaces
{
    /// <summary>
    /// Interface quản lý liên kết ProductMaster - AssemblyItem (NaviProductMasterItem)
    /// </summary>
    public interface INaviProductMasterItemService
    {
        /// <summary>Lấy tất cả liên kết chưa bị xóa.</summary>
        Task<List<NaviProductMasterItemDto>> GetAllAsync();

        /// <summary>Lấy liên kết theo Id.</summary>
        Task<NaviProductMasterItemDto> GetByIdAsync(int id);

        /// <summary>Lấy danh sách steps cho một ProductMaster cụ thể.</summary>
        Task<List<NaviProductMasterItemDto>> GetByProductMasterAsync(int productMasterId);

        /// <summary>Lấy danh sách ProductMasters cho một Item cụ thể.</summary>
        Task<List<NaviProductMasterItemDto>> GetByItemAsync(int itemId);

        /// <summary>Tạo liên kết mới.</summary>
        Task<NaviProductMasterItemDto> CreateAsync(CreateNaviProductMasterItemDto dto);

        /// <summary>Xóa mềm liên kết.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
