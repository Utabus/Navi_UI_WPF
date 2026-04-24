using System;

namespace Navi.Application.DTOs
{
    /// <summary>
    /// DTO cho bước lắp ráp kèm trạng thái History (OK/NG)
    /// </summary>
    public class NaviItemStatusDto : NaviItemDto
    {
        public string PO { get; set; }
        public bool? NG { get; set; }
        public bool? OK { get; set; }
        public string HistoryNote { get; set; }
        public int? Count { get; set; }
        public int? ItemAuditId { get; set; }
        public int? ProductId { get; set; }
    }
}
