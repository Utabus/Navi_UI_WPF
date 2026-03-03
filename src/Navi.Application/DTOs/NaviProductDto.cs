using System;
using System.Collections.Generic;

namespace Navi.Application.DTOs
{
    // ─────────────────────────────────────────────────────────────
    // Read DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO đọc thông tin sản phẩm NaviProduct
    /// </summary>
    public class NaviProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
        public bool IsDelete { get; set; }
    }

    /// <summary>
    /// DTO sản phẩm kèm danh sách các bước lắp ráp
    /// </summary>
    public class NaviProductWithItemsDto : NaviProductDto
    {
        public List<NaviItemDto> Items { get; set; }

        public NaviProductWithItemsDto()
        {
            Items = new List<NaviItemDto>();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Write DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO tạo sản phẩm mới
    /// </summary>
    public class CreateNaviProductDto
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO cập nhật sản phẩm
    /// </summary>
    public class UpdateNaviProductDto
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO tạo sản phẩm kèm danh sách Item trong một transaction
    /// </summary>
    public class CreateNaviProductWithItemsDto
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public List<int> ItemIds { get; set; }

        public CreateNaviProductWithItemsDto()
        {
            ItemIds = new List<int>();
        }
    }

    /// <summary>
    /// DTO cập nhật sản phẩm kèm danh sách Item trong một transaction
    /// </summary>
    public class UpdateNaviProductWithItemsDto
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public List<int> ItemIds { get; set; }

        public UpdateNaviProductWithItemsDto()
        {
            ItemIds = new List<int>();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Import Excel
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Kết quả import Excel
    /// </summary>
    public class ImportResultDto
    {
        public int TotalRows { get; set; }
        public int InsertedRows { get; set; }
        public int UpdatedRows { get; set; }
        public int SkippedRows { get; set; }
        public int FailedRows { get; set; }
        public List<ImportErrorDto> Errors { get; set; }

        public ImportResultDto()
        {
            Errors = new List<ImportErrorDto>();
        }
    }

    /// <summary>
    /// Chi tiết lỗi từng dòng khi import
    /// </summary>
    public class ImportErrorDto
    {
        public int RowNumber { get; set; }
        public string ProductName { get; set; }
        public string Reason { get; set; }
    }
}
