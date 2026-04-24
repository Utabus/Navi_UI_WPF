using System;

namespace Navi.Application.DTOs
{
    public class NaviItemAuditDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Version { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Bolts { get; set; }
        public string Force { get; set; }
        public string Images { get; set; }
        public string Type { get; set; }
        public int? Step { get; set; }
        public byte? Grease { get; set; }
        public byte? ForceBit { get; set; }
        public int? Timer { get; set; }
        public string ChangedBy { get; set; }
        public DateTime Cdt { get; set; }
    }
}
