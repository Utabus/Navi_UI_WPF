using System;
using System.Collections.Generic;

namespace Navi.Application.DTOs
{
    // ─────────────────────────────────────────────────────────────
    // Read DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO đọc thông tin bước lắp ráp NaviItem
    /// </summary>
    public class NaviItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Bolts { get; set; }
        public string Force { get; set; }
        public string Images { get; set; }
        public string Type { get; set; }
        public int? Step { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
        public bool IsDelete { get; set; }
    }

    /// <summary>
    /// DTO bước lắp ráp kèm danh sách sản phẩm liên kết
    /// </summary>
    public class NaviItemWithProductsDto : NaviItemDto
    {
        public List<NaviProductDto> Products { get; set; }

        public NaviItemWithProductsDto()
        {
            Products = new List<NaviProductDto>();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Write DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO tạo bước lắp ráp mới
    /// </summary>
    public class CreateNaviItemDto
    {
        public string Description { get; set; }
        public string Note { get; set; }
        public string Bolts { get; set; }
        public string Force { get; set; }
        public string Images { get; set; }
        public string Type { get; set; }
        public int? Step { get; set; }
    }

    /// <summary>
    /// DTO cập nhật bước lắp ráp
    /// </summary>
    public class UpdateNaviItemDto
    {
        public string Description { get; set; }
        public string Note { get; set; }
        public string Bolts { get; set; }
        public string Force { get; set; }
        public string Images { get; set; }
        public string Type { get; set; }
        public int? Step { get; set; }
    }
}
