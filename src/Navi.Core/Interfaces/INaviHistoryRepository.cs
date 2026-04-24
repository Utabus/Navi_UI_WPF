using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Core.Entities;

namespace Navi.Core.Interfaces
{
    /// <summary>
    /// Interface repository cho NaviHistory - Quản lý truy cập dữ liệu lịch sử thao tác qua API
    /// </summary>
    public interface INaviHistoryRepository : IDisposable
    {
        /// <summary>Lấy tất cả history records chưa bị xóa.</summary>
        Task<List<NaviHistory>> GetAllAsync();

        /// <summary>Lấy một history record theo Id.</summary>
        Task<NaviHistory> GetByIdAsync(int id);

        /// <summary>Lấy lịch sử thao tác theo mã nhân viên.</summary>
        Task<List<NaviHistory>> GetByCodeNVAsync(string codeNV);

        /// <summary>Lấy lịch sử thao tác theo Item Id.</summary>
        Task<List<NaviHistory>> GetByItemIdAsync(int itemId);

        /// <summary>Lấy lịch sử thao tác theo Production Order.</summary>
        Task<List<NaviHistory>> GetByPOAsync(string po);

        /// <summary>Lấy lịch sử so sánh Audit (Drift Detection).</summary>
        Task<List<NaviHistoryAuditComparison>> GetAuditComparisonAsync(int itemId);

        /// <summary>Tạo history record mới.</summary>
        Task<NaviHistory> CreateAsync(NaviHistory history);

        /// <summary>Cập nhật history record.</summary>
        Task<NaviHistory> UpdateAsync(NaviHistory history);

        /// <summary>Xóa mềm history record.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
