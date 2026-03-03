using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Core.Interfaces
{
    /// <summary>
    /// Interface quản lý lịch sử thao tác NaviHistory — gọi tới /api/navihistory
    /// </summary>
    public interface INaviHistoryService
    {
        /// <summary>Lấy tất cả history records chưa bị xóa.</summary>
        Task<List<NaviHistoryDto>> GetAllAsync();

        /// <summary>Lấy một history record theo Id.</summary>
        Task<NaviHistoryDto> GetByIdAsync(int id);

        /// <summary>Lấy lịch sử thao tác theo mã nhân viên.</summary>
        Task<List<NaviHistoryDto>> GetByCodeNVAsync(string codeNV);

        /// <summary>Lấy lịch sử thao tác theo ProductItem Id.</summary>
        Task<List<NaviHistoryDto>> GetByProductItemAsync(int productItemId);

        /// <summary>Lấy lịch sử thao tác theo Production Order.</summary>
        Task<List<NaviHistoryDto>> GetByPOAsync(string po);

        /// <summary>Tạo history record mới.</summary>
        Task<NaviHistoryDto> CreateAsync(CreateNaviHistoryDto dto);

        /// <summary>Cập nhật history record.</summary>
        Task<NaviHistoryDto> UpdateAsync(int id, UpdateNaviHistoryDto dto);

        /// <summary>Xóa mềm history record.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
