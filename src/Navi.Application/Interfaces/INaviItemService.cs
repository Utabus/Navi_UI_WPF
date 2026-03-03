using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Application.Interfaces
{
    /// <summary>
    /// Interface quản lý bước lắp ráp NaviItem — gọi tới /api/naviitems
    /// </summary>
    public interface INaviItemService
    {
        Task<List<NaviItemDto>> GetAllAsync();
        Task<NaviItemDto> GetByIdAsync(int id);
        Task<NaviItemWithProductsDto> GetWithProductsAsync(int id);
        Task<List<NaviItemDto>> GetByTypeAsync(string type);
        Task<List<NaviItemDto>> SearchAsync(string term);
        Task<NaviItemDto> CreateAsync(CreateNaviItemDto dto);
        Task<NaviItemDto> UpdateAsync(int id, UpdateNaviItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
