using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Application.Interfaces
{
    /// <summary>
    /// Interface quản lý lịch sử thao tác NaviHistory — gọi tới /api/navihistory
    /// </summary>
    public interface INaviHistoryService
    {
        Task<List<NaviHistoryDto>> GetAllAsync();
        Task<NaviHistoryDto> GetByIdAsync(int id);
        Task<List<NaviHistoryDto>> GetByCodeNVAsync(string codeNV);
        Task<List<NaviHistoryDto>> GetByProductItemAsync(int productItemId);
        Task<List<NaviHistoryDto>> GetByPOAsync(string po);
        Task<NaviHistoryDto> CreateAsync(CreateNaviHistoryDto dto);
        Task<NaviHistoryDto> UpdateAsync(int id, UpdateNaviHistoryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
