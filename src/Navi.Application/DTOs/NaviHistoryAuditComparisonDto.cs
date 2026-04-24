using System;

namespace Navi.Application.DTOs
{
    public class NaviHistoryAuditComparisonDto
    {
        public int HistoryId { get; set; }
        public string PO { get; set; }
        public DateTime AssemblyTime { get; set; }
        public NaviItemAuditDto SnapshotAtAssembly { get; set; }
        public NaviItemDto CurrentItem { get; set; }
        public bool HasDrift { get; set; }
    }
}
