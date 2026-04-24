using System;

namespace Navi.Core.Entities
{
    /// <summary>
    /// Model so sánh History Snapshot vs Current Item (Drift Detection).
    /// </summary>
    public class NaviHistoryAuditComparison
    {
        public int HistoryId { get; set; }
        public string PO { get; set; }
        public DateTime AssemblyTime { get; set; }
        public NaviItemAudit SnapshotAtAssembly { get; set; }
        public AssemblyItem CurrentItem { get; set; }
        public bool HasDrift { get; set; }
    }
}
