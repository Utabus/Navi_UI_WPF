using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Application.Interfaces
{
    /// <summary>
    /// Interface quản lý sản phẩm NaviProduct — gọi tới /api/naviproducts
    /// </summary>
    public interface INaviProductService
    {
        Task<List<NaviProductDto>> GetAllAsync();
        Task<NaviProductDto> GetByIdAsync(int id);
        Task<NaviProductWithItemsDto> GetWithItemsAsync(int id);
        Task<List<NaviProductDto>> SearchAsync(string term);
        Task<NaviProductDto> CreateAsync(CreateNaviProductDto dto);
        Task<NaviProductDto> UpdateAsync(int id, UpdateNaviProductDto dto);
        Task<bool> DeleteAsync(int id);
        Task<NaviProductDto> CreateWithItemsAsync(CreateNaviProductWithItemsDto dto);
        Task<NaviProductDto> UpdateWithItemsAsync(int id, UpdateNaviProductWithItemsDto dto);
        Task<bool> DeleteWithItemsAsync(int id);
        Task<ImportResultDto> ImportExcelAsync(Stream fileStream, string fileName);
    }
}
