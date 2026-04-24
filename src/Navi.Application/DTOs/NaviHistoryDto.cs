using System;
using Newtonsoft.Json;

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
        public string Device { get; set; }
        public string ProductName { get; set; }
        public int? Step { get; set; }
        public int? ItemId { get; set; }
        public int? ItemAuditId { get; set; }
        public int? ProductId { get; set; }
        public string Type { get; set; }
        public int? Count { get; set; }
        public bool? OK { get; set; }
        public bool? NG { get; set; }
        public string Note { get; set; }
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
        public string Device { get; set; }
        public string ProductName { get; set; }
        public int? Step { get; set; }
        public int? ItemId { get; set; }
        public int? ItemAuditId { get; set; }
        public int? ProductId { get; set; }
        public string Type { get; set; }
        public int? Count { get; set; }
        public bool? OK { get; set; }
        public bool? NG { get; set; }
        public string Note { get; set; }
    }

    /// <summary>
    /// DTO cập nhật log thao tác
    /// </summary>
    public class UpdateNaviHistoryDto : CreateNaviHistoryDto
    {
    }
}
