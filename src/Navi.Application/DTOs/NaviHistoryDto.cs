using System;

namespace Navi.Application.DTOs
{
    // ─────────────────────────────────────────────────────────────
    // Read DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO đọc lịch sử thao tác nhân viên NaviHistory
    /// </summary>
    public class NaviHistoryDto
    {
        public int Id { get; set; }
        public string NameNV { get; set; }
        public string CodeNV { get; set; }
        public string PO { get; set; }
        public string Step { get; set; }
        public int? ProductItemId { get; set; }
        public string Type { get; set; }
        public int? Count { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
        public bool IsDelete { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    // Write DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO ghi log thao tác mới
    /// </summary>
    public class CreateNaviHistoryDto
    {
        public string NameNV { get; set; }
        public string CodeNV { get; set; }
        public string PO { get; set; }
        public string Step { get; set; }
        public int? ProductItemId { get; set; }
        public string Type { get; set; }
        public int? Count { get; set; }
    }

    /// <summary>
    /// DTO cập nhật log thao tác
    /// </summary>
    public class UpdateNaviHistoryDto
    {
        public string NameNV { get; set; }
        public string CodeNV { get; set; }
        public string PO { get; set; }
        public string Step { get; set; }
        public int? ProductItemId { get; set; }
        public string Type { get; set; }
        public int? Count { get; set; }
    }
}
