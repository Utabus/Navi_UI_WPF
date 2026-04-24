using System;

namespace Navi.Application.DTOs
{
    public class NaviFgDto
    {
        public int Id { get; set; }
        public string PO { get; set; }
        public double? MinFG { get; set; }
        public double? MaxFG { get; set; }
        public double? SizeBall { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
        public string CodeNV { get; set; }
        public DateTime? Cdt { get; set; }
        public DateTime? Udt { get; set; }
        public bool IsDelete { get; set; }
    }

    public class CreateNaviFgDto
    {
        public string PO { get; set; }
        public double? MinFG { get; set; }
        public double? MaxFG { get; set; }
        public double? SizeBall { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
        public string CodeNV { get; set; }
    }

    public class UpdateNaviFgDto : CreateNaviFgDto
    {
    }
}
