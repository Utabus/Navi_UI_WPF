using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Application.Interfaces
{
    /// <summary>
    /// Interface quản lý liên kết NaviProductItem (many-to-many) — gọi tới /api/naviproductitems
    /// </summary>
    public interface INaviProductItemService
    {
        Task<List<NaviProductItemDto>> GetAllAsync();
        Task<NaviProductItemDto> GetByIdAsync(int id);
        Task<List<NaviProductItemDto>> GetByProductAsync(int productId);
        Task<List<NaviProductItemDto>> GetByItemAsync(int itemId);
        Task<bool> ExistsAsync(int productId, int itemId);
        Task<NaviProductItemDto> CreateAsync(CreateNaviProductItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
