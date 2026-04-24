using System;
using Newtonsoft.Json;

namespace Navi.Core.Entities
{
    /// <summary>
    /// Entity đại diện cho lịch sử thao tác nhân viên (LXA_NAVI_HISTORY)
    /// </summary>
    public class NaviHistory
    {
        public int Id { get; set; }
        public string NameNV { get; set; }
        public string CodeNV { get; set; }
        public string PO { get; set; }
        public int? Step { get; set; }
        public int? ItemId { get; set; }
        public int? ItemAuditId { get; set; }
        public int? ProductId { get; set; }
        public string Type { get; set; }
        public int? Count { get; set; }
        public bool? OK { get; set; }
        public bool? NG { get; set; }
        public string Note { get; set; }
        public string Device { get; set; }
        public string ProductName { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
        public bool IsDelete { get; set; }

        public NaviHistory()
        {
            Cdt = DateTime.Now;
            Udt = DateTime.Now;
            IsDelete = false;
        }
    }
}
