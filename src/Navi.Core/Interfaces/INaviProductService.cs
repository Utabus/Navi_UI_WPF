using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Core.Interfaces
{
    /// <summary>
    /// Interface quản lý sản phẩm NaviProduct — gọi tới /api/naviproducts
    /// </summary>
    public interface INaviProductService
    {
        /// <summary>Lấy tất cả sản phẩm chưa bị xóa.</summary>
        Task<List<NaviProductDto>> GetAllAsync();

        /// <summary>Lấy sản phẩm theo Id.</summary>
        Task<NaviProductDto> GetByIdAsync(int id);

        /// <summary>Lấy sản phẩm kèm danh sách items liên kết.</summary>
        Task<NaviProductWithItemsDto> GetWithItemsAsync(int id);

        /// <summary>Tìm kiếm sản phẩm theo từ khóa.</summary>
        Task<List<NaviProductDto>> SearchAsync(string term);

        /// <summary>Tạo sản phẩm mới.</summary>
        Task<NaviProductDto> CreateAsync(CreateNaviProductDto dto);

        /// <summary>Cập nhật thông tin sản phẩm.</summary>
        Task<NaviProductDto> UpdateAsync(int id, UpdateNaviProductDto dto);

        /// <summary>Xóa mềm sản phẩm.</summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>Tạo sản phẩm kèm danh sách items trong một transaction.</summary>
        Task<NaviProductDto> CreateWithItemsAsync(CreateNaviProductWithItemsDto dto);

        /// <summary>Cập nhật sản phẩm và danh sách items trong một transaction.</summary>
        Task<NaviProductDto> UpdateWithItemsAsync(int id, UpdateNaviProductWithItemsDto dto);

        /// <summary>Xóa mềm sản phẩm và toàn bộ ProductItems liên quan.</summary>
        Task<bool> DeleteWithItemsAsync(int id);

        /// <summary>Import hàng loạt từ file Excel.</summary>
        Task<ImportResultDto> ImportExcelAsync(Stream fileStream, string fileName);
    }
}
